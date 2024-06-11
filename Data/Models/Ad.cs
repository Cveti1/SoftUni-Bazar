using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static SoftUniBazar.Data.DataConstants;

namespace SoftUniBazar.Data.Models
{
    public class Ad
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(AdMaxName)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(DescriptionMaxName)]
        public string Description { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }


        [Required]
        public string OwnerId { get; set; } = null!;


        [Required]
        //  [ForeignKey(nameof(OwnerId))]
        public IdentityUser Owner { get; set; } = null!;


        [Required]
        public string ImageUrl { get; set; } = null!;


        [Required]
        public DateTime CreatedOn { get; set; }


        [Required]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;


    }
}
