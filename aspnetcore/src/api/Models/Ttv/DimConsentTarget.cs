using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

/// <summary>
/// https://iri.suomi.fi/model/researchfi_core/concent_target
/// </summary>
public partial class DimConsentTarget
{
    public int Id { get; set; }

    /// <summary>
    /// https://iri.suomi.fi/model/researchfi_core/consent_controller
    /// </summary>
    public int ControllerOrganizationId { get; set; }

    /// <summary>
    /// https://iri.suomi.fi/model/researchfi_core/consent_purpose
    /// 
    /// http://uri.suomi.fi/codelist/research/profile_data_use_purpose
    /// dim_referencedata.code_sheme = &apos;profile_data_use_purpose&apos;
    /// </summary>
    public int PurposeCodeId { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Modified { get; set; }

    public virtual FactOrganizationGeneral ControllerOrganization { get; set; }

    public virtual DimReferencedatum PurposeCode { get; set; }
}
