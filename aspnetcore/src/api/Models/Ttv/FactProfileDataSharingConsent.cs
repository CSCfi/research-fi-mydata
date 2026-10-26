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
    public int DimDateIdConsentGranted { get; set; }

    /// <summary>
    /// https://iri.suomi.fi/model/researchfi_core/consent_withdrawn_date
    /// suostumuksen perumispäivämäärä
    /// </summary>
    public int? DimDateIdConsentWithdrawn { get; set; }

    /// <summary>
    /// https://iri.suomi.fi/model/researchfi_core/consent_expire_date
    /// suostumuksen viimeinen voimassaolo pvm (5 vuotta antopäivämäärästä)
    /// </summary>
    public int? DimDateIdConsentExpire { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Modified { get; set; }

    public virtual FactOrganizationGeneral ControllerOrganization { get; set; }

    public virtual DimDate DimDateIdConsentExpireNavigation { get; set; }

    public virtual DimDate DimDateIdConsentGrantedNavigation { get; set; }

    public virtual DimDate DimDateIdConsentWithdrawnNavigation { get; set; }

    public virtual DimUserProfile DimUserProfile { get; set; }
}
