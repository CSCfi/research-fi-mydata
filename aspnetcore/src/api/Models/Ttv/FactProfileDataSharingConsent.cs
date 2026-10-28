using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

/// <summary>
/// https://iri.suomi.fi/model/researchfi_core/granted_permission
/// </summary>
public partial class FactProfileDataSharingConsent
{
    /// <summary>
    /// https://iri.suomi.fi/model/researchfi_core/consenting_user_profile
    /// </summary>
    public int DimUserProfileId { get; set; }

    /// <summary>
    /// https://iri.suomi.fi/model/researchfi_core/consent_controller
    /// </summary>
    public int ControllerOrganizationId { get; set; }

    /// <summary>
    /// https://iri.suomi.fi/model/researchfi_core/consent_granted_date
    /// suostumuksen antamispäivämäärä
    /// </summary>
    public DateTime DateConsentGranted { get; set; }

    /// <summary>
    /// https://iri.suomi.fi/model/researchfi_core/consent_withdrawn_date
    /// suostumuksen perumispäivämäärä
    /// </summary>
    public DateTime? DateConsentWithdrawn { get; set; }

    /// <summary>
    /// https://iri.suomi.fi/model/researchfi_core/consent_expire_date
    /// suostumuksen viimeinen voimassaolo pvm (5 vuotta antopäivämäärästä)
    /// </summary>
    public DateTime? DateConsentExpire { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Modified { get; set; }

    public virtual FactOrganizationGeneral ControllerOrganization { get; set; }

    public virtual DimUserProfile DimUserProfile { get; set; }
}
