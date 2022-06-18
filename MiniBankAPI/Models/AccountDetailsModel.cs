
namespace MiniBankAPI.Models {
    public class AccountDetailsModel {
        public string AccountNumber { get; set; }

        public double Balance { get; set; }

        public int ContactInfo { get; set; }

        public string CardInfo { get; set; }

        public string Division { get; set; }

        public AccountDetailsModel() { }

        public AccountDetailsModel(string accountNumber, double balance, int contactInfoID, string cardInfoID, string division) {
            AccountNumber = accountNumber;
            Balance = balance;
            ContactInfo = contactInfoID;
            CardInfo = cardInfoID;
            Division = division;
        }

        public string ToString(bool mode) {
            return $"'{AccountNumber}', {(mode ? Balance + ", " : "")}{ContactInfo}, '{CardInfo}' {(mode ? ",'" + Division.ToUpper() + "'" : "")}";
        }

    }
}
