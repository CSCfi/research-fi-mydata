using System.ComponentModel.DataAnnotations;

namespace api.Models.Ai
{
    public class TranslateRequest  {
        public TranslateRequest()
        {
        }

        [Required]
        public string TextToTranslate { get; set; }

        [Required]
        public string TargetLanguage { get; set; }
    }
}
