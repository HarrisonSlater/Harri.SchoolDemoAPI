using FluentAssertions;
using Harri.SchoolDemoAPI.Models.Attributes;
using Harri.SchoolDemoAPI.Models.Dto;

namespace Harri.SchoolDemoAPI.Tests.Unit
{
    [TestFixture]
    public class ValidGPAQueryDtoAttributeTests
    {
        private ValidGPAAttribute _validGPAQueryDtoAttribute;

        [SetUp]
        public void Setup()
        {
            _validGPAQueryDtoAttribute = new ValidGPAAttribute();
        }

        private static IEnumerable<TestCaseData> GetGPAQueryStudentsTestCases()
        {
            yield return new TestCaseData(new GPAQueryDto() { GPA = null }, true);

            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Gt = 4 } }, true);
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Lt = 4 } }, true);
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Gt = 4, Lt = 4 } }, true);
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Eq = 4 } }, true);

            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Eq = 4, Gt = 4 } }, false);
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Eq = 4, Lt = 4 } }, false);
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Eq = 4, Gt = 4, Lt = 4 } }, false);

            foreach (var isNullValue in new bool[] { true, false })
            {
                yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Gt = 4, IsNull = isNullValue } }, false);
                yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Lt = 4, IsNull = isNullValue } }, false);
                yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Gt = 4, Lt = 4, IsNull = isNullValue } }, false);
                yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Eq = 4, IsNull = isNullValue } }, false);
            }
        }

        [TestCaseSource(nameof(GetGPAQueryStudentsTestCases))]
        public void QueryStudents_ShouldReturnExpectedResponseType_ForValidAndInvalidQueries(GPAQueryDto gpaQueryDto, bool expectedValidationResult)
        {
            // Arrange
            // Act
            var result = _validGPAQueryDtoAttribute.IsValid(gpaQueryDto.GPA);

            // Assert
            result.Should().Be(expectedValidationResult);
        }

        [Test]
        public void QueryStudents_ShouldReturnTrueOnNull()
        {
            // Arrange
            // Act
            var result = _validGPAQueryDtoAttribute.IsValid(null);

            // Assert
            result.Should().Be(true);
        }
    }
}