using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimResearchProject
{
    public int Id { get; set; }

    /// <summary>
    /// Hanke - vastuuorganisaatio
    /// </summary>
    public int ResponsibleOrganization { get; set; }

    /// <summary>
    /// Hanke - nimi
    /// </summary>
    public string NameFi { get; set; }

    public string NameEn { get; set; }

    public string NameSv { get; set; }

    /// <summary>
    /// Hanke - lyhenne
    /// </summary>
    public string AbbrevationFi { get; set; }

    public string AbbrevationEn { get; set; }

    public string AbbrevationSv { get; set; }

    /// <summary>
    /// Hanke - tiivistelmä
    /// </summary>
    public string SummaryFi { get; set; }

    public string SummaryEn { get; set; }

    public string SummarySv { get; set; }

    /// <summary>
    /// Hanke - lisätieto
    /// </summary>
    public string AdditionalInfromationFi { get; set; }

    public string AdditionalInformationEn { get; set; }

    public string AddtitionalInformationSv { get; set; }

    /// <summary>
    /// Hanke - alkamispäivämäärä
    /// </summary>
    public int? StartDate { get; set; }

    /// <summary>
    /// Hanke - päättymispäivämäärä
    /// </summary>
    public int? EndDate { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public int DimRegisteredDataSourceId { get; set; }

    public int? ResponsiblePerson { get; set; }

    public string GoalsFi { get; set; }

    public string GoalsEn { get; set; }

    public string GoalsSv { get; set; }

    public string OutcomeEffectFi { get; set; }

    public string OutcomeEffectEn { get; set; }

    public string OutcomeEffectSv { get; set; }

    public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }

    public virtual DimDate EndDateNavigation { get; set; }

    public virtual DimOrganization ResponsibleOrganizationNavigation { get; set; }

    public virtual DimName ResponsiblePersonNavigation { get; set; }

    public virtual DimDate StartDateNavigation { get; set; }
}
