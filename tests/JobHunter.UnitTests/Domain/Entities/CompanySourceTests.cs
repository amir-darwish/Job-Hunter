using JobHunter.Domain.Entities;
using JobHunter.Domain.Enums;

namespace JobHunter.UnitTests.Domain.Entities
{
    public sealed class CompanySourceTests
    {
        [Fact]
        public void Constructor_WithValidData_ShouldCreateCompanySource()
        {
            // Arrange
            int companyId = 1;
            var careersUrl = new Uri("https://jobs.atos.net");
            enAtsType ats = enAtsType.SuccessFactors;

            // Act
            var source = new CompanySource(
                companyId,
                careersUrl,
                ats);

            // Assert
            Assert.Equal(companyId, source.CompanyId);
            Assert.Equal(careersUrl, source.CareersUrl);
            Assert.Equal(ats, source.Ats);
            Assert.True(source.IsEnabled);
            Assert.Null(source.AtsIdentifier);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Constructor_WithInvalidCompanyId_ShouldThrowArgumentException(int companyId)
        {
            // Arrange
            var careersUrl = new Uri("https://jobs.atos.net");
            enAtsType ats = enAtsType.SuccessFactors;

            // Act
            Action act = () => new CompanySource(
                companyId,
                careersUrl,
                ats);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void Constructor_WithRelativeCareersUrl_ShouldThrowArgumentException()
        {
            // Arrange
            int companyId = 1;

            var careersUrl = new Uri(
                "/jobs",
                UriKind.Relative);

            enAtsType ats = enAtsType.SuccessFactors;

            // Act
            Action act = () => new CompanySource(
                companyId,
                careersUrl,
                ats);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void Constructor_WithNullCareersUrl_ShouldThrowArgumentNullException()
        {
            // Arrange
            int companyId = 1;
            enAtsType ats = enAtsType.SuccessFactors;

            // Act
            Action act = () => new CompanySource(
                companyId,
                null!,
                ats);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }


    }
}