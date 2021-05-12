namespace KO.Covid.Domain.Test
{
    using FluentAssertions;
    using KO.Covid.Domain.Test.Models;
    using Xunit;

    public class DomainExtensionsTests
    {
        [Fact]
        public void CheckNull_ShouldReturnNull_IfAllUnderlyingProperties_AreNull()
        {
            // Arrange
            var person = new Person
            {
                Id = null,
                Name = null,
                Age = null,
                Comorbidities = null
            };

            // Act
            var result = person.CheckNull();

            // Assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData("1001", null, null, null)]
        [InlineData(null, "Rahul", 35, null)]
        [InlineData(null, null, null, new string[] { })]
        [InlineData(null, null, null, new string[] { "Diabetes", "Thyroid" })]
        public void CheckNull_ShouldNotReturnNull_IfAtleast1UnderlyingProperties_IsNotNull(
            string id,
            string name,
            int? age,
            string[] comorbidities)
        {
            // Arrange
            var person = new Person
            {
                Id = id,
                Name = name,
                Age = age,
                Comorbidities = comorbidities
            };

            // Act
            var result = person.CheckNull();

            // Assert
            result.Should().NotBeNull();
        }
    }
}
