using Microsoft.AspNetCore.Mvc;
using MiniBankAPI.Models;
using System.Text.RegularExpressions;
using System;
using MiniBankAPI.Interfaces;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

namespace MiniBankAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CardInfoController : ControllerBase {

        private readonly SqlConnection db;
        private readonly List<string> servers;

        public CardInfoController(IDbService ds) {
            db = ds.Get();
            servers = ds.Servers();
        }

        [HttpGet]
        public IActionResult Get() {
            var cmd = new SqlCommand("SELECT * FROM dbo.CardInfo", db);
            var result = new List<CardInfoModel>();

            try {
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read()) {
                    result.Add(new CardInfoModel(
                        rdr["CardNumber"].ToString(), 
                        rdr["ValidUntil"].ToString(), 
                        rdr["CardOwner"].ToString(), 
                        rdr["CvcCode"].ToString()));
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

            var cmd = new SqlCommand($"SELECT * FROM {server}.JanekOddzial.dbo.CardInfo", db);
            var list = new List<CardInfoModel>();

            try {
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                    list.Add(new CardInfoModel(
                        rdr["CardNumber"].ToString(),
                        rdr["ValidUntil"].ToString(),
                        rdr["CardOwner"].ToString(),
                        rdr["CvcCode"].ToString()));
            } catch (Exception e) {
                return BadRequest(e.Message);
            }

            if (list.Count == 0)
                return NoContent();

            return Ok(list);
        }

        [HttpGet("{cardNumber}")]
        public IActionResult Get(string cardNumber) {
            var cmd = new SqlCommand("SELECT * FROM dbo.CardInfo", db);
            var list = new List<CardInfoModel>();

            try {
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                    list.Add(new CardInfoModel(
                        rdr["CardNumber"].ToString(), 
                        rdr["ValidUntil"].ToString(), 
                        rdr["CardOwner"].ToString(), 
                        rdr["CvcCode"].ToString()));
            } catch (Exception e) {
                return BadRequest(e.Message);
            }

            var result = list.Where(c => c.CardNumber == cardNumber).FirstOrDefault();

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] CardInfoModel card, string server = "WINSERV") {
            if (!servers.Contains(server.ToUpper()))
                return BadRequest("Incorrect Server");

            if (card.CardNumber == null || card.CardOwner == null ||
                card.CvcCode == null || card.ValidUntil == null)
                return BadRequest("Missing data");

            if (!ValidateInput(card)) {
                return BadRequest("Incorrect input");
            }

            int status;

            try {
                var cmd = new SqlCommand($"INSERT INTO dbo.CardInfo(CardNumber, ValidUntil, CardOwner, CvcCode) Values({card})",db);

                status = cmd.ExecuteNonQuery();

                if(status == 1) {
                    cmd = new SqlCommand($"INSERT INTO {server}.JanekOddzial.dbo.CardInfo(CardNumber, ValidUntil, CardOwner, CvcCode) Values({card})", db);

                    cmd.ExecuteNonQuery();
                }
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
            return status == 1 ? Ok(card) : BadRequest();
        }

        [HttpDelete]
        public IActionResult Remove(string cardNumber, string server = "WINSERV") {
            if (!servers.Contains(server.ToUpper()))
                return BadRequest("Incorrect Server");

            if (!ValidateCN(cardNumber))
                return BadRequest("Incorrect CardNumber");

            int success;
            try {
                var cmd = new SqlCommand($"DELETE FROM {server}.JanekOddzial.dbo.CardInfo WHERE CardNumber = {cardNumber}", db);

                success = cmd.ExecuteNonQuery();

                if (success == 1) {
                    cmd = new SqlCommand($"DELETE FROM dbo.CardInfo WHERE CardNumber = {cardNumber}", db);

                    cmd.ExecuteNonQuery();
                }
            } catch (Exception e) {
                return BadRequest(e.Message);
            }

            return success == 1 ? Ok($"Deleted {cardNumber} from {server} and Central") : NotFound();
        }

        private bool ValidateInput(CardInfoModel card) {
            var cNumberMatch = ValidateCN(card.CardNumber);

            var vUntilMatch = ValidateVU(card.ValidUntil);

            var cOwnerMatch = ValidateCO(card.CardOwner);

            var cCodeMatch = ValidateCC(card.CvcCode);

            return cNumberMatch && vUntilMatch && cOwnerMatch && cCodeMatch;
        }

        private bool ValidateCN(string cardNumber) => Regex.IsMatch(cardNumber, @"^\d{16,16}$");
        private bool ValidateVU(string validUntil) => Regex.IsMatch(validUntil, @"^[0-1]\d\/\d{2,2}$")
            && Convert.ToInt32(validUntil.Split('/')[0]) > 0 && Convert.ToInt32(validUntil.Split('/')[0]) < 13;
        private bool ValidateCO(string cardOwner) => Regex.IsMatch(cardOwner, @"^[A-ZĄĆĘŁŃÓŚŹŻ][a-ząćęłńóśżź]{2,29} [A-ZĄĆĘŁŃÓŚŹŻ][a-ząćęłńóśźż]{2,49}$");
        private bool ValidateCC(string cvcCode) => Regex.IsMatch(cvcCode, @"^[0-9]{3,3}$");
    }
}
