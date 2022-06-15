using System.Data.Linq.Mapping;

namespace MiniBankAPI.Models.Centrala {
    [Table(Name = "CardInfo")]
    public class CardInfoModel {
        [Column(Name = "CardNumber", DbType = "varchar(16) NOT NULL",
            IsDbGenerated = false, IsPrimaryKey = true)]
        public string CardNumber { get; set; }

        [Column(Name = "ValidUntil", DbType = "varchar(5) NOT NULL")]
        public string ValidUntil { get; set; }

        [Column(Name = "CardOwner", DbType = "varchar(81) NOT NULL")]
        public string CardOwner { get; set; }

        [Column(Name = "CvcCode", DbType = "varchar(3) NOT NULL")]
        public string CvcCode { get; set; }

        public CardInfoModel(string cardNumber, string validUntil, string cardOwner, string cvcCode) {
            CardNumber = cardNumber;
            ValidUntil = validUntil;
            CardOwner = cardOwner;
            CvcCode = cvcCode;
        }

        public CardInfoModel() { }

        public void Update(CardInfoModel other){
            if (other.CardOwner != null)
                CardOwner = other.CardOwner;

            if (other.CvcCode != null)
                //        cardOld.CvcCode = card.CvcCode;
                //    if (card.ValidUntil != null)
                //        cardOld.ValidUntil = card.ValidUntil
                ValidUntil = other.ValidUntil;
            CvcCode = other.CvcCode;
        }
    }
}
