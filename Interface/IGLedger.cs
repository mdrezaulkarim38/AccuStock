using AccuStock.Models.ViewModels.GeneralLedger;

namespace AccuStock.Interface
{
    public interface IGLedger
    {
        public Task<List<GLedger>> GetAllGLedger();
    }
}
