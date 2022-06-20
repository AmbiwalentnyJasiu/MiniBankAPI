using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using MiniBankAPI.Models;
using System.Text.RegularExpressions;
using MiniBankAPI.Interfaces;
using System.Data.SqlClient;

namespace MiniBankAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AddressInfoController : ControllerBase {

        private readonly SqlConnection db;
        private readonly List<string> servers;

        public AddressInfoController(IDbService ds) {
            db = ds.Get();
            servers = ds.Servers();
        }

        [HttpGet]
        public IActionResult Get() {
            var cmd = new SqlCommand("SELECT * FROM dbo.AddressInfo", db);
            var result = new List<AddressInfoModel>();

            try {
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read()) {
                    result.Add(new AddressInfoModel(
                        Convert.ToInt32(rdr["ID"]),
                        rdr["PostalCode"].ToString(),
                        rdr["City"].ToString(),
                        rdr["Street"].ToString(),
                        rdr["BuildingNumber"].ToString(),
                        rdr["FlatNumber"].ToString()));
                }
            } catch(Exception e) {
                return BadRequest(e.Message);
            }

            if (result.Count == 0)
                return NoContent();

            return Ok(result);
        }

        [HttpGet("servers/{server}")]
        public IActionResult GetFromServer(string server = "WINSERV") {
            if (!servers.Contains(server.ToUpper()))
                return BadRequest("Incorrect Server");

            var cmd = new SqlCommand($"SELECT * FROM {server}.JanekOddzial.dbo.AddressInfo", db);
            var list = new List<AddressInfoModel>();

            try {
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                    list.Add(new AddressInfoModel(
                        Convert.ToInt32(rdr["ID"]), 
                        rdr["PostalCode"].ToString(), 
                        rdr["City"].ToString(), 
                        rdr["Street"].ToString(), 
                        rdr["BuildingNumber"].ToString(), 
                        rdr["FlatNumber"].ToString()));
            } catch (Exception e) {
                return BadRequest(e.Message);
            }

            if (list.Count == 0)
                return NoContent();

            return Ok(list);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id) {
            var cmd = new SqlCommand("SELECT * FROM dbo.AddressInfo", db);
            var list = new List<AddressInfoModel>();

            try {
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                    list.Add(new AddressInfoModel(
                        Convert.ToInt32(rdr["ID"]),
                        rdr["PostalCode"].ToString(),
                        rdr["City"].ToString(),
                        rdr["Street"].ToString(),
                        rdr["BuildingNumber"].ToString(),
                        rdr["FlatNumber"].ToString()));
            } catch (Exception e) {
                return BadRequest(e.Message);
            }

            var result = list.Where(a => a.ID == id).FirstOrDefault();

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] AddressInfoModel address, string server = "WINSERV") {
            if (!servers.Contains(server.ToUpper()))
                return BadRequest("Incorrect Server");

            if (address.City == null || address.PostalCode == null ||
               address.Street == null || address.BuildingNumber == null)
                return BadRequest("Missing data");

            if (!ValidateInput(address))
                return BadRequest("Incorrect data");

            int status;

            var inserted = new AddressInfoModel();
            try {
                var cmd = new SqlCommand($"INSERT INTO dbo.AddressInfo(PostalCode, City, Street, BuildingNumber, FlatNumber) Values({address.ToString()})", db);

                status = cmd.ExecuteNonQuery();
                
                if(status == 1) {
                    cmd = new SqlCommand($"SELECT TOP 1 * FROM dbo.AddressInfo ORDER BY ID DESC", db);

                    var rdr = cmd.ExecuteReader();
                    rdr.Read();

                    inserted = new AddressInfoModel(
                    Convert.ToInt32(rdr["ID"].ToString()),
                    rdr["PostalCode"].ToString(),
                    rdr["City"].ToString(),
                    rdr["Street"].ToString(),
                    rdr["BuildingNumber"].ToString(),
                    rdr["FlatNumber"].ToString());

                    rdr.Close();

                    cmd = new SqlCommand($"INSERT INTO {server}.JanekOddzial.dbo.AddressInfo(ID, PostalCode, City, Street, BuildingNumber, FlatNumber) Values({inserted.ToString(true)})", db);

                    cmd.ExecuteNonQuery();
                }


            } catch(Exception e) {
                return BadRequest(e.Message);
            }
            return status == 1 ? Ok(inserted) : BadRequest();
        }


        [HttpDelete]
        public IActionResult Remove(int id, string server = "WINSERV") {
            if (!servers.Contains(server.ToUpper()))
                return BadRequest("Incorrect Server");

            int success;
            try {
                var cmd = new SqlCommand($"DELETE FROM {server}.JanekOddzial.dbo.AddressInfo WHERE ID = {id}", db);

                success = cmd.ExecuteNonQuery();

                if(success == 1) {
                    cmd = new SqlCommand($"DELETE FROM dbo.AddressInfo WHERE ID = {id}", db);

                    cmd.ExecuteNonQuery();
                }
            } catch (Exception e){
                return BadRequest(e.Message);
            }

            return success == 1 ? Ok($"Deleted {id} from {server} and Central") : NotFound();
        }

        private bool ValidateInput(AddressInfoModel address) {
            var pCodeMatch = ValidatePC(address.PostalCode);

            var vUntilMatch = ValidateC(address.City);

            var cOwnerMatch = ValidateS(address.Street);

            var bNumberMatch = ValidateBN(address.BuildingNumber);

            var fNumberMatch = ValidateFN(address.FlatNumber);

            return pCodeMatch && vUntilMatch && cOwnerMatch && bNumberMatch && fNumberMatch;
        }

        private bool ValidatePC(string postalCode) => Regex.IsMatch(postalCode, @"^\d{2,2}-\d{3,3}$");
        private bool ValidateC(string city) => Regex.IsMatch(city, @"^([A-ZĄĆĘŁŃÓŚŹŻ][a-ząćęłńóśźż]* )*[A-ZĄĆĘŁŃÓŚŹŻ][a-ząćęłńóśźż]*$|^[A-ZĄĆĘŁŃÓŚŹŻ][a-ząćęłńóśźż]*$") && city.Length <= 30;
        private bool ValidateS(string street) => Regex.IsMatch(street, @"^([A-ZĄĆĘŁŃÓŚŹŻ][a-ząćęłńóśźż]* )*[A-ZĄĆĘŁŃÓŚŹŻ][a-ząćęłńóśźż]*$|^[A-ZĄĆĘŁŃÓŚŹŻ][a-ząćęłńóśźż]*$") && street.Length <= 30 ;
        private bool ValidateBN(string buildingNumber) => Regex.IsMatch(buildingNumber, @"^[1-9][0-9]{0,3}[A-Z]{0,1}$");
        private bool ValidateFN(string flatNumber) => flatNumber == "" ? true : Regex.IsMatch(flatNumber, @"^[1-9][0-9]{0,3}[A-Z]{0,1}$");
    }
}
