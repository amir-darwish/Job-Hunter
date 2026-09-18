namespace JobHunter.Domain.Entities
{
    public sealed class Company
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public Uri? Website { get; private set; }
        public bool IsEnabled { get; private set; }

        public Company(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Company name cannot be null or empty.", nameof(name));
            }

            Name = name.Trim();
            IsEnabled = true;
        }

        public void SetWebsite(string website)
        {
            if (string.IsNullOrWhiteSpace(website))
            {
                throw new ArgumentException("Website cannot be null or empty.", nameof(website));
            }
            if (!Uri.TryCreate(website, UriKind.Absolute, out var uriResult) || (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException("Invalid website URL format.", nameof(website));
            }
            Website = uriResult;
        }
    }
}
