using FluentAssertions;
using Harri.SchoolDemoAPI.Models.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Harri.SchoolDemoAPI.Tests.Unit
{
    [TestFixture]
    public class PositiveAttributeTests
    {
        private PositiveIntAttribute _positiveIntAttribute;
        private PositiveDecimalAttribute _positiveDecimalAttribute;

        [SetUp]
        public void Setup()
        {
            _positiveIntAttribute = new PositiveIntAttribute();
            _positiveDecimalAttribute = new PositiveDecimalAttribute();
        }

        [TestCase(2, true)]
        [TestCase(1, true)]
        [TestCase(0, false)]
        [TestCase(-1, false)]
        [TestCase(-100, false)]
        [TestCase(2.1, true)]
        [TestCase(2.11, true)]
        [TestCase(null, true)]
        [TestCase(2.111, true)]
        [TestCase(2111, true)]
        public void PositiveAttributes_ShouldValidateCorrectly(decimal? d, bool expectedResult)
        {
            // Arrange
            var validationContextDecimal = new ValidationContext(new TestDTO() { DecimalToTest = d }) { MemberName = "DecimalToTest" };
            var validationContextInt = new ValidationContext(new TestDTO() { IntToTest = (int?)d }) { MemberName = "IntToTest" };

            // Act
            var decimalResult = _positiveDecimalAttribute.GetValidationResult(d, validationContextDecimal);
            var intResult = _positiveIntAttribute.GetValidationResult(d, validationContextInt);

            // Assert
            if (expectedResult is true)
            {
                decimalResult.Should().Be(ValidationResult.Success);
                intResult.Should().Be(ValidationResult.Success);
            }
            else
            {
                decimalResult.Should().NotBe(ValidationResult.Success);
                intResult.Should().NotBe(ValidationResult.Success);
                decimalResult?.ErrorMessage.Should().NotBeNullOrEmpty();
                intResult?.ErrorMessage.Should().NotBeNullOrEmpty();
            }
        }

        [TestCase(0.1)]
        [TestCase(0.01)]
        [TestCase(0.001)]
        [TestCase(0.0000001)]
        public void PositiveDecimalAttribute_ShouldValidateCorrectly(decimal? d)
        {
            // Arrange
            var validationContextDecimal = new ValidationContext(new TestDTO() { DecimalToTest = d }) { MemberName = "DecimalToTest" };

            // Act
            var decimalResult = _positiveDecimalAttribute.GetValidationResult(d, validationContextDecimal);

            // Assert
            decimalResult.Should().Be(ValidationResult.Success);
        }

        public class TestDTO
        {
            public decimal? DecimalToTest { get; set; }
            public int? IntToTest { get; set; }
        }
    }
}
