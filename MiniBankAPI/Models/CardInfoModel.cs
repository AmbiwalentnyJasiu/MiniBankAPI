
namespace MiniBankAPI.Models {
    public class CardInfoModel {
        public string CardNumber { get; set; }

        public string ValidUntil { get; set; }

        public string CardOwner { get; set; }

        public string CvcCode { get; set; }

        public CardInfoModel(string cardNumber, string validUntil, string cardOwner, string cvcCode) {
            CardNumber = cardNumber;
            ValidUntil = validUntil;
            CardOwner = cardOwner;
            CvcCode = cvcCode;
        }

        public CardInfoModel() { }

        public override string ToString() {
            return $"'{CardNumber}', '{ValidUntil}', '{CardOwner}', '{CvcCode}'";
        }
    }
}
