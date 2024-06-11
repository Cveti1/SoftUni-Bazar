using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SoftUniBazar.Models.Category;
using static SoftUniBazar.Data.DataConstants;

namespace SoftUniBazar.Models.Ad
{
    public class AdFormModel
    {
        [Required]
        [StringLength(AdMaxName, MinimumLength = AdMinName,
            ErrorMessage = "Ad name must be between 5 and 25 characters.")]
        public string Name { get; set; } = null!;


        [Required]
        [StringLength(DescriptionMaxName, MinimumLength = DescriptionMinName,
            ErrorMessage = "Description must be between 15 and 250 characters.")]

        public string Description { get; set; } = null!;


        [Required] [DisplayName("Image Url")] public string ImageUrl { get; set; } = null!;


        [Required] public decimal Price { get; set; }


        [Required] public int CategoryId { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; }
            = new List<CategoryViewModel>();





    }
}



