using Jobtri.Domain.Entities;

namespace Jobtri.UnitTests.Domain.Entities
{
    public sealed class JobTests
    {
        [Fact]
        public void Constructor_WithValidData_ShouldCreateJob()
        {
            // Arrange
            string title = "Junior .NET Developer";
            int companySourceId = 1;
            string sourceJobId = "ATOS-123";
            var jobUrl = new Uri("https://jobs.atos.net/job/123");
            string location = "Paris, France";
            DateTimeOffset datePosted = new DateTimeOffset(2026, 9, 18, 0, 0, 0, TimeSpan.Zero);

            var before = DateTimeOffset.UtcNow;

            // Act
            var job = new Job(title, companySourceId, sourceJobId, jobUrl, location, datePosted);

            var after = DateTimeOffset.UtcNow;

            // Assert
            Assert.Equal(title, job.Title);
            Assert.Equal(companySourceId, job.CompanySourceId);
            Assert.Equal(sourceJobId, job.SourceJobId);
            Assert.Equal(jobUrl, job.JobUrl);
            Assert.Equal(location, job.Location);
            Assert.Equal(datePosted, job.DatePosted);
            Assert.InRange(job.FirstSeenAt, before, after);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidTitle_ShouldThrowArgumentException(string? title)
        {
            // Arrange
            var jobUrl = new Uri("https://jobs.atos.net/job/123");

            // Act
            Action act = () => new Job(title!, 1, "ATOS-123", jobUrl);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Constructor_WithInvalidCompanySourceId_ShouldThrowArgumentException(int companySourceId)
        {
            // Arrange
            var jobUrl = new Uri("https://jobs.atos.net/job/123");

            // Act
            Action act = () => new Job("Junior .NET Developer", companySourceId, "ATOS-123", jobUrl);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidSourceJobId_ShouldThrowArgumentException(string? sourceJobId)
        {
            // Arrange
            var jobUrl = new Uri("https://jobs.atos.net/job/123");

            // Act
            Action act = () => new Job("Junior .NET Developer", 1, sourceJobId!, jobUrl);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void Constructor_WithNullJobUrl_ShouldThrowArgumentNullException()
        {
            // Act
            Action act = () => new Job("Junior .NET Developer", 1, "ATOS-123", null!);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }

        [Fact]
        public void Constructor_WithRelativeJobUrl_ShouldThrowArgumentException()
        {
            // Arrange
            var jobUrl = new Uri("/job/123", UriKind.Relative);

            // Act
            Action act = () => new Job("Junior .NET Developer", 1, "ATOS-123", jobUrl);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void Constructor_WithInvalidJobUrlScheme_ShouldThrowArgumentException()
        {
            // Arrange
            var jobUrl = new Uri("ftp://jobs.atos.net/job/123");

            // Act
            Action act = () => new Job("Junior .NET Developer", 1, "ATOS-123", jobUrl);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void Constructor_WithTitleContainingSpaces_ShouldTrimTitle()
        {
            // Arrange
            var jobUrl = new Uri("https://jobs.atos.net/job/123");

            // Act
            var job = new Job("   Junior .NET Developer   ", 1, "ATOS-123", jobUrl);

            // Assert
            Assert.Equal("Junior .NET Developer", job.Title);
        }

        [Fact]
        public void Constructor_WithSourceJobIdContainingSpaces_ShouldTrimSourceJobId()
        {
            // Arrange
            var jobUrl = new Uri("https://jobs.atos.net/job/123");

            // Act
            var job = new Job("Junior .NET Developer", 1, "   ATOS-123   ", jobUrl);

            // Assert
            Assert.Equal("ATOS-123", job.SourceJobId);
        }

        [Fact]
        public void Constructor_WithLocationContainingSpaces_ShouldTrimLocation()
        {
            // Arrange
            var jobUrl = new Uri("https://jobs.atos.net/job/123");

            // Act
            var job = new Job("Junior .NET Developer", 1, "ATOS-123", jobUrl, "   Paris, France   ");

            // Assert
            Assert.Equal("Paris, France", job.Location);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithEmptyLocation_ShouldSetLocationToNull(string? location)
        {
            // Arrange
            var jobUrl = new Uri("https://jobs.atos.net/job/123");

            // Act
            var job = new Job("Junior .NET Developer", 1, "ATOS-123", jobUrl, location);

            // Assert
            Assert.Null(job.Location);
        }
    }
}