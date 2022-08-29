
using System.Collections.Generic;
using api.Models;
using api.Models.ProfileEditor;
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
        public bool IsOrcidPublication(ProfileDataRaw p)
        {
            return p.FactFieldValues_DimOrcidPublicationId != -1 && p.DimFieldDisplaySettings_FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_ORCID;
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
        public List<ProfileEditorPublication> AddPublicationToProfileEditorData(ProfileEditorSource dataSource, ProfileDataRaw profileDataRaw, List<ProfileEditorPublication> publications)
        {
            // Loop existing publications and check for duplicates.
            foreach (ProfileEditorPublication publication in publications)
            {
                // Check duplicate publicationId.
                if (
                    (!IsOrcidPublication(profileDataRaw) && profileDataRaw.DimPublication_PublicationId != "" && profileDataRaw.DimPublication_PublicationId == publication.PublicationId) ||
                    (IsOrcidPublication(profileDataRaw) && profileDataRaw.DimOrcidPublication_PublicationId != "" && profileDataRaw.DimOrcidPublication_PublicationId == publication.PublicationId)
                )
                {
                    AddDataSource(publication, dataSource);
                    return publications;
                }

                // Check duplicate DOI.
                if (
                    IsOrcidPublication(profileDataRaw) &&
                    profileDataRaw.DimOrcidPublication_Doi != "" &&
                    profileDataRaw.DimOrcidPublication_Doi == publication.Doi &&
                    !HasSameDoiButIsDifferentPublication(profileDataRaw.DimOrcidPublication_PublicationName, publication)
                )
                {
                    AddDataSource(publication, dataSource);
                    return publications;
                }
            }

            // Duplication not detected. Add publication to list of publications.
            if (!this.IsOrcidPublication(profileDataRaw))
            {
                // Add Virta publication
                publications.Add(
                    new ProfileEditorPublication()
                    {
                        PublicationId = profileDataRaw.DimPublication_PublicationId,
                        PublicationName = profileDataRaw.DimPublication_PublicationName,
                        PublicationYear = profileDataRaw.DimPublication_PublicationYear,
                        Doi = profileDataRaw.DimPublication_Doi,
                        TypeCode = profileDataRaw.DimPublication_PublicationTypeCode,
                        itemMeta = new ProfileEditorItemMeta()
                        {
                            Id = profileDataRaw.FactFieldValues_DimPublicationId,
                            Type = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION,
                            Show = profileDataRaw.FactFieldValues_Show,
                            PrimaryValue = profileDataRaw.FactFieldValues_PrimaryValue
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
                        PublicationId = profileDataRaw.DimOrcidPublication_PublicationId,
                        PublicationName = profileDataRaw.DimOrcidPublication_PublicationName,
                        PublicationYear = profileDataRaw.DimOrcidPublication_PublicationYear,
                        Doi = profileDataRaw.DimOrcidPublication_Doi,
                        TypeCode = "",
                        itemMeta = new ProfileEditorItemMeta()
                        {
                            Id = profileDataRaw.FactFieldValues_DimOrcidPublicationId,
                            Type = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_ORCID,
                            Show = profileDataRaw.FactFieldValues_Show,
                            PrimaryValue = profileDataRaw.FactFieldValues_PrimaryValue
                        },
                        DataSources = new List<ProfileEditorSource> { dataSource }
                    }
                );
            }

            return publications;
        }
    }
}