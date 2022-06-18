
namespace MiniBankAPI.Models {
    public class TransactionModel {
        public int ID { get; set; }
        public string SenderID { get; set; }
        public double Amount { get; set; }
        public string DateSent { get; set; }
        public string ReceiverID { get; set; }

        public TransactionModel() { }

        public TransactionModel(int id, string senderID, double amount, string date, string receiverID) {
            ID = id;
            SenderID = senderID;
            Amount = amount;
            DateSent = date;
            ReceiverID = receiverID;
        }

        public string ToString(bool mode) {
            return $"{(mode ? ID + ", " : "")}'{SenderID}', {Amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}, '{DateSent}', '{ReceiverID}'";
        }
    }
}
