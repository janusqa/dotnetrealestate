using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using RealEstate.Dto;

namespace RealEstate.UI.Models.ViewModels
{
    public class VillaNumberUpdateView
    {
        [Required]
        public required UpdateVillaNumberDto Dto { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem>? VillaList { get; set; }

    }
}