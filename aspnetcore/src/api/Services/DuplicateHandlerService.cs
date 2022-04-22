
using System.Collections.Generic;
using api.Models;
using api.Models.ProfileEditor;
using api.Models.Ttv;

namespace api.Services
{
    /*
     * DuplicateHandlerService
     */
    public class DuplicateHandlerService
    {
        private readonly List<string> typeCodes = new List<string> { "A3", "A4", "B2", "B3", "D2", "D3", "E1" };

        public DuplicateHandlerService()
        {
        }

        /*
         * Check if FactFieldValue contains ORCID publication.
         */
        public bool IsOrcidPublication(FactFieldValue ffv)
        {
            return ffv.DimOrcidPublication != null;
        }

        /*
         * When two publications have the same DOI, they will be considered as DIFFERENT publications if:
         *     - Virta publication code is A3, A4, B2, B3, D2, D3 or E1
         *     - AND the publication names differ
         */
        public bool HasSameDoiButIsDifferentPublication(DimOrcidPublication orcidPublication, ProfileEditorPublicationExperimental publication)
        {
            return this.typeCodes.Contains(publication.TypeCode) && orcidPublication.PublicationName != publication.PublicationName;
        }

        /*
         * Add publication data source.
         */
        public List<ProfileEditorSource> AddDataSource(ProfileEditorPublicationExperimental publication, ProfileEditorSource dataSource)
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
        public List<ProfileEditorPublicationExperimental> AddPublicationToProfileEditorData(ProfileEditorSource dataSource, FactFieldValue ffv, List<ProfileEditorPublicationExperimental> publications)
        {
            // Loop existing publications and check for duplicates.
            foreach (ProfileEditorPublicationExperimental publication in publications)
            {
                // Check duplicate publicationId.
                if (!this.IsOrcidPublication(ffv) && ffv.DimPublication.PublicationId == publication.PublicationId || this.IsOrcidPublication(ffv) && ffv.DimOrcidPublication.PublicationId == publication.PublicationId)
                {
                    this.AddDataSource(publication, dataSource);
                    return publications;
                }

                // Check duplicate DOI.
                if (this.IsOrcidPublication(ffv) && ffv.DimOrcidPublication.DoiHandle != "" && ffv.DimOrcidPublication.DoiHandle == publication.Doi && !this.HasSameDoiButIsDifferentPublication(ffv.DimOrcidPublication, publication))
                {
                    this.AddDataSource(publication, dataSource);
                    return publications;
                }
            }

            // Duplication not detected. Add publication to list of publications.
            if (!this.IsOrcidPublication(ffv))
            {
                // Add Virta publication
                publications.Add(
                    new ProfileEditorPublicationExperimental()
                    {
                        PublicationId = ffv.DimPublication.PublicationId,
                        PublicationName = ffv.DimPublication.PublicationName,
                        PublicationYear = ffv.DimPublication.PublicationYear,
                        Doi = ffv.DimPublication.Doi,
                        TypeCode = ffv.DimPublication.PublicationTypeCode,
                        itemMeta = new ProfileEditorItemMeta()
                        {
                            Id = ffv.DimPublication.Id,
                            Type = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION,
                            Show = ffv.Show,
                            PrimaryValue = ffv.PrimaryValue
                        },
                        DataSources = new List<ProfileEditorSource> { dataSource }
                    }
                );
            }
            else if (this.IsOrcidPublication(ffv))
            {
                // Add ORCID publication
                publications.Add(
                    new ProfileEditorPublicationExperimental()
                    {
                        PublicationId = ffv.DimOrcidPublication.PublicationId,
                        PublicationName = ffv.DimOrcidPublication.PublicationName,
                        PublicationYear = ffv.DimOrcidPublication.PublicationYear,
                        Doi = ffv.DimOrcidPublication.DoiHandle, // TODO: ORCID doi field name?
                        TypeCode = "",
                        itemMeta = new ProfileEditorItemMeta()
                        {
                            Id = ffv.DimOrcidPublicationId,
                            Type = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_ORCID,
                            Show = ffv.Show,
                            PrimaryValue = ffv.PrimaryValue
                        },
                        DataSources = new List<ProfileEditorSource> { dataSource }
                    }
                );
            }

            return publications;
        }
    }
}