using Microsoft.AspNetCore.Mvc;
using MiniBankAPI.Interfaces;
using MiniBankAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace MiniBankAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ContactInfoController : ControllerBase {
        private readonly SqlConnection db;
        private readonly List<string> servers;

        public ContactInfoController(IDbService ds) {
            db = ds.Get();
            servers = ds.Servers();
        }

        [HttpGet]
        public IActionResult Get() {
            var cmd = new SqlCommand("SELECT * FROM dbo.ContactInfo", db);
            var result = new List<ContactInfoModel>();

            try {
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read()) {
                    result.Add(new ContactInfoModel(rdr.GetInt32(0), rdr.GetString(1), rdr.GetString(2), rdr.GetString(3), rdr.GetString(4), rdr.GetString(5), rdr.GetInt32(6)));
                }
            } catch (Exception e) {
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

            var cmd = new SqlCommand($"SELECT * FROM {server}.JanekOddzial.dbo.ContactInfo", db);
            var list = new List<ContactInfoModel>();

            try {
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                    list.Add(new ContactInfoModel(
                        Convert.ToInt32(rdr["ID"]),
                        rdr["FirstName"].ToString(),
                        rdr["MiddleName"].ToString(),
                        rdr["LastName"].ToString(),
                        rdr["PhoneNumber"].ToString(),
                        rdr["PNDirCode"].ToString(),
                        Convert.ToInt32(rdr["AddressInfo"])));
            } catch (Exception e) {
                return BadRequest(e.Message);
            }

            if (list.Count == 0)
                return NoContent();

            return Ok(list);
        }

        [HttpGet("id")]
        public IActionResult Get(int id) {
            var cmd = new SqlCommand("SELECT * FROM dbo.ContactInfo", db);
            var list = new List<ContactInfoModel>();

            try {
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                    list.Add(new ContactInfoModel(
                        Convert.ToInt32(rdr["ID"]), 
                        rdr["FirstName"].ToString(), 
                        rdr["MiddleName"].ToString(), 
                        rdr["LastName"].ToString(), 
                        rdr["PhoneNumber"].ToString(), 
                        rdr["PNDirCode"].ToString(), 
                        Convert.ToInt32(rdr["AddressInfo"])));
            } catch (Exception e) {
                return BadRequest(e.Message);
            }

            var result = list.Where(a => a.ID == id).FirstOrDefault();

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ContactInfoModel contact, string server = "WINSERV") {

            if (!servers.Contains(server.ToUpper()))
                return BadRequest("Incorrect Server");

            if (contact.FirstName == null || contact.MiddleName == null || contact.LastName == null ||
               contact.PhoneNumber == null || contact.PNDirCode == null || contact.AddressInfo == 0)
                return BadRequest("Missing data");

            if (!ValidateInput(contact))
                return BadRequest("Incorrect data");
            int status;
            var inserted = new ContactInfoModel();
            try {
                var cmd = new SqlCommand($"INSERT INTO dbo.ContactInfo(FirstName, MiddleName, LastName, PhoneNumber, PNDirCode, AddressInfo) Values({contact.ToString(false)})", db);

                status = cmd.ExecuteNonQuery();
                

                if (status == 1) {
                    cmd = new SqlCommand($"SELECT TOP 1 * FROM dbo.ContactInfo ORDER BY ID DESC", db);

                    using var rdr = cmd.ExecuteReader();
                    rdr.Read();

                        inserted = new ContactInfoModel(
                            Convert.ToInt32(rdr["ID"]),
                            rdr["FirstName"].ToString(),
                            rdr["MiddleName"].ToString(),
                            rdr["LastName"].ToString(),
                            rdr["PhoneNumber"].ToString(),
                            rdr["PNDirCode"].ToString(),
                            Convert.ToInt32(rdr["AddressInfo"]));
                    rdr.Close();

                    var cmd2 = new SqlCommand($"INSERT INTO {server}.JanekOddzial.dbo.ContactInfo(ID, FirstName, MiddleName, LastName, PhoneNumber, PNDirCode, AddressInfo) Values({inserted.ToString(true)})", db);
                    
                    cmd2.ExecuteNonQuery();
                }
            } catch (Exception e) {
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
                var cmd = new SqlCommand($"DELETE FROM {server}.JanekOddzial.dbo.ContactInfo WHERE ID = {id}", db);

                success = cmd.ExecuteNonQuery();

                if (success == 1) {
                    cmd = new SqlCommand($"DELETE FROM dbo.ContactInfo WHERE ID = {id}", db);

                    cmd.ExecuteNonQuery();
                }
            } catch (Exception e) {
                return BadRequest(e.Message);
            }

            return success == 1 ? Ok($"Deleted {id} from {server} and Central") : NotFound();
        }

        private bool ValidateInput(ContactInfoModel address) {
            var fNameMatch = ValidateFN(address.FirstName);

            var mNameMatch = ValidateMN(address.MiddleName);

            var lNameMatch = ValidateLN(address.LastName);

            var pNumberMatch = ValidatePN(address.PhoneNumber);

            var pnDirMatch = ValidatePD(address.PNDirCode);

            return fNameMatch && mNameMatch && lNameMatch && pNumberMatch && pnDirMatch;
        }

        private bool ValidateFN(string firstName) => Regex.IsMatch(firstName, @"^[A-ZĄĆĘŁŃÓŚŹŻ][a-ząćęłńóśźż]{2,29}$");
        private bool ValidateMN(string middleName) => Regex.IsMatch(middleName, @"^[A-ZĄĆĘŁŃÓŚŹŻ][a-ząćęłńóśźż]{2,29}$|^$");
        private bool ValidateLN(string lastName) => Regex.IsMatch(lastName, @"^[A-ZĄĆĘŁŃÓŚŹŻ][a-ząćęłńóśźż]{2,49}$");
        private bool ValidatePN(string phoneNumber) => Regex.IsMatch(phoneNumber, @"^[0-9]{9,9}$");
        private bool ValidatePD(string pnDirCode) => Regex.IsMatch(pnDirCode, @"^\+[0-9]{1,3}$");
    }
}
