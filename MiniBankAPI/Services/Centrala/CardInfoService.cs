using MiniBankAPI.Interfaces.Centrala;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using MiniBankAPI.Models.Centrala;

namespace MiniBankAPI.Services.Centrala {
    public class CardInfoService : ICardInfoService {

        private DataContext db;

        public CardInfoService(IDbService ds) {
            db = ds.Get();
        }

        public CardInfoModel Add(CardInfoModel card) {
            var cards = db.GetTable<CardInfoModel>();
            cards.InsertOnSubmit(card);
            db.SubmitChanges();

            return card;
        }

        public IEnumerable<CardInfoModel> GetAllItems() {
            return db.GetTable<CardInfoModel>().OrderBy(c => c.CardNumber);
        }

        public CardInfoModel GetById(string cardNumber) {
            var cards = db.GetTable<CardInfoModel>();
            var card = cards.Where(c => c.CardNumber == cardNumber).FirstOrDefault();
            return card;
        }

        public CardInfoModel Update(CardInfoModel card) {
            var cardOld = GetById(card.CardNumber);

            //if(cardOld != null) {
            //    if (card.CardOwner != null)
            //        cardOld.CardOwner = card.CardOwner;
            //    if (card.CvcCode != null)
            //        cardOld.CvcCode = card.CvcCode;
            //    if (card.ValidUntil != null)
            //        cardOld.ValidUntil = card.ValidUntil;
            //    db.SubmitChanges();
            //}

            cardOld.Update(card);
            db.SubmitChanges();

            return cardOld;
        }
    }
}
