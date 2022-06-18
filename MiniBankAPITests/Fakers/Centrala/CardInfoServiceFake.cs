using MiniBankAPI.Interfaces.Centrala;
using MiniBankAPI.Models.Centrala;
using System.Collections.Generic;
using System.Linq;

namespace MiniBankAPITests.Fakers.Centrala {
    class CardInfoServiceFake : IService {
        private readonly List<CardInfoModel> _cards;

        public CardInfoServiceFake() {
            _cards = new List<CardInfoModel> {
                new CardInfoModel(){CardNumber = "1111222211112222", CardOwner="Anna Michalska", CvcCode="696",ValidUntil="09/23"},
                new CardInfoModel(){CardNumber = "3333333344444444", CardOwner="Tomasz Michalski", CvcCode="123",ValidUntil="11/27"},
                new CardInfoModel(){CardNumber = "1234567891011129", CardOwner="Marta Michalska", CvcCode="192",ValidUntil="04/25"},
            };
        }

        public CardInfoModel Add(CardInfoModel cardInfo) {
            _cards.Add(cardInfo);
            return cardInfo;
        }

        public IEnumerable<CardInfoModel> GetAllItems() {
            return _cards;
        }

        public CardInfoModel GetByCardNumber(string cardNumber) {
            return _cards.Where(c => c.CardNumber == cardNumber).FirstOrDefault();
        }

        public CardInfoModel UpdateByCardNumber(CardInfoModel card) {
            var cardOld = _cards.Where(c => c.CardNumber == card.CardNumber).FirstOrDefault();

            if(cardOld == null)
                return null;
            _cards.Remove(cardOld);
            _cards.Add(card);

            return card;
        }
    }
}
