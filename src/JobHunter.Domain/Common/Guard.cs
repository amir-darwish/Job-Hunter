namespace JobHunter.Domain.Common
{
    public static class Guard
    {
        public static Uri ValidHttpUrl(Uri? value, string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (!value.IsAbsoluteUri ||
                (value.Scheme != Uri.UriSchemeHttp &&
                 value.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException("URL must be a valid HTTP or HTTPS absolute URI.", parameterName);
            }

            return value;
        }
    }
}