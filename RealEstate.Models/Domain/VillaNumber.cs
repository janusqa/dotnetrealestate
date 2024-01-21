using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Models.Domain
{
    public class VillaNumber : BaseDomain
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)] // creates a primary that must be provided by the user
        public int VillaNo { get; set; }
        public string? SpecialDetails { get; set; }
        public int VillaId { get; set; }
        [ForeignKey("VillaId")]
        public Villa? Villa { get; set; }
    }
}