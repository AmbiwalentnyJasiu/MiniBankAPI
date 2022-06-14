using System.Data.Linq.Mapping;

namespace MiniBankAPI.Models.Centrala {
    [Table(Name = "AddressInfo")]
    public class AddressInfoModel {
        [Column(Name = "ID", DbType = "int NOT NULL IDENTITY",
            IsPrimaryKey = true, IsDbGenerated = true)]
        public int ID { get; set; }

        [Column(Name = "PostalCode", DbType = "varchar(6) NOT NULL")]
        public string PostalCode { get; set; }

        [Column(Name = "City", DbType = "varchar(30) NOT NULL")]
        public string City { get; set; }

        [Column(Name = "Street", DbType = "varchar(30) NOT NULL")]
        public string Street { get; set; }

        [Column(Name ="BuildingNumber",DbType ="int NOT NULL")]
        public int BuildingNumber { get; set; }

        [Column(Name ="FlatNumber",DbType ="int NULL")]
        public int FlatNumber { get; set; }

        public AddressInfoModel() { }

        public AddressInfoModel(string postalCode, string city, string street, int buildingNumber, int flatNumber) {
            PostalCode = postalCode;
            City = city;
            Street = street;
            BuildingNumber = buildingNumber;
            FlatNumber = flatNumber;
        }
    }
}
