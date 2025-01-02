using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

/// <summary>
/// https://iri.suomi.fi/model/researchfi_core_project/
/// Projektin kuvailutiedot ajassa
/// https://iri.suomi.fi/model/researchfi_core_project/cl_project_descriptive_in_time
/// </summary>
public partial class DimDescriptiveItem
{
    public int Id { get; set; }

    /// <summary>
    /// https://iri.suomi.fi/model/researchfi_core_project/
    /// Projektin kuvailutiedot ajassa
    /// https://iri.suomi.fi/model/researchfi_core_project/cl_project_descriptive_in_time
    /// - liittyy projektiin
    /// </summary>
    public int DimResearchProjectId { get; set; }

    /// <summary>
    /// https://iri.suomi.fi/model/researchfi_core_project/
    /// Projektin kuvailutiedot ajassa
    /// https://iri.suomi.fi/model/researchfi_core_project/cl_project_descriptive_in_time
    /// * alkamispäivämäärä
    /// </summary>
    public int DimStartDate { get; set; }

    /// <summary>
    /// https://iri.suomi.fi/model/researchfi_core_project/
    /// Projektin kuvailutiedot ajassa
    /// https://iri.suomi.fi/model/researchfi_core_project/cl_project_descriptive_in_time
    /// * päättymispäivämäärä
    /// </summary>
    public int? DimEndDate { get; set; }

    /// <summary>
    /// https://iri.suomi.fi/model/researchfi_core_project/
    /// Projektin kuvailutiedot ajassa
    /// https://iri.suomi.fi/model/researchfi_core_project/cl_project_descriptive_in_time
    /// * kuvailutiedon sisältö
    /// </summary>
    public string DescriptiveItem { get; set; }

    /// <summary>
    /// https://iri.suomi.fi/model/researchfi_core_project/
    /// https://iri.suomi.fi/model/researchfi_core_project/cl_project_descriptive_type
    /// - description
    /// - name
    /// - goal
    /// - outcome_effect
    /// - abberviation
    /// </summary>
    public string DescriptiveItemType { get; set; }

    /// <summary>
    /// fi, en, sv, NULL
    /// </summary>
    public string DescriptiveItemLanguage { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public int DimRegisteredDataSourceId { get; set; }

    public virtual DimDate DimEndDateNavigation { get; set; }

    public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }

    public virtual DimResearchProject DimResearchProject { get; set; }

    public virtual DimDate DimStartDateNavigation { get; set; }
}
