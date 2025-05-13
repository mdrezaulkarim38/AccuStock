using Microsoft.AspNetCore.Mvc.Rendering;

namespace AccuStock.Models.ViewModels.Sale
{
    public class SaleViewModel
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public int CustomerId { get; set; }
        public int BranchId { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }        
        public List<SelectListItem>? CustomerList { get; set; }
        public List<SelectListItem>? BranchList { get; set; }
        public List<SelectListItem>? ProductList { get; set; }      
        public List<SaleDetailVM> Details { get; set; } = new List<SaleDetailVM>();
    }
}
