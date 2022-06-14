using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using MiniBankAPI.Models.Centrala;
using System.Text.RegularExpressions;

namespace MiniBankAPI.Controllers.Centrala {
    [Route("api/centrala/[controller]")]
    [ApiController]
    public class AddressInfoController : ControllerBase {
        private readonly DbContext db = new DbContext();

        [HttpGet]
        public IEnumerable<AddressInfoModel> GetAllAddressInfos() {
            return db.dataContext.GetTable<AddressInfoModel>().OrderBy(c => c.ID);
        }

        [HttpGet("id")]
        public IActionResult GetAddressInfo(int id) {
            var addresses = db.dataContext.GetTable<AddressInfoModel>();
            var address = addresses.Where(a => a.ID == id).FirstOrDefault();
            return Ok(address);
        }

        [HttpPost]
        public IActionResult PostAddressInfo(string postalCode, string city, string street, int buildingNumber, int flatNumber) {
            var newAddress = new AddressInfoModel(postalCode, city, street, buildingNumber, flatNumber);
            var addresses = db.dataContext.GetTable<AddressInfoModel>();
            bool result = false;
            if (ValidateInput(postalCode, city, street)) {
                addresses.InsertOnSubmit(newAddress);
                result = true;
            }

            db.dataContext.SubmitChanges();
            return result ? Ok() : BadRequest("Incorrect data");
        }

        [HttpPatch]
        public IActionResult UpdateCardInfo(int id, string postalCode = "", string city = "", string street = "", int buildingNumber = 0, int flatNumber = 0) {
            var addresses = db.dataContext.GetTable<AddressInfoModel>();
            var address = addresses.Where(c => c.ID == id).FirstOrDefault();

            if (postalCode != "") {
                if (ValidatePC(postalCode))
                    address.PostalCode = postalCode;
                else
                    return BadRequest("Incorrect PostalCode");
            }

            if (city != "") {
                if (ValidateC(city))
                    address.City = city;
                else
                    return BadRequest("Incorrect City");
            }

            if (street != "") {
                if (ValidateS(street))
                    address.Street = street;
                else
                    return BadRequest("Incorrect Street");
            }

            if (buildingNumber > 0)
                address.BuildingNumber = buildingNumber;

            if (flatNumber > 0)
                address.FlatNumber = flatNumber;

            db.dataContext.SubmitChanges();

            return Ok();

        }

        private bool ValidateInput(string postalCode, string city, string street) {
            var pCodeMatch = ValidatePC(postalCode);

            var vUntilMatch = ValidateC(city);

            var cOwnerMatch = ValidateS(street);

            return pCodeMatch && vUntilMatch && cOwnerMatch;
        }

        private bool ValidatePC(string postalCode) => Regex.IsMatch(postalCode, @"^\d{2,2}-\d{3,3}$");
        private bool ValidateC(string city) => Regex.IsMatch(city, @"^([[A-Z][a-z]* )*[[A-Z][a-z]*$|^[[A-Z][a-z]*$") && city.Length <= 30;
        private bool ValidateS(string street) => Regex.IsMatch(street, @"^([[A-Z][a-z]* )*[[A-Z][a-z]*$|^[[A-Z][a-z]*$") && street.Length <= 30 ;
    }
}
