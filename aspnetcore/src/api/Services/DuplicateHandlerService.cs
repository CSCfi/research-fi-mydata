
using System.Collections.Generic;
using api.Models.Common;
using api.Models.ProfileEditor;
using api.Models.ProfileEditor.Items;
using api.Models.Ttv;

namespace api.Services
{
    /*
     * DuplicateHandlerService
     */
    public class DuplicateHandlerService : IDuplicateHandlerService
    {
        private readonly List<string> typeCodes = new() { "A3", "A4", "B2", "B3", "D2", "D3", "E1" };
        private const string PeerReviewedCode = "1";
        private const string NotPeerReviewedCode = "0";
        private const string PeerReviewedFi = "Vertaisarvioitu";
        private const string NotPeerReviewedFi = "Ei-vertaisarvioitu";
        private const string PeerReviewedSv = "Kollegialt utvärderad";
        private const string NotPeerReviewedSv = "Inte kollegialt utvärderad";
        private const string PeerReviewedEn = "Peer-Reviewed";
        private const string NotPeerReviewedEn = "Non Peer-Reviewed";

        public DuplicateHandlerService()
        {
        }

        /*
         * Check if ProfileDataRaw contains ORCID publication.
         */
        public bool IsOrcidPublication(ProfileDataFromSql profileData)
        {
            return profileData.FactFieldValues_DimProfileOnlyPublicationId != -1 && profileData.DimFieldDisplaySettings_FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_PROFILE_ONLY;
        }

        /*
         * Make sure that publication year contains either a valid year or null.
         */
        public int? HandlePublicationYear(int? dimDateYear)
        {
            if (dimDateYear == null || dimDateYear < 1)
            {
                return null;
            }
            else {
                return dimDateYear;
            }
        }

        /*
         * When two publications have the same DOI, they will be considered as DIFFERENT publications if:
         *     - Virta publication code is A3, A4, B2, B3, D2, D3 or E1
         *     - AND the publication names differ
         */
        public bool HasSameDoiButIsDifferentPublication(string orcidPublicationName, ProfileEditorPublication publication)
        {
            return this.typeCodes.Contains(publication.PublicationTypeCode) && orcidPublicationName != publication.PublicationName;
        }

        /*
         * Add publication data source.
         */
        public List<ProfileEditorSource> AddDataSource(ProfileEditorPublication publication, ProfileEditorSource dataSource)
        {
            if (!publication.DataSources.Contains(dataSource))
            {
                publication.DataSources.Add(dataSource);
            }
            return publication.DataSources;
        }


        /*
         * Add publication to publication list.
         * Handle duplicates by matching DOI. Handle special case in DOI matching regarding Virta publication type codes.
         */
        public List<ProfileEditorPublication> AddPublicationToProfileEditorData(ProfileEditorSource dataSource, ProfileDataFromSql profileData, List<ProfileEditorPublication> publications)
        {
            // Loop existing publications and check for duplicates.
            foreach (ProfileEditorPublication publication in publications)
            {
                // Check duplicate publicationId.
                if (
                    (!IsOrcidPublication(profileData) && profileData.DimPublication_PublicationId != "" && profileData.DimPublication_PublicationId == publication.PublicationId) ||
                    (IsOrcidPublication(profileData) && profileData.DimProfileOnlyPublication_PublicationId != "" && profileData.DimProfileOnlyPublication_PublicationId == publication.PublicationId)
                )
                {
                    AddDataSource(publication, dataSource);
                    return publications;
                }

                // Check duplicate DOI.
                if (
                    IsOrcidPublication(profileData) &&
                    !string.IsNullOrWhiteSpace(profileData.DimProfileOnlyPublication_Doi) && !string.IsNullOrWhiteSpace(publication.Doi) &&
                    profileData.DimProfileOnlyPublication_Doi.ToLower() == publication.Doi.ToLower() &&
                    !HasSameDoiButIsDifferentPublication(profileData.DimProfileOnlyPublication_PublicationName, publication)
                )
                {
                    AddDataSource(publication, dataSource);
                    return publications;
                }
            }

            // Duplication not detected. Add publication to list of publications.
            if (!this.IsOrcidPublication(profileData))
            {
                // Add Virta publication
                publications.Add(
                    new ProfileEditorPublication()
                    {
                        AuthorsText = profileData.DimPublication_AuthorsText,
                        ConferenceName = profileData.DimPublication_ConferenceName,
                        DataSources = new List<ProfileEditorSource> { dataSource },
                        Doi = profileData.DimPublication_Doi,
                        JournalName = profileData.DimPublication_JournalName,
                        OpenAccess = !string.IsNullOrEmpty(profileData.DimPublication_OpenAccessCode) && profileData.DimPublication_OpenAccessCode == "1" ? 1 : 0,
                        ParentPublicationName = profileData.DimPublication_ParentPublicationName,
                        PeerReviewed = new ProfileEditorPublicationPeerReviewed()
                        {
                            Id = profileData.DimPublication_PeerReviewed != null && profileData.DimPublication_PeerReviewed.Value ? PeerReviewedCode : NotPeerReviewedCode,
                            NameFiPeerReviewed = profileData.DimPublication_PeerReviewed != null && profileData.DimPublication_PeerReviewed.Value ? PeerReviewedFi : NotPeerReviewedFi,
                            NameSvPeerReviewed = profileData.DimPublication_PeerReviewed != null && profileData.DimPublication_PeerReviewed.Value ? PeerReviewedSv : NotPeerReviewedSv,
                            NameEnPeerReviewed = profileData.DimPublication_PeerReviewed != null && profileData.DimPublication_PeerReviewed.Value ? PeerReviewedEn : NotPeerReviewedEn
                        },
                        PublicationId = profileData.DimPublication_PublicationId,
                        PublicationName = profileData.DimPublication_PublicationName,
                        PublicationTypeCode = profileData.DimPublication_PublicationTypeCode,
                        PublicationYear = HandlePublicationYear(profileData.DimPublication_PublicationYear),
                        SelfArchivedAddress = profileData.DimPublication_SelfArchivedAddress,
                        SelfArchivedCode = (profileData.DimPublication_SelfArchivedCode != null && (bool)profileData.DimPublication_SelfArchivedCode) ? "1" : "0",
                        itemMeta = new ProfileEditorItemMeta(
                            id: profileData.FactFieldValues_DimPublicationId,
                            type: Constants.FieldIdentifiers.ACTIVITY_PUBLICATION,
                            show: profileData.FactFieldValues_Show,
                            primaryValue: profileData.FactFieldValues_PrimaryValue
                        )
                    }
                );
            }
            else
            {
                // Add ORCID publication
                publications.Add(
                    new ProfileEditorPublication()
                    {
                        AuthorsText = "",
                        ConferenceName = "",
                        DataSources = new List<ProfileEditorSource> { dataSource },
                        Doi = profileData.DimProfileOnlyPublication_Doi,
                        JournalName = "",
                        OpenAccess = !string.IsNullOrEmpty(profileData.DimProfileOnlyPublication_OpenAccessCode) && profileData.DimProfileOnlyPublication_OpenAccessCode == "1" ? 1 : 0,
                        ParentPublicationName = "",
                        PeerReviewed = new ProfileEditorPublicationPeerReviewed()
                        {
                            Id = profileData.DimProfileOnlyPublication_PeerReviewed != null && profileData.DimProfileOnlyPublication_PeerReviewed.Value ? PeerReviewedCode : NotPeerReviewedCode,
                            NameFiPeerReviewed = profileData.DimProfileOnlyPublication_PeerReviewed != null && profileData.DimProfileOnlyPublication_PeerReviewed.Value ? PeerReviewedFi : NotPeerReviewedFi,
                            NameSvPeerReviewed = profileData.DimProfileOnlyPublication_PeerReviewed != null && profileData.DimProfileOnlyPublication_PeerReviewed.Value ? PeerReviewedSv : NotPeerReviewedSv,
                            NameEnPeerReviewed = profileData.DimProfileOnlyPublication_PeerReviewed != null && profileData.DimProfileOnlyPublication_PeerReviewed.Value ? PeerReviewedEn : NotPeerReviewedEn
                        },
                        PublicationId = profileData.DimProfileOnlyPublication_PublicationId,
                        PublicationName = profileData.DimProfileOnlyPublication_PublicationName,
                        PublicationTypeCode = "",
                        PublicationYear = HandlePublicationYear(profileData.DimProfileOnlyPublication_PublicationYear),
                        SelfArchivedAddress = "",
                        SelfArchivedCode = "",
                        itemMeta = new ProfileEditorItemMeta(
                            id: profileData.FactFieldValues_DimProfileOnlyPublicationId,
                            type: Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_PROFILE_ONLY,
                            show: profileData.FactFieldValues_Show,
                            primaryValue: profileData.FactFieldValues_PrimaryValue
                        )
                    }
                );
            }

            return publications;
        }
    }
}