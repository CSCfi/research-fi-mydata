namespace api.Services
{
    public interface IDataSourceHelperService
    {
        int DimRegisteredDataSourceId_ORCID { get; set; }
        string DimRegisteredDataSourceName_ORCID { get; set; }
        int DimOrganizationId_ORCID { get; set; }
        string DimOrganizationNameFi_ORCID { get; set; }
        string DimOrganizationNameEn_ORCID { get; set; }
        string DimOrganizationNameSv_ORCID { get; set; }
        int DimRegisteredDataSourceId_TTV { get; set; }
        string DimRegisteredDataSourceName_TTV { get; set; }
        int DimOrganizationId_TTV { get; set; }
        string DimOrganizationNameFi_TTV { get; set; }
        string DimOrganizationNameEn_TTV { get; set; }
        string DimOrganizationNameSv_TTV { get; set; }
        int DimPurposeId_TTV { get; set; }
    }
}