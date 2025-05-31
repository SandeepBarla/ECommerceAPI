using FluentValidation;
using ECommerceAPI.WebApi.DTOs.RequestModels;

namespace ECommerceAPI.WebApi.Validators
{
  public class AddressUpsertRequestValidator : AbstractValidator<AddressUpsertRequest>
  {
    public AddressUpsertRequestValidator()
    {
      RuleFor(x => x.Name)
          .NotEmpty().WithMessage("Name is required.")
          .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

      RuleFor(x => x.Street)
          .NotEmpty().WithMessage("Street address is required.")
          .MaximumLength(200).WithMessage("Street address must not exceed 200 characters.");

      RuleFor(x => x.City)
          .NotEmpty().WithMessage("City is required.")
          .MaximumLength(50).WithMessage("City must not exceed 50 characters.");

      RuleFor(x => x.State)
          .NotEmpty().WithMessage("State is required.")
          .MaximumLength(50).WithMessage("State must not exceed 50 characters.");

      RuleFor(x => x.Pincode)
          .NotEmpty().WithMessage("Pincode is required.")
          .Matches(@"^\d{6}$").WithMessage("Pincode must be exactly 6 digits.");

      RuleFor(x => x.Phone)
          .NotEmpty().WithMessage("Phone number is required.")
          .Matches(@"^\d{10}$").WithMessage("Phone number must be exactly 10 digits.");
    }
  }
}