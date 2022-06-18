
namespace MiniBankAPI.Models {
    public class ContactInfoModel {
        public int ID { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string PNDirCode { get; set; }

        public int AddressInfo { get; set; }

        public ContactInfoModel() { }
        public ContactInfoModel(int id, string firstName, string middleName, string lastName, string phoneNumber, string pnDirCode, int addressInfo) {
            ID = id;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            PNDirCode = pnDirCode;
            AddressInfo = addressInfo;
        }

        public string ToString(bool mode) {
            return $"{(mode ? ID + ", " : "")}'{FirstName}', '{MiddleName}', '{LastName}', '{PhoneNumber}', '{PNDirCode}', {AddressInfo}";
        }

    }
}
