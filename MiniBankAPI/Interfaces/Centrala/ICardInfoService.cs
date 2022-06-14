using MiniBankAPI.Models.Centrala;
using System.Collections.Generic;

namespace MiniBankAPI.Interfaces.Centrala {
    public interface ICardInfoService {
        IEnumerable<CardInfoModel> GetAllItems();
        CardInfoModel Add(CardInfoModel cardInfo);
        CardInfoModel GetByCardNumber(string cardNumber);
        CardInfoModel UpdateByCardNumber(CardInfoModel card);
    }
}
