using MiniBankAPI.Models.Centrala;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniBankAPI.Interfaces.Centrala {
    public interface IAddressInfoService {
        IEnumerable<AddressInfoModel> GetAllItems();
        AddressInfoModel Add(AddressInfoModel cardInfo);
        AddressInfoModel GetById(string cardNumber);
        AddressInfoModel Update(AddressInfoModel card);
    }
}
