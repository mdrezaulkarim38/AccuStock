using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AccuStock.Models.ViewModels.Purchase;

    public class PurchaseViewModel
    {
        public int Id { get; set; }
        [Required]
        public int VendorId { get; set; }
        [Required]
        public int BranchId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime PurchaseDate { get; set; }
        public string? Notes { get; set; }
        public IEnumerable<SelectListItem>? VendorList { get; set; }
        public IEnumerable<SelectListItem>? BranchList { get; set; }
        public IEnumerable<SelectListItem>? ProductList { get; set; }
        public List<PurchaseDetailViewModel> Details { get; set; } = new();
    }

