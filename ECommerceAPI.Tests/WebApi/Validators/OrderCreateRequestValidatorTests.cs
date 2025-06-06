using Xunit;
using FluentValidation.TestHelper;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.Validators;
using System.Collections.Generic;
namespace ECommerceAPI.Tests.WebApi.Validators;

public class OrderCreateRequestValidatorTests
{
    private readonly OrderCreateRequestValidator _validator;

    public OrderCreateRequestValidatorTests()
    {
        _validator = new OrderCreateRequestValidator();
    }

    [Fact]
    public void Should_HaveError_When_OrderProducts_IsEmpty()
    {
        var request = new OrderCreateRequest
        {
            OrderProducts = new List<OrderProductRequest>(), // Empty list
            TotalAmount = 100,
            AddressId = 1
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.OrderProducts)
            .WithErrorMessage("Order must have at least one product.");
    }

    [Fact]
    public void Should_HaveError_When_OrderProduct_Has_Invalid_ProductId()
    {
        var request = new OrderCreateRequest
        {
            OrderProducts = new List<OrderProductRequest>
            {
                new OrderProductRequest { ProductId = 0, Quantity = 2 } // Invalid ProductId
            },
            TotalAmount = 100,
            AddressId = 1
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor("OrderProducts[0].ProductId")
            .WithErrorMessage("Product ID must be greater than zero.");
    }

    [Fact]
    public void Should_HaveError_When_OrderProduct_Has_Invalid_Quantity()
    {
        var request = new OrderCreateRequest
        {
            OrderProducts = new List<OrderProductRequest>
            {
                new OrderProductRequest { ProductId = 1, Quantity = 0 } // Invalid Quantity
            },
            TotalAmount = 100,
            AddressId = 1
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor("OrderProducts[0].Quantity")
            .WithErrorMessage("Quantity must be at least 1.");
    }

    [Fact]
    public void Should_HaveError_When_TotalAmount_Is_Zero()
    {
        var request = new OrderCreateRequest
        {
            OrderProducts = new List<OrderProductRequest>
            {
                new OrderProductRequest { ProductId = 1, Quantity = 2 }
            },
            TotalAmount = 0, // Invalid TotalAmount
            AddressId = 1
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.TotalAmount)
            .WithErrorMessage("Total amount must be greater than zero.");
    }

    [Fact]
    public void Should_HaveError_When_AddressId_Is_Invalid()
    {
        var request = new OrderCreateRequest
        {
            OrderProducts = new List<OrderProductRequest>
            {
                new OrderProductRequest { ProductId = 1, Quantity = 2 }
            },
            TotalAmount = 100,
            AddressId = 0 // Invalid AddressId
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.AddressId)
            .WithErrorMessage("Address ID must be valid.");
    }

    [Fact]
    public void Should_NotHaveError_When_Request_Is_Valid()
    {
        var request = new OrderCreateRequest
        {
            OrderProducts = new List<OrderProductRequest>
            {
                new OrderProductRequest { ProductId = 1, Quantity = 2 }
            },
            TotalAmount = 100,
            AddressId = 1
        };

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_NotHaveError_When_AddressId_Is_Null()
    {
        var request = new OrderCreateRequest
        {
            OrderProducts = new List<OrderProductRequest>
            {
                new OrderProductRequest { ProductId = 1, Quantity = 2 }
            },
            TotalAmount = 100,
            AddressId = null // Nullable for backward compatibility
        };

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(r => r.AddressId);
    }
}