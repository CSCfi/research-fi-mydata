using System.ComponentModel.DataAnnotations;

namespace api.Models.Ai
{
    public class Biography  {
        public Biography()
        {
        }

        [Required(AllowEmptyStrings = true)]
        public string Fi { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string En { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Sv { get; set; }
    }
}
