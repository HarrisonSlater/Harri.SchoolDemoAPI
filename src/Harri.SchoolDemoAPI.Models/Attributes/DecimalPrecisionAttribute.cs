using FluentValidation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Harri.SchoolDemoAPI.Models.Attributes
{
    /// <summary>
    /// Attribute wrapper for FluentValidation PrecisionScale
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DecimalPrecisionAttribute : ValidationAttribute
    {
        private int _maxDigits;
        private int _precision;

        public DecimalPrecisionAttribute(int maxDigits, int precision)
        {
            _maxDigits = maxDigits;
            _precision = precision;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null) return ValidationResult.Success;

            var propInfo = validationContext.ObjectType.GetProperty(validationContext.MemberName);

            var d = (decimal)value;

            var validator = new DecimalPrecisionValidator(_maxDigits, _precision, propInfo.Name);
            var result = validator.Validate(d);

            if (result.Errors.Any())
            {
                ErrorMessage = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));
            }

            return result.IsValid ? ValidationResult.Success : new ValidationResult(ErrorMessage, new string[1]{propInfo.Name});
        }
    }

    public class DecimalPrecisionValidator : AbstractValidator<decimal>
    {
        public DecimalPrecisionValidator(int maxDigits, int precision, string propertyName)
        {
            RuleFor(x => x).PrecisionScale(maxDigits, precision, false).WithName(propertyName);
        }
    }
}
