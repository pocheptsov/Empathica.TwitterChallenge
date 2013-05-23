using System.ComponentModel.DataAnnotations;

namespace Empathica.TwitterChallenge.Web.ViewModels
{
    public class StatusViewModel
    {
        [Required]
        [StringLength(140)]
        public string Status { get; set; }
    }
}