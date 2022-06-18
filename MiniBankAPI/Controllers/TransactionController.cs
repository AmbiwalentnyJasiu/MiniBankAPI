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
    public class TransactionController : ControllerBase {
        private readonly SqlConnection db;
        private readonly List<string> servers;

        public TransactionController(IDbService ds) {
            db = ds.Get();
            servers = ds.Servers();
        }

        [HttpGet]
        public IActionResult Get() {
            var result = new List<TransactionModel>();
            foreach (var server in servers) {
                var cmd = new SqlCommand($"SELECT * FROM {server}.JanekOddzial.dbo.TransactionHistory", db);

                try {
                    using var rdr = cmd.ExecuteReader();
                    while (rdr.Read()) {
                        result.Add(new TransactionModel(
                            Convert.ToInt32(rdr["ID"]),
                            rdr["SenderID"].ToString(),
                            Convert.ToDouble(rdr["Amount"]),
                            rdr["DateSent"].ToString(),
                            rdr["ReceiverID"].ToString()));
                    }
                } catch (Exception e) {
                    return BadRequest(e.Message);
                }
            }

            if (result.Count == 0)
                return NoContent();

            return Ok(result);
        }

        [HttpGet("servers/{server}")]
        public IActionResult GetFromServer(string server = "WINSERV") {
            if (!servers.Contains(server.ToUpper()))
                return BadRequest("Incorrect Server");

            var result = new List<TransactionModel>();
            var cmd = new SqlCommand($"SELECT * FROM {server}.JanekOddzial.dbo.TransactionHistory", db);

            try {
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read()) {
                    result.Add(new TransactionModel(
                        Convert.ToInt32(rdr["ID"]),
                        rdr["SenderID"].ToString(),
                        Convert.ToDouble(rdr["Amount"]),
                        rdr["DateSent"].ToString(),
                        rdr["ReceiverID"].ToString()));
                }
            } catch (Exception e) {
                return BadRequest(e.Message);
            }


            if (result.Count == 0)
                return NoContent();

            return Ok(result);
        }

        [HttpGet("id")]
        public IActionResult Get(string accNumber) {
            var cmd = new SqlCommand($"SELECT Division FROM dbo.AccountDetails WHERE AccountNumber = {accNumber}", db);
            string server; 
            try {
                server = cmd.ExecuteScalar().ToString();
            } catch (Exception e) {
                return BadRequest(e.Message);
            }

            cmd = new SqlCommand($"SELECT * FROM {server}.JanekOddzial.dbo.TransactionHistory", db);
            var list = new List<TransactionModel>();

            try {
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                    list.Add(new TransactionModel(
                        Convert.ToInt32(rdr["ID"]),
                        rdr["SenderID"].ToString(),
                        Convert.ToDouble(rdr["Amount"]),
                        rdr["DateSent"].ToString(),
                        rdr["ReceiverID"].ToString()));
            } catch (Exception e) {
                return BadRequest(e.Message);
            }

            list.RemoveAll(c => c.SenderID != accNumber);

            if (list.Count == 0)
                return NotFound();

            return Ok(list);
        }

        [HttpPost]
        public IActionResult Post([FromBody] TransactionModel trans) {
            if (trans.SenderID == null || trans.ReceiverID == null ||
                trans.DateSent == null)
                return BadRequest("Missing data");

            if (!ValidateInput(trans)) {
                return BadRequest("Incorrect input");
            }

            int status;
            var inserted = new TransactionModel();

            try {
                var cmd = new SqlCommand($"EXEC dbo.GetAccountBalance '{trans.SenderID}'", db);
                var balance = Convert.ToDouble(cmd.ExecuteScalar());
                if (trans.Amount > balance)
                    return BadRequest("Not enough money");

                cmd = new SqlCommand($"SELECT AccountNumber FROM dbo.AccountDetails WHERE AccountNumber = {trans.ReceiverID}", db);
       
                var receiver = cmd.ExecuteScalar();
                if (receiver == null)
                    return BadRequest("Incorrect receiver");

                cmd = new SqlCommand($"SELECT Division FROM dbo.AccountDetails WHERE AccountNumber = {trans.SenderID}", db);
                string server;

                server = cmd.ExecuteScalar().ToString();
                cmd = new SqlCommand($"INSERT INTO {server}.JanekOddzial.dbo.TransactionHistory(SenderID, Amount, DateSent, ReceiverID) Values({trans.ToString(false)})", db);

                status = cmd.ExecuteNonQuery();

                cmd = new SqlCommand($"EXEC dbo.UpdateMoney '{trans.SenderID}', {-trans.Amount}", db);
                cmd.ExecuteNonQuery();
                cmd = new SqlCommand($"EXEC dbo.UpdateMoney '{trans.ReceiverID}', {trans.Amount}", db);
                cmd.ExecuteNonQuery();

                if (status == 1) {
                    cmd = new SqlCommand($"SELECT TOP 1 * FROM {server}.JanekOddzial.dbo.TransactionHistory ORDER BY ID DESC", db);

                    using var rdr = cmd.ExecuteReader();
                    rdr.Read();

                    inserted = new TransactionModel(
                        Convert.ToInt32(rdr["ID"]),
                        rdr["SenderID"].ToString(),
                        Convert.ToDouble(rdr["Amount"]),
                        rdr["DateSent"].ToString(),
                        rdr["ReceiverID"].ToString());
                }
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
            return status == 1 ? Ok(inserted) : BadRequest();
        }

        [HttpPatch]
        public IActionResult UpdateMoney(string accNumber, double amount) {
            int success;
            try {
                var cmd = new SqlCommand($"EXEC dbo.UpdateMoney '{accNumber}', {amount}", db);
                success = cmd.ExecuteNonQuery();
            }catch(Exception e) {
                return BadRequest(e.Message);
            }
            return success == 1 ? Ok() : NotFound();
        }

        private bool ValidateInput(TransactionModel trans) {
            var cMatch = ValidateAN(trans.SenderID);

            var rMatch = ValidateAN(trans.ReceiverID);

            var dMatch = ValidateD(trans.DateSent);

            return cMatch && rMatch && dMatch;
        }

        private bool ValidateAN(string number) => Regex.IsMatch(number, @"^\d{26,26}$");
        private bool ValidateD(string date) => Regex.IsMatch(date, @"^\d{2,2}\/\d{2,2}\/\d{4,4}$");
    }
}
