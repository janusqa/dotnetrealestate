using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using RealEstate.Dto;

namespace RealEstate.UI.Models.ViewModels
{
    public class VillaNumberCreateView
    {
        [Required]
        public required CreateVillaNumberDto Dto { get; set; }

        [ValidateNever]
        // [NotMapped]
        public IEnumerable<SelectListItem>? VillaList { get; set; }

    }
}