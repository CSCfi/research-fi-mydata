namespace api.Models.Common
{
    public partial class PublicationDoiMatchingDTO
    {
        public string DimProfileOnlyPublication_Doi { get; set; }
        public string DimProfileOnlyPublication_PublicationName { get; set; }
        public int DimPublication_Id { get; set; }
        public string DimPublication_PublicationId { get; set; }
        public string DimPublication_PublicationName { get; set; }
        public string DimPublication_Doi { get; set; }
        public string DimPublication_TypeCode { get; set; }
        public bool FactFieldValues_Show { get; set; }
    }
}
