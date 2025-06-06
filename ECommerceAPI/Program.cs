using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Infrastructure.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Services;
using ECommerceAPI.Infrastructure.Interfaces;
using ECommerceAPI.Infrastructure.Repositories;
using ECommerceAPI.Infrastructure.Repositories.Interfaces;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.Validators;
using FluentValidation;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load configuration
var configuration = builder.Configuration;

// ✅ Configure Database
var connectionString = configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, b => b.MigrationsAssembly("ECommerceAPI"))); // 🔥 Ensures migrations are created in Infrastructure

// ✅ Load JWT settings
var jwtSettings = configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

if (key.Length < 32)
{
    throw new InvalidOperationException("JWT Key is too short! Must be at least 32 characters.");
}

// ✅ Configure Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero // Ensures tokens expire exactly on time
        };
    });

builder.Services.AddAuthorization();

// ✅ Add Application Services
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<ISizeService, SizeService>();
builder.Services.AddScoped<IAddressService, AddressService>();


builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ISizeRepository, SizeRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();

builder.Services.AddScoped<IValidator<ProductUpsertRequest>, ProductUpsertRequestValidator>();
builder.Services.AddScoped<IValidator<OrderCreateRequest>, OrderCreateRequestValidator>();
builder.Services.AddScoped<IValidator<OrderStatusUpdateRequest>, OrderStatusUpdateRequestValidator>();
builder.Services.AddScoped<IValidator<PaymentStatusUpdateRequest>, PaymentStatusUpdateRequestValidator>();
builder.Services.AddScoped<IValidator<UserLoginRequest>, UserLoginRequestValidator>();
builder.Services.AddScoped<IValidator<UserRegisterRequest>, UserRegisterRequestValidator>();
builder.Services.AddScoped<IValidator<CartAddOrUpdateItemRequest>, CartAddOrUpdateItemRequestValidator>();
builder.Services.AddScoped<IValidator<AddressUpsertRequest>, AddressUpsertRequestValidator>();
builder.Services.AddScoped<IValidator<UserUpdateRequest>, UserUpdateRequestValidator>();


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


// ✅ Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// ✅ Add Controllers, Swagger & Exception Handling
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "E-Commerce API",
        Version = "v1",
        Description = "An API for managing users, orders, and products.",
        Contact = new OpenApiContact
        {
            Name = "Sandeep Barla",
            Email = "barlavenkatsandeep@gmail.com",
            Url = new Uri("https://github.com/SandeepBarla")
        }
    });

    // ✅ Enable JWT Authentication in Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter 'Bearer YOUR_TOKEN_HERE' in the box below",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// ✅ Apply EF Core migrations automatically at startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate(); // Applies any pending migrations to Supabase

    // ✅ Seed initial data
    await ECommerceAPI.Infrastructure.Context.DataSeeder.SeedDataAsync(dbContext);
}

// Register Global Exception Handling Middleware
app.UseMiddleware<ECommerceAPI.WebApi.Middleware.ExceptionHandlingMiddleware>();

// ✅ Exception Handling Middleware (Prevents crashes)
app.UseExceptionHandler("/error");

// ✅ Enable CORS
app.UseCors("AllowAll");

// ✅ Enable Authentication & Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Commerce API v1");
        c.RoutePrefix = "swagger"; // ✅ Ensures Swagger is available at /swagger
    });
}

// ✅ Map Controllers
app.MapControllers();

// ✅ Run the Application
app.Run();
