using Jobtri.Domain.Common;
using Jobtri.Domain.Enums;

namespace Jobtri.Domain.Entities
{
    public sealed class CompanySource
    {
        public int Id { get; private set; }

        public int CompanyId { get; private set; }

        public Uri CareersUrl { get; private set; }

        public enAtsType Ats { get; private set; }

        public string? AtsIdentifier { get; private set; }

        public bool IsEnabled { get; private set; }

        public CompanySource(int companyId, Uri careersUrl, enAtsType ats, string? atsIdentifier = null)
        {
            if (companyId <= 0)
            {
                throw new ArgumentException("Company ID must be a positive number.", nameof(companyId));
            }
            CompanyId = companyId;
            CareersUrl = Guard.ValidHttpUrl(careersUrl, nameof(careersUrl));
            Ats = ats;
            AtsIdentifier = string.IsNullOrWhiteSpace(atsIdentifier) ? null : atsIdentifier.Trim();
            IsEnabled = true;
        }

    }
}