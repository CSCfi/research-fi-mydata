using System;

namespace api.Models.Orcid
{
    public class OrcidTokens {
        public OrcidTokens() { }

        public OrcidTokens(string? accessToken, string? refreshToken, long? expiresSeconds, DateTime? expiresDatetime, string? scope)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            ExpiresSeconds = expiresSeconds;
            ExpiresDatetime = expiresDatetime;
            Scope = scope;

            // Calculate ExpiresDatetime from ExpiresSeconds
            if (ExpiresSeconds != null && ExpiresDatetime == null)
            {
                DateTimeOffset dateTimeOffSet = DateTimeOffset.FromUnixTimeSeconds((long)ExpiresSeconds);
                ExpiresDatetime = dateTimeOffSet.DateTime;
            }
        }

        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public long? ExpiresSeconds { get; set; }
        public DateTime? ExpiresDatetime { get; set; }
        public string? Scope { get; set; }
    }
}