using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Models.Domain
{
    public class BaseDomain
    {
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}