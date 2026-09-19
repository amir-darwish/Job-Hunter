using Jobtri.Domain.Entities;

namespace Jobtri.UnitTests.Domain.Entities
{
    public sealed class CompanyTests
    {
        [Fact]
        public void Constructor_WithValidName_ShouldCreateEnabledCompany()
        {
            // Arrange
            string name = "Atos";

            // Act
            var company = new Company(name);

            // Assert
            Assert.Equal(name, company.Name);
            Assert.True(company.IsEnabled);
            Assert.Null(company.Website);
        }

        [Fact]
        public void Constructor_WithNameContainingSpaces_ShouldTrimName()
        {
            // Arrange
            string name = "   Atos   ";

            // Act
            var company = new Company(name);

            // Assert
            Assert.Equal("Atos", company.Name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidName_ShouldThrowArgumentException(string? name)
        {
            // Act
            Action act = () => new Company(name!);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }
    }
}