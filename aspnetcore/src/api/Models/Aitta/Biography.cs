using System.ComponentModel.DataAnnotations;

namespace api.Models.Ai
{
    public class Biography  {
        public Biography()
        {
            Fi = "";
            En = "";
            Sv = "";
        }

        [Required]
        public string Fi { get; set; }
        [Required]
        public string En { get; set; }
        [Required]
        public string Sv { get; set; }
    }
}
