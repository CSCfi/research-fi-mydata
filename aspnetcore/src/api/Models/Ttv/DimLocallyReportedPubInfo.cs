using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimLocallyReportedPubInfo
{
    public int Id { get; set; }

    public int DimPublicationid { get; set; }

    public string SelfArchivedType { get; set; }

    public string SelfArchivedUrl { get; set; }

    public string SelfArchivedVersionCode { get; set; }

    public string SelfArchivedLicenseCode { get; set; }

    public DateTime? SelfArchivedEmbargoDate { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public virtual DimPublication DimPublication { get; set; }
}
