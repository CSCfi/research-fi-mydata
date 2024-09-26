namespace api.Models.Common
{
    public partial class FactContributionTableMinimalDTO
    {
        public int DimResearchActivityId { get; set; }
        public int DimResearchDatasetId { get; set; }
        public int DimPublicationId { get; set; }
        public int CoPublication_Parent_DimPublicationId { get; set; }
    }
}
