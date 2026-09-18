using JobHunter.Domain.Common;

namespace JobHunter.Domain.Entities
{
    public sealed class Job
    {
        public int Id { get; private set; }

        public string Title { get; private set; }

        public int CompanySourceId { get; private set; }

        // id of the job in the source system (ATS, job board, etc.)
        public string SourceJobId { get; private set; }

        public Uri JobUrl { get; private set; }

        public string? Location { get; private set; }

        public DateTimeOffset? DatePosted { get; private set; }

        public DateTimeOffset FirstSeenAt { get; private set; }

        public Job(string title, int companySourceId, string sourceJobId, Uri jobUrl, string? location = null, DateTimeOffset? datePosted = null)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Job title cannot be empty.", nameof(title));
            }

            if (companySourceId <= 0)
            {
                throw new ArgumentException("Company source ID must be a positive number.", nameof(companySourceId));
            }

            if (string.IsNullOrWhiteSpace(sourceJobId))
            {
                throw new ArgumentException("Source job ID cannot be empty.", nameof(sourceJobId));
            }


            Title = title.Trim();
            CompanySourceId = companySourceId;
            SourceJobId = sourceJobId.Trim();
            JobUrl = Guard.ValidHttpUrl(jobUrl, nameof(jobUrl));
            Location = string.IsNullOrWhiteSpace(location) ? null : location.Trim();
            DatePosted = datePosted;
            FirstSeenAt = DateTimeOffset.UtcNow;
        }
    }
}