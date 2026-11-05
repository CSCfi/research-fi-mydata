using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using api.Models.Api;

namespace api.Services
{
    public interface IConsentService
    {
        Task<IReadOnlyList<ConsentTargetResponse>> GetTargetsAsync(string rorId, CancellationToken cancellationToken);
        Task<IReadOnlyList<ConsentResponse>> GetConsentsAsync(int userProfileId, CancellationToken cancellationToken);
        Task<ConsentGrantResult> GrantConsentAsync(int userProfileId, int consentTargetId, CancellationToken cancellationToken);
        Task<bool> WithdrawConsentAsync(int userProfileId, int consentTargetId, CancellationToken cancellationToken);
    }

    public enum ConsentGrantResult
    {
        Created,
        TargetNotFound,
        ConsentAlreadyExists
    }
}
