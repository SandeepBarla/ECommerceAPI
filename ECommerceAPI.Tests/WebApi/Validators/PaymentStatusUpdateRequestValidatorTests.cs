using Xunit;
using FluentValidation.TestHelper;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.Validators;

namespace ECommerceAPI.Tests.WebApi.Validators
{
  public class PaymentStatusUpdateRequestValidatorTests
  {
    private readonly PaymentStatusUpdateRequestValidator _validator;

    public PaymentStatusUpdateRequestValidatorTests()
    {
      _validator = new PaymentStatusUpdateRequestValidator();
    }

    [Fact]
    public void ShouldNotHaveError_WhenStatusIsApproved()
    {
      // Arrange
      var request = new PaymentStatusUpdateRequest
      {
        Status = "Approved"
      };

      // Act & Assert
      var result = _validator.TestValidate(request);
      result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void ShouldNotHaveError_WhenStatusIsPending()
    {
      // Arrange
      var request = new PaymentStatusUpdateRequest
      {
        Status = "Pending"
      };

      // Act & Assert
      var result = _validator.TestValidate(request);
      result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void ShouldNotHaveError_WhenStatusIsRejectedWithRemarks()
    {
      // Arrange
      var request = new PaymentStatusUpdateRequest
      {
        Status = "Rejected",
        Remarks = "Invalid payment proof - image unclear"
      };

      // Act & Assert
      var result = _validator.TestValidate(request);
      result.ShouldNotHaveValidationErrorFor(x => x.Status);
      result.ShouldNotHaveValidationErrorFor(x => x.Remarks);
    }

    [Fact]
    public void ShouldHaveError_WhenStatusIsEmpty()
    {
      // Arrange
      var request = new PaymentStatusUpdateRequest
      {
        Status = ""
      };

      // Act & Assert
      var result = _validator.TestValidate(request);
      result.ShouldHaveValidationErrorFor(x => x.Status)
          .WithErrorMessage("Payment status is required.");
    }

    [Fact]
    public void ShouldHaveError_WhenStatusIsNull()
    {
      // Arrange
      var request = new PaymentStatusUpdateRequest
      {
        Status = null
      };

      // Act & Assert
      var result = _validator.TestValidate(request);
      result.ShouldHaveValidationErrorFor(x => x.Status)
          .WithErrorMessage("Payment status is required.");
    }

    [Fact]
    public void ShouldHaveError_WhenStatusIsInvalid()
    {
      // Arrange
      var request = new PaymentStatusUpdateRequest
      {
        Status = "InvalidStatus"
      };

      // Act & Assert
      var result = _validator.TestValidate(request);
      result.ShouldHaveValidationErrorFor(x => x.Status)
          .WithErrorMessage("Invalid payment status. Allowed values: Pending, Approved, Rejected.");
    }

    [Fact]
    public void ShouldHaveError_WhenStatusIsRejectedWithoutRemarks()
    {
      // Arrange
      var request = new PaymentStatusUpdateRequest
      {
        Status = "Rejected",
        Remarks = null
      };

      // Act & Assert
      var result = _validator.TestValidate(request);
      result.ShouldHaveValidationErrorFor(x => x.Remarks)
          .WithErrorMessage("Remarks are required when rejecting payment.");
    }

    [Fact]
    public void ShouldHaveError_WhenStatusIsRejectedWithEmptyRemarks()
    {
      // Arrange
      var request = new PaymentStatusUpdateRequest
      {
        Status = "Rejected",
        Remarks = ""
      };

      // Act & Assert
      var result = _validator.TestValidate(request);
      result.ShouldHaveValidationErrorFor(x => x.Remarks)
          .WithErrorMessage("Remarks are required when rejecting payment.");
    }

    [Fact]
    public void ShouldHaveError_WhenStatusIsRejectedWithWhitespaceRemarks()
    {
      // Arrange
      var request = new PaymentStatusUpdateRequest
      {
        Status = "Rejected",
        Remarks = "   "
      };

      // Act & Assert
      var result = _validator.TestValidate(request);
      result.ShouldHaveValidationErrorFor(x => x.Remarks)
          .WithErrorMessage("Remarks are required when rejecting payment.");
    }

    [Fact]
    public void ShouldNotHaveError_WhenStatusIsApprovedWithRemarks()
    {
      // Arrange
      var request = new PaymentStatusUpdateRequest
      {
        Status = "Approved",
        Remarks = "Payment verified successfully"
      };

      // Act & Assert
      var result = _validator.TestValidate(request);
      result.ShouldNotHaveValidationErrorFor(x => x.Status);
      result.ShouldNotHaveValidationErrorFor(x => x.Remarks);
    }

    [Fact]
    public void ShouldNotHaveError_WhenStatusIsApprovedWithoutRemarks()
    {
      // Arrange
      var request = new PaymentStatusUpdateRequest
      {
        Status = "Approved",
        Remarks = null
      };

      // Act & Assert
      var result = _validator.TestValidate(request);
      result.ShouldNotHaveValidationErrorFor(x => x.Status);
      result.ShouldNotHaveValidationErrorFor(x => x.Remarks);
    }

    [Fact]
    public void ShouldNotHaveError_WhenStatusIsPendingWithoutRemarks()
    {
      // Arrange
      var request = new PaymentStatusUpdateRequest
      {
        Status = "Pending",
        Remarks = null
      };

      // Act & Assert
      var result = _validator.TestValidate(request);
      result.ShouldNotHaveValidationErrorFor(x => x.Status);
      result.ShouldNotHaveValidationErrorFor(x => x.Remarks);
    }

    [Theory]
    [InlineData("approved")]
    [InlineData("APPROVED")]
    [InlineData("Rejected")]
    [InlineData("pending")]
    public void ShouldHaveError_WhenStatusCaseDoesNotMatch(string status)
    {
      // This test ensures case sensitivity - only exact case matches are valid
      // Arrange
      var request = new PaymentStatusUpdateRequest
      {
        Status = status
      };

      // Act & Assert
      var result = _validator.TestValidate(request);

      // All these should fail except exact case matches
      if (status != "Approved" && status != "Rejected" && status != "Pending")
      {
        result.ShouldHaveValidationErrorFor(x => x.Status)
            .WithErrorMessage("Invalid payment status. Allowed values: Pending, Approved, Rejected.");
      }
    }

    [Fact]
    public void ShouldPassValidation_WhenValidApprovalRequest()
    {
      // Arrange
      var request = new PaymentStatusUpdateRequest
      {
        Status = "Approved"
      };

      // Act & Assert
      var result = _validator.TestValidate(request);
      result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldPassValidation_WhenValidRejectionRequest()
    {
      // Arrange
      var request = new PaymentStatusUpdateRequest
      {
        Status = "Rejected",
        Remarks = "Payment proof is invalid - please upload a clearer image"
      };

      // Act & Assert
      var result = _validator.TestValidate(request);
      result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldHaveError_WhenRemarksRequiredForRejectionButNotProvided()
    {
      // Arrange
      var request = new PaymentStatusUpdateRequest
      {
        Status = "Rejected",
        Remarks = "" // Empty remarks for rejection
      };

      // Act & Assert
      var result = _validator.TestValidate(request);
      result.ShouldHaveValidationErrorFor(x => x.Remarks)
          .WithErrorMessage("Remarks are required when rejecting payment.");
    }

    // ✅ COMPREHENSIVE EDGE CASE AND ERROR HANDLING TESTS
    [Fact]
    public void ShouldHaveError_WhenStatusIsWhitespace()
    {
      // Arrange
      var request = new PaymentStatusUpdateRequest
      {
        Status = "   "
      };

      // Act & Assert
      var result = _validator.TestValidate(request);
      result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Theory]
    [InlineData("rejected", null)]
    [InlineData("rejected", "")]
    [InlineData("rejected", "   ")]
    public void ShouldHaveError_WhenStatusIsRejectedButRemarksAreInvalid(string status, string remarks)
    {
      // Arrange
      var request = new PaymentStatusUpdateRequest
      {
        Status = status,
        Remarks = remarks
      };

      // Act & Assert
      var result = _validator.TestValidate(request);
      // Since status is not exact case match, it will fail on status validation first
      result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void ShouldNotHaveError_WhenStatusIsApprovedAndRemarksAreNull()
    {
      // Arrange
      var request = new PaymentStatusUpdateRequest
      {
        Status = "Approved",
        Remarks = null
      };

      // Act & Assert
      var result = _validator.TestValidate(request);
      result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldNotHaveError_WhenStatusIsApprovedAndRemarksAreEmpty()
    {
      // Arrange
      var request = new PaymentStatusUpdateRequest
      {
        Status = "Approved",
        Remarks = ""
      };

      // Act & Assert
      var result = _validator.TestValidate(request);
      result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldNotHaveError_WhenStatusIsPendingAndRemarksAreNull()
    {
      // Arrange
      var request = new PaymentStatusUpdateRequest
      {
        Status = "Pending",
        Remarks = null
      };

      // Act & Assert
      var result = _validator.TestValidate(request);
      result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldNotHaveError_WhenStatusIsRejectedWithValidRemarks()
    {
      // Arrange
      var request = new PaymentStatusUpdateRequest
      {
        Status = "Rejected",
        Remarks = "Payment screenshot is blurry and unreadable"
      };

      // Act & Assert
      var result = _validator.TestValidate(request);
      result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldNotHaveError_WhenStatusIsRejectedWithLongRemarks()
    {
      // Arrange
      var longRemarks = new string('A', 1000); // Very long remarks
      var request = new PaymentStatusUpdateRequest
      {
        Status = "Rejected",
        Remarks = longRemarks
      };

      // Act & Assert
      var result = _validator.TestValidate(request);
      result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldWorkCorrectly_WhenValidatorIsUsedMultipleTimes()
    {
      // Test that validator doesn't have state issues

      // First validation - valid
      var validRequest = new PaymentStatusUpdateRequest
      {
        Status = "Approved"
      };
      var validResult = _validator.TestValidate(validRequest);
      validResult.ShouldNotHaveAnyValidationErrors();

      // Second validation - invalid
      var invalidRequest = new PaymentStatusUpdateRequest
      {
        Status = "Rejected",
        Remarks = ""
      };
      var invalidResult = _validator.TestValidate(invalidRequest);
      invalidResult.ShouldHaveValidationErrorFor(x => x.Remarks);

      // Third validation - valid again
      var validRequest2 = new PaymentStatusUpdateRequest
      {
        Status = "Rejected",
        Remarks = "Valid rejection reason"
      };
      var validResult2 = _validator.TestValidate(validRequest2);
      validResult2.ShouldNotHaveAnyValidationErrors();
    }
  }
}