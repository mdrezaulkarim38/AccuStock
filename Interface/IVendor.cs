using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface IVendor
    {
        Task<List<Vendor>> GetAllVendor();
        Task<Vendor> GetVendorById(int id);
        Task<bool> CreateVendor(Vendor vendor);
        Task<bool> UpdateVendor(Vendor vendor);
        Task<bool> DeleteVendor(int id);
        Task<bool> ToggleVendorStatus(int id);
    }
}
