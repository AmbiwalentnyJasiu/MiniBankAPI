
namespace MiniBankAPI.Models {
    public class AddressInfoModel {
        public int ID { get; set; }

        public string PostalCode { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string BuildingNumber { get; set; }

        public string FlatNumber { get; set; }

        public AddressInfoModel() { }

        public AddressInfoModel(int id, string postalCode, string city, string street, string buildingNumber, string flatNumber) {
            ID = id;
            PostalCode = postalCode;
            City = city;
            Street = street;
            BuildingNumber = buildingNumber;
            FlatNumber = flatNumber;
        }

        public string ToString(bool mode = false) {
            return $"{(mode ? ID + ", " : "")}'{PostalCode}', '{City}', '{Street}', '{BuildingNumber}', '{FlatNumber}'";
        }
    }
}
