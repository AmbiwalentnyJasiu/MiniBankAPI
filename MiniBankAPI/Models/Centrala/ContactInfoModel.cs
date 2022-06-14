using System.Data.Linq.Mapping;
using System.Data.Linq;

namespace MiniBankAPI.Models.Centrala {
    [Table(Name = "ContactInfo")]
    public class ContactInfoModel {
        [Column(Name = "ID", DbType = "int NOT NULL IDENTITY",
            IsPrimaryKey = true, IsDbGenerated = true)]
        public int ID { get; set; }

        [Column(Name ="FirstName",DbType ="varchar(30) NOT NULL")]
        public string FirstName { get; set; }

        [Column(Name ="MiddleName",DbType ="varchar(30) NULL")]
        public string MiddleName { get; set; }

        [Column(Name="LastName",DbType ="varchar(50) NOT NULL")]
        public string LastName { get; set; }

        [Column(Name ="PhoneNumber", DbType ="varchar(9) NOT NULL")]
        public string PhoneNumber { get; set; }

        [Column(Name ="PNDirCode",DbType ="varchar(4) NOT NULL")]
        public string PNDirCode { get; set; }

        [Column(Name ="AddressInfo", DbType ="int NOT NULL")]
        public int AddressInfoID { get; set; }

        [Association(ThisKey = "AddressInfo", OtherKey = "ID")]
        public EntityRef<AddressInfoModel> AddressInfo { get; set; }

        public ContactInfoModel() { }

    }
}
