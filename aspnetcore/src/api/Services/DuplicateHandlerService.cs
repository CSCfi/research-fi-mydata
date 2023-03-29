
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
            return this.typeCodes.Contains(publication.TypeCode) && orcidPublicationName != publication.PublicationName;
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
                    profileData.DimProfileOnlyPublication_Doi != "" &&
                    profileData.DimProfileOnlyPublication_Doi == publication.Doi &&
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
                        PublicationId = profileData.DimPublication_PublicationId,
                        PublicationName = profileData.DimPublication_PublicationName,
                        PublicationYear = HandlePublicationYear(profileData.DimPublication_PublicationYear),
                        Doi = profileData.DimPublication_Doi,
                        AuthorsText = profileData.DimPublication_AuthorsText,
                        TypeCode = profileData.DimPublication_PublicationTypeCode,
                        JournalName = profileData.DimPublication_JournalName,
                        ConferenceName = profileData.DimPublication_ConferenceName,
                        ParentPublicationName = profileData.DimPublication_ParentPublicationName,
                        itemMeta = new ProfileEditorItemMeta()
                        {
                            Id = profileData.FactFieldValues_DimPublicationId,
                            Type = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION,
                            Show = profileData.FactFieldValues_Show,
                            PrimaryValue = profileData.FactFieldValues_PrimaryValue
                        },
                        DataSources = new List<ProfileEditorSource> { dataSource }
                    }
                );
            }
            else
            {
                // Add ORCID publication
                publications.Add(
                    new ProfileEditorPublication()
                    {
                        PublicationId = profileData.DimProfileOnlyPublication_PublicationId,
                        PublicationName = profileData.DimProfileOnlyPublication_PublicationName,
                        PublicationYear = HandlePublicationYear(profileData.DimProfileOnlyPublication_PublicationYear),
                        Doi = profileData.DimProfileOnlyPublication_Doi,
                        AuthorsText = "",
                        TypeCode = "",
                        JournalName = "",
                        ConferenceName = "",
                        ParentPublicationName = "",
                        itemMeta = new ProfileEditorItemMeta()
                        {
                            Id = profileData.FactFieldValues_DimProfileOnlyPublicationId,
                            Type = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_PROFILE_ONLY,
                            Show = profileData.FactFieldValues_Show,
                            PrimaryValue = profileData.FactFieldValues_PrimaryValue
                        },
                        DataSources = new List<ProfileEditorSource> { dataSource }
                    }
                );
            }

            return publications;
        }
    }
}