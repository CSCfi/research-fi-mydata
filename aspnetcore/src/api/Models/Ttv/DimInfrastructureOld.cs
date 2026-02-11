using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimInfrastructureOld
{
    public int Id { get; set; }

    public int NextInfastructureId { get; set; }

    public string NameFi { get; set; }

    public string NameSv { get; set; }

    public string NameEn { get; set; }

    public string DescriptionFi { get; set; }

    public string DescriptionSv { get; set; }

    public string DescriptionEn { get; set; }

    public int? StartYear { get; set; }

    public int? EndYear { get; set; }

    public string Acronym { get; set; }

    public bool FinlandRoadmap { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public string Urn { get; set; }

    public string ScientificDescriptionFi { get; set; }

    public string ScientificDescriptionSv { get; set; }

    public string ScientificDescriptionEn { get; set; }
}
