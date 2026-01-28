using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimAddress
{
    public int Id { get; set; }

    public int DimContactInformationId { get; set; }

    public string Street { get; set; }

    public string Premise { get; set; }

    public string PostOfficeBox { get; set; }

    public string PostalCode { get; set; }

    /// <summary>
    /// https://iri.suomi.fi/model/researchfi_core/locality
    /// </summary>
    public string LocalityFi { get; set; }

    /// <summary>
    /// https://iri.suomi.fi/model/researchfi_core/locality
    /// </summary>
    public string LocalitySv { get; set; }

    /// <summary>
    /// https://iri.suomi.fi/model/researchfi_core/locality
    /// </summary>
    public string LocalityEn { get; set; }

    public int CountryCode { get; set; }

    /// <summary>
    /// visiting_address
    /// </summary>
    public string AddressType { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public virtual DimReferencedatum CountryCodeNavigation { get; set; }

    public virtual DimContactInformation DimContactInformation { get; set; }
}
