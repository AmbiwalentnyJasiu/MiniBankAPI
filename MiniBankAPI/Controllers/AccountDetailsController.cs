using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniBankAPI.Interfaces;
using MiniBankAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MiniBankAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AccountDetailsController : ControllerBase {
        private readonly SqlConnection db;
        private readonly List<string> servers;

        public AccountDetailsController(IDbService ds) {
            db = ds.Get();
            servers = ds.Servers();
        }

        [HttpGet]
        public IActionResult Get() {
            var cmd = new SqlCommand("SELECT * FROM dbo.AccountDetails", db);
            var result = new List<AccountDetailsModel>();

            try {
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read()) {
                    result.Add(new AccountDetailsModel(
                        rdr["AccountNumber"].ToString(),
                        Convert.ToDouble(rdr["Balance"]),
                        Convert.ToInt32(rdr["ContactInfo"]),
                        rdr["CardInfo"].ToString(),
                        rdr["Division"].ToString()));
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

            var cmd = new SqlCommand($"SELECT * FROM dbo.AccountDetails", db);
            var list = new List<AccountDetailsModel>();

            try {
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                        list.Add(new AccountDetailsModel(
                        rdr["AccountNumber"].ToString(),
                        Convert.ToDouble(rdr["Balance"]),
                        Convert.ToInt32(rdr["ContactInfo"]),
                        rdr["CardInfo"].ToString(),
                        rdr["Division"].ToString()));
            } catch (Exception e) {
                return BadRequest(e.Message);
            }

            list.RemoveAll(c => !c.Division.Equals(server, StringComparison.OrdinalIgnoreCase));

            if (list.Count == 0)
                return NoContent();

            return Ok(list);
        }

        [HttpGet("{accNumber}")]
        public IActionResult Get(string accNumber) {
            var cmd = new SqlCommand("SELECT * FROM dbo.AccountDetails", db);
            var list = new List<AccountDetailsModel>();

            try {
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                    list.Add(new AccountDetailsModel(
                        rdr["AccountNumber"].ToString(),
                        Convert.ToDouble(rdr["Balance"]),
                        Convert.ToInt32(rdr["ContactInfo"]),
                        rdr["CardInfo"].ToString(),
                        rdr["Division"].ToString()));
            } catch (Exception e) {
                return BadRequest(e.Message);
            }

            var result = list.Where(c => c.AccountNumber == accNumber).FirstOrDefault();

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] AccountDetailsModel acc, string server = "WINSERV") {
            if (!servers.Contains(server.ToUpper()))
                return BadRequest("Incorrect Server");

            if (acc.AccountNumber == null || acc.CardInfo == null ||
                acc.Division == null || acc.ContactInfo == 0)
                return BadRequest("Missing data");

            if (!ValidateInput(acc)) {
                return BadRequest("Incorrect input");
            }

            int status;

            try {
                var cmd = new SqlCommand($"INSERT INTO dbo.AccountDetails(AccountNumber, Balance, ContactInfo, CardInfo, Division) Values({acc.ToString(true)})", db);

                status = cmd.ExecuteNonQuery();

                if (status == 1) {
                    cmd = new SqlCommand($"INSERT INTO {server}.JanekOddzial.dbo.AccountDetails(AccountNumber, ContactInfo, CardInfo) Values({acc.ToString(false)})", db);

                    cmd.ExecuteNonQuery();
                }
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
            return status == 1 ? Ok(acc) : BadRequest();
        }

        [HttpDelete]
        public IActionResult Remove(string accNumber, string server = "WINSERV") {
            if (!servers.Contains(server.ToUpper()))
                return BadRequest("Incorrect Server");

            if (!ValidateAN(accNumber))
                return BadRequest("Incorrect CardNumber");

            int success;
            try {
                var cmd = new SqlCommand($"DELETE FROM {server}.JanekOddzial.dbo.AccountDetails WHERE AccountNumber = {accNumber}", db);

                success = cmd.ExecuteNonQuery();

                if (success == 1) {
                    cmd = new SqlCommand($"DELETE FROM dbo.AccountDetails WHERE AccountNumber = {accNumber}", db);

                    cmd.ExecuteNonQuery();
                }
            } catch (Exception e) {
                return BadRequest(e.Message);
            }

            return success == 1 ? Ok($"Deleted {accNumber} from {server} and Central") : NotFound();
        }

        private bool ValidateInput(AccountDetailsModel acc) {
            var aMatch = ValidateAN(acc.AccountNumber);

            var cMatch = ValidateCI(acc.CardInfo);

            var dMatch = ValidateD(acc.Division);

            return aMatch && cMatch && dMatch;
        }

        private bool ValidateAN(string accNumber) => Regex.IsMatch(accNumber, @"^\d{26,26}$");
        private bool ValidateCI(string cardInfo) => Regex.IsMatch(cardInfo, @"^\d{16,16}$");
        private bool ValidateD(string division) => Regex.IsMatch(division, @"^WINSERV[0-9]{3,3}$") && servers.Contains(division.ToUpper());
    }
}
