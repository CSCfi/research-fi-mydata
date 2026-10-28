using System;
using System.Collections.Generic;

namespace api.Models.Api
{
    /// <summary>
    /// A consent target and the organization and purpose it represents.
    /// </summary>
    public sealed record ConsentTargetResponse(
        int ConsentTargetId,
        string OrganizationNameFi,
        string OrganizationNameSv,
        string OrganizationNameEn,
        string RorId,
        string PurposeNameFi,
        string PurposeNameEn,
        string PurposeNameSv);

    /// <summary>
    /// An active consent granted to a controller organization.
    /// </summary>
    public sealed record ConsentResponse(
        int ConsentTargetId,
        string OrganizationNameFi,
        string OrganizationNameSv,
        string OrganizationNameEn,
        string RorId,
        string PurposeNameFi,
        string PurposeNameEn,
        string PurposeNameSv,
        DateTime ConsentGrantedDate,
        DateTime? ConsentExpireDate);

    /// <summary>
    /// Request to grant consent to a consent target.
    /// </summary>
    public sealed record CreateConsentRequest
    {
        public int ConsentTargetId { get; init; }
    }

    public sealed class ApiResponseConsentTargetsGet : ApiResponse
    {
        public ApiResponseConsentTargetsGet(bool success, string reason, IReadOnlyList<ConsentTargetResponse> data)
            : base(success, reason)
        {
            Data = data;
        }

        public IReadOnlyList<ConsentTargetResponse> Data { get; }
    }

    public sealed class ApiResponseConsentGet : ApiResponse
    {
        public ApiResponseConsentGet(bool success, string reason, IReadOnlyList<ConsentResponse> data)
            : base(success, reason)
        {
            Data = data;
        }

        public IReadOnlyList<ConsentResponse> Data { get; }
    }
}
