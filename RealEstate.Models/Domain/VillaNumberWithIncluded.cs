
namespace RealEstate.Models.Domain
{
    public class VillaNumberWithIncluded : Villa
    {
        public int VnId { get; set; }
        public int VillaNo { get; set; }
        public string? SpecialDetails { get; set; }
    }
}