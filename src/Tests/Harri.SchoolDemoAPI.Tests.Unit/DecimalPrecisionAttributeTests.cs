using FluentAssertions;
using Harri.SchoolDemoAPI.Models.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Harri.SchoolDemoAPI.Tests.Unit
{
    [TestFixture]
    public class DecimalPrecisionAttributeTests
    {
        private DecimalPrecisionAttribute _decimalPrecisionAttribute;

        [SetUp]
        public void Setup()
        {
            _decimalPrecisionAttribute = new DecimalPrecisionAttribute(3, 2);
        }

        [TestCase(2, true)]
        [TestCase(0, true)]
        [TestCase(2.1, true)]
        [TestCase(2.11, true)]
        [TestCase(null, true)]
        [TestCase(2.111, false)]
        [TestCase(2111, false)]
        public void DecimalPrecisionAttribute_ShouldValidateCorrectly(decimal? d, bool expectedResult)
        {
            // Arrange
            var validationContext = new ValidationContext(new TestDTO() { DecimalToTest = d }) { MemberName = "DecimalToTest" };

            // Act

            var result = _decimalPrecisionAttribute.GetValidationResult(d, validationContext);

            // Assert
            if (expectedResult is true)
            {
                result.Should().Be(ValidationResult.Success);
            }
            else
            {
                result.Should().NotBe(ValidationResult.Success);
                result?.ErrorMessage.Should().NotBeNullOrEmpty();
            }
        }

        public class TestDTO
        {
            public decimal? DecimalToTest { get; set; }
        }
    }
}
