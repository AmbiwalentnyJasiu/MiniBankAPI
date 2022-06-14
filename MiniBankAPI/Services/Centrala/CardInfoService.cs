using MiniBankAPI.Interfaces.Centrala;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using MiniBankAPI.Models.Centrala;

namespace MiniBankAPI.Services.Centrala {
    public class CardInfoService : ICardInfoService {

        private DataContext db = new DbContext().dataContext;

        public CardInfoModel Add(CardInfoModel card) {
            var cards = db.GetTable<CardInfoModel>();
            cards.InsertOnSubmit(card);
            db.SubmitChanges();

            return card;
        }

        public IEnumerable<CardInfoModel> GetAllItems() {
            return db.GetTable<CardInfoModel>().OrderBy(c => c.CardNumber);
        }

        public CardInfoModel GetByCardNumber(string cardNumber) {
            var cards = db.GetTable<CardInfoModel>();
            var card = cards.Where(c => c.CardNumber == cardNumber).FirstOrDefault();
            return card;
        }

        public CardInfoModel UpdateByCardNumber(CardInfoModel card) {
            var cardOld = GetByCardNumber(card.CardNumber);

            if(cardOld != null) {
                if (card.CardOwner != null)
                    cardOld.CardOwner = card.CardOwner;
                if (card.CvcCode != null)
                    cardOld.CvcCode = card.CvcCode;
                if (card.ValidUntil != null)
                    cardOld.ValidUntil = card.ValidUntil;
                db.SubmitChanges();
            }

            return cardOld;
        }
    }
}
