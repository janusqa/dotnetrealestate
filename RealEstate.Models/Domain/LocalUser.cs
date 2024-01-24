using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Domain
{
    public class LocalUser : BaseDomain
    {
        public int Id { get; set; }
        [Required]
        public required string UserName { get; set; }
        [Required]
        public required string Password { get; set; }
        [Required]
        public required string Role { get; set; }
        public string? Name { get; set; }
    }
}