using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace api.Models.Api
{
    /// <summary>
    /// A consent target and the organization and purpose it represents.
    /// </summary>
    public sealed record ConsentTargetResponse(
        int ConsentTargetId,
        ConsentControllerResponse ConsentController,
        IReadOnlyList<ConsentPurposeResponse> ConsentPurpose,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] ConsentDetailsResponse Consent);

    /// <summary>
    /// The organization responsible for a consent target.
    /// </summary>
    public sealed record ConsentControllerResponse(
        ConsentLanguageVariant OrganizationName,
        [property: JsonPropertyName("ROR")] string Ror);

    /// <summary>
    /// A consent purpose and its localized label.
    /// </summary>
    public sealed record ConsentPurposeResponse(ConsentLanguageVariant CodeLabel, string CodeValue);

    /// <summary>
    /// Localized labels for a consent target.
    /// </summary>
    public sealed record ConsentLanguageVariant(
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string Fi,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string Sv,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string En);

    /// <summary>
    /// Dates associated with a consent.
    /// </summary>
    public sealed record ConsentDetailsResponse(
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] DateTime? ConsentGrantedDate,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] DateTime? ConsentWithdrawnDate,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] DateTime? ConsentExpireDate);

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
        public ApiResponseConsentGet(bool success, string reason, IReadOnlyList<ConsentTargetResponse> data)
            : base(success, reason)
        {
            Data = data;
        }

        public IReadOnlyList<ConsentTargetResponse> Data { get; }
    }
}
