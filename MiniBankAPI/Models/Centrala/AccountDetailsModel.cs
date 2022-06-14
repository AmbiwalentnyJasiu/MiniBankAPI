using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace MiniBankAPI.Models.Centrala {
    [Table(Name="AccountDetails")]
    public class AccountDetailsModel {
        [Column(Name ="AccountNumber", DbType ="varchar(26) NOT NULL",
            IsPrimaryKey =true, IsDbGenerated = false)]
        public string AccountNumber { get; set; }

        [Column(Name ="Balance",DbType ="smallmoney NOT NULL")]
        public double Balance { get; set; }

        [Column(Name ="ContactInfo",DbType ="int NOT NULL")]
        public int ContactInfoID { get; set; }

        [Association(ThisKey ="ContactInfo", OtherKey ="ID")]
        public EntityRef<ContactInfoModel> ContactInfo { get; set; }

        [Column(Name ="CardInfo",DbType ="varchar(16) NOT NULL")]
        public string CardInfoID { get; set; }

        [Association(ThisKey ="CardInfo", OtherKey ="CardNumber")]
        public EntityRef<CardInfoModel> CardInfo { get; set; }

        public AccountDetailsModel() { }

        public AccountDetailsModel(string accountNumber, double balance, int contactInfoID, string cardInfoID, ContactInfoModel contactInfo, CardInfoModel cardInfo) {
            AccountNumber = accountNumber;
            Balance       = balance;
            ContactInfoID = contactInfoID;
            CardInfoID    = cardInfoID;
            ContactInfo   = new EntityRef<ContactInfoModel>(contactInfo);
            CardInfo      = new EntityRef<CardInfoModel>(cardInfo);
        }

    }
}
    