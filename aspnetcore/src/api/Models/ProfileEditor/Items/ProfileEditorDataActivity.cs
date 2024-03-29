﻿using System.Collections.Generic;

namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorDataActivity
    {
        public ProfileEditorDataActivity()
        {
            affiliations = new List<ProfileEditorAffiliation>();
            educations = new List<ProfileEditorEducation>();
            publications = new List<ProfileEditorPublication>();
            fundingDecisions = new List<ProfileEditorFundingDecision>();
            researchDatasets = new List<ProfileEditorResearchDataset>();
            activitiesAndRewards = new List<ProfileEditorActivityAndReward>();
        }

        public List<ProfileEditorAffiliation> affiliations { get; set; }
        public List<ProfileEditorEducation> educations { get; set; }
        public List<ProfileEditorPublication> publications { get; set; }
        public List<ProfileEditorFundingDecision> fundingDecisions { get; set; }
        public List<ProfileEditorResearchDataset> researchDatasets { get; set; }
        public List<ProfileEditorActivityAndReward> activitiesAndRewards { get; set; }
    }
}
