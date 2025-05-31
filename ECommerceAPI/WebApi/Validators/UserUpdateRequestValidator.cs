using FluentValidation;
using ECommerceAPI.WebApi.DTOs.RequestModels;

namespace ECommerceAPI.WebApi.Validators
{
  public class UserUpdateRequestValidator : AbstractValidator<UserUpdateRequest>
  {
    public UserUpdateRequestValidator()
    {
      RuleFor(x => x.FullName)
          .NotEmpty().WithMessage("Full name is required.")
          .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

      RuleFor(x => x.Phone)
          .Matches(@"^\d{10}$").WithMessage("Phone number must be exactly 10 digits.")
          .When(x => !string.IsNullOrEmpty(x.Phone));
    }
  }
}