using System;
using System.Collections.Generic;

namespace api.Models.Ai
{
    public partial class AittaModel
    {
        public AittaModel()
        {
        }

        public List<AittaGrantedFunding> UserParticipatedGrantedFunding { get; set; } = new List<AittaGrantedFunding>();
        public List<AittaResearchDataset> UserParticipatedDataset { get; set; } = new List<AittaResearchDataset>();
        public List<string> ResearcherDescription { get; set; } = new List<string>();
        public List<AittaEducation> HasCompleted { get; set; } = new List<AittaEducation>();
        public List<AittaPublication> UserParticipatedPublication { get; set; } = new List<AittaPublication>();
        public string PersonName { get; set; } = string.Empty;
        public List<AittaResearchActivity> UserParticipatedActivity { get; set; } = new List<AittaResearchActivity>();
        public List<AittaAffiliation> HasAffiliation { get; set; } = new List<AittaAffiliation>();
    }

    public class AittaKeyword
    {
        public string? KeywordContent { get; set; } = null;
    }

    /*
    public class AittaDescriptiveItem
    {
        public string? DescriptiveContent { get; set; } = null;
    }
    */

    public class AittaReferenceData
    {
        public string? CodeName { get; set; } = null;
        public string? CodeValue { get; set; } = null;
    }

    public class AittaOrganization
    {
        public AittaOrganization? IsPartOfOrganization { get; set; } = null;
        public string? OrganizationName { get; set; } = null;
    }

    public class AittaPublication
    {
        public List<string>? FieldsOfScience { get; set; } = null;
        public int? Year { get; set; } = null;
        public string Name { get; set; } = string.Empty;
        public string? Type { get; set; } = null;
        public string? TargetAudience { get; set; } = null;
        public List<string>? Keywords { get; set; } = null;
        public string? Abstract { get; set; } = null;
    }

    public class AittaResearchActivity
    {
        public List<string>? DescriptionLangVariant { get; set; } = null;
        public List<AittaReferenceData>? ActivityRole { get; set; } = null;
        public AittaDate? EndsOn { get; set; } = null;
        public List<AittaReferenceData>? ActivityType { get; set; } = null;
        public AittaDate? StartsOn { get; set; } = null;
        public List<string>? ActivityTitle { get; set; } = null;
    }
    public class AittaResearchDataset
    {
        public string? DatasetDescription { get; set; } = null;
        public List<string>? FieldsOfScience { get; set; } = null;
        public List<string>? Theme { get; set; } = null;
        public List<string>? Keywords { get; set; } = null;
        public string? DatasetTitle { get; set; } = null;
        public DateTime? DatasetCreationDate { get; set; } = null;
    }

    public class AittaGrantedFunding
    {
        public List<string>? Theme { get; set; } = null;
        public AittaDate? EndsOn { get; set; } = null;
        public List<string>? FieldsOfScience { get; set; } = null;
        public string? Description { get; set; } = null;
        public string? TypeOfFunding { get; set; } = null;
        public List<string>? Keywords { get; set; } = null;
        public List<string>? FieldsOfResearch { get; set; } = null;
        public AittaDate? StartsOn { get; set; } = null;
        public AittaOrganization? HasFunder { get; set; } = null;
        public string? Name { get; set; } = null;
    }

    public class AittaEducation_DegreeGrantingInstitution
    {
        public string? Description { get; set; } = null;
        public string? Title { get; set; } = null;
    }

    public class AittaEducation
    {
        public string? DegreeGrantingInstitution { get; set; } = null;
        public string? EducationName { get; set; } = null;
    }

    public class AittaAffiliation
    {
        public string? AffiliationType { get; set; } = null;
        public string? PositionTitle { get; set; } = null;
        public AittaOrganization? Organization { get; set; } = null;
        public AittaDate? StartsOn { get; set; } = null;
        public AittaDate? EndsOn { get; set; } = null;
    }

    public class AittaDate
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
    }
}
