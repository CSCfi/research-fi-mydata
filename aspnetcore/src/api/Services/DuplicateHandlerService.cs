
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
            return p.FactFieldValues_DimOrcidPublicationId != -1;
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
        public List<ProfileEditorPublication> AddPublicationToProfileEditorData(ProfileEditorSource dataSource, ProfileDataRaw p, List<ProfileEditorPublication> publications)
        {
            // Loop existing publications and check for duplicates.
            foreach (ProfileEditorPublication publication in publications)
            {
                // Check duplicate publicationId.
                if (
                    (!IsOrcidPublication(p) && p.DimPublication_PublicationId == publication.PublicationId) || (IsOrcidPublication(p) &&
                    p.DimOrcidPublication_PublicationId == publication.PublicationId)
                )
                {
                    this.AddDataSource(publication, dataSource);
                    return publications;
                }

                // Check duplicate DOI.
                if (
                    IsOrcidPublication(p) &&
                    p.DimOrcidPublication_Doi != "" &&
                    p.DimOrcidPublication_Doi == publication.Doi &&
                    !HasSameDoiButIsDifferentPublication(p.DimOrcidPublication_PublicationName, publication)
                )
                {
                    this.AddDataSource(publication, dataSource);
                    return publications;
                }
            }

            // Duplication not detected. Add publication to list of publications.
            if (!this.IsOrcidPublication(p))
            {
                // Add Virta publication
                publications.Add(
                    new ProfileEditorPublication()
                    {
                        PublicationId = p.DimPublication_PublicationId,
                        PublicationName = p.DimPublication_PublicationName,
                        PublicationYear = p.DimPublication_PublicationYear,
                        Doi = p.DimPublication_Doi,
                        TypeCode = p.DimPublication_PublicationTypeCode,
                        itemMeta = new ProfileEditorItemMeta()
                        {
                            Id = p.FactFieldValues_DimPublicationId,
                            Type = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION,
                            Show = p.FactFieldValues_Show,
                            PrimaryValue = p.FactFieldValues_PrimaryValue
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
                        PublicationId = p.DimOrcidPublication_PublicationId,
                        PublicationName = p.DimOrcidPublication_PublicationName,
                        PublicationYear = p.DimOrcidPublication_PublicationYear,
                        Doi = p.DimOrcidPublication_Doi,
                        TypeCode = "",
                        itemMeta = new ProfileEditorItemMeta()
                        {
                            Id = p.FactFieldValues_DimOrcidPublicationId,
                            Type = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_ORCID,
                            Show = p.FactFieldValues_Show,
                            PrimaryValue = p.FactFieldValues_PrimaryValue
                        },
                        DataSources = new List<ProfileEditorSource> { dataSource }
                    }
                );
            }

            return publications;
        }
    }
}