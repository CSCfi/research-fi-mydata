namespace api.Models.Common
{
    public partial class FactContributionTableMinimalDTO
    {
        public int DimFundingDecisionId { get; set; }
        public int DimResearchActivityId { get; set; }
        public int DimResearchDatasetId { get; set; }
        public int DimPublicationId { get; set; }
    }
}
