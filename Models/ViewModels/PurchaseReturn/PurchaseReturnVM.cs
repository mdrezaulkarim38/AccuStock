using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AccuStock.Models.ViewModels.PurchaseReturn
{
    public class PurchaseReturnVM
    {
        public int Id { get; set; }

        public int PurchaseId { get; set; }

        public int BranchId { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReturnDate { get; set; }

        [StringLength(100)]
        public string? Notes { get; set; }

        public SelectList? PurchaseList { get; set; }
        public SelectList? BranchList { get; set; }

        [Required(ErrorMessage = "At least one return detail is required.")]
        public List<PurchaseReturnDetailVM> Details { get; set; } = new List<PurchaseReturnDetailVM>();
    }
}
