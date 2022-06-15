using MiniBankAPI.Models.Centrala;
using System.Collections.Generic;

namespace MiniBankAPI.Interfaces.Centrala {
    public interface ICardInfoService {
        IEnumerable<CardInfoModel> GetAllItems();
        CardInfoModel Add(CardInfoModel cardInfo);
        CardInfoModel GetById(string cardNumber);
        CardInfoModel Update(CardInfoModel card);
    }
}
