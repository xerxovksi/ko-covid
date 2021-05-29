namespace KO.Covid.Domain.Test
{
    using FluentAssertions;
    using KO.Covid.Domain.Test.Models;
    using System.Collections.Generic;
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

        [Fact]
        public void AddRange_ShouldAddRangeOf_UniqueElements_ToHashSet()
        {
            // Arrange
            var originalSet = new HashSet<string>
            {
                "2", "3", "5", "7", "11", "13"
            };

            var newSet = new HashSet<string>
            {
                "11", "13", "15", "17", "19"
            };

            var expectedResultSet = new HashSet<string>
            {
                "2", "3", "5", "7", "11", "13", "15", "17", "19"
            };

            // Act
            var resultSet = originalSet.AddRange(newSet);

            // Assert
            resultSet.Should().BeEquivalentTo(expectedResultSet);
        }
    }
}
