using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;

namespace CarSalesSystem.Services
{
    public interface IDealerService
    {
        void Add(DealerDto dto);
        bool CheckIsDealerByUserId(string userId);
        Dealer GetById(int id);
        List<DealerDto> GetAll();
        void Update(Dealer dealer);
        Dealer Details(int id);
        void Edit(DealerDto dto);

    }
}
