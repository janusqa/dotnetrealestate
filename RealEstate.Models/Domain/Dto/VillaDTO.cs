// DTOs. Used to access our models. 
// We do not access models directly in our applications we
// access DTOS. This is so we can scope/filter the info
// accessed. Eg. if our model for DB purposes consist of 
// a CreatedDate field we may not want to send this back
// So in our controller we gaurd the info the model returns
// Via a DTO.
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Domain.Dto
{
    public class VillaDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public required string Name { get; set; }
        public string? Details { get; set; }
        [Required]
        public double Rate { get; set; }
        public int Sqft { get; set; }
        public int Occupancy { get; set; }
        public string? ImageUrl { get; set; }
        public string? Amenity { get; set; }
    }
}