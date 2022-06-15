using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using MiniBankAPI.Models.Centrala;
using System.Text.RegularExpressions;
using MiniBankAPI.Interfaces.Centrala;
using System;

namespace MiniBankAPI.Controllers.Centrala {
    [Route("api/centrala/[controller]")]
    [ApiController]
    public class CardInfoController : ControllerBase {

        private readonly DbContext db = new DbContext();

        private readonly ICardInfoService _service;

        public CardInfoController(ICardInfoService service) {
            _service = service;
        }

        [HttpGet]
        public IActionResult Get() {
           return Ok(_service.GetAllItems());
        }

        [HttpGet("id")]
        public IActionResult Get(string cardNumber = "dupka") {
            if (!ValidateCN(cardNumber))
                return BadRequest("Incorrect card number");

            var card = _service.GetById(cardNumber);

            if (card == null)
                return NotFound();

            return Ok(card);
        }

        [HttpPost]
        public IActionResult Post([FromBody] CardInfoModel card) {
            if (card.CardNumber == null || card.CardOwner == null ||
                card.CvcCode == null || card.ValidUntil == null)
                return BadRequest("Missing data");

            if (!ValidateInput(card)) {
                return BadRequest("Incorrect input");
            }

            var inserted = _service.Add(card);

            return CreatedAtAction("Get", new { cardNumber = inserted.CardNumber }, inserted);
        }

        [HttpPatch]
        public IActionResult Update([FromBody] CardInfoModel card) {

            if(card.CardNumber == null)
                return BadRequest("Missing CardNumber");

            if (!ValidateCN(card.CardNumber))
                return BadRequest("Incorrect CardNumber");

            if (card.ValidUntil != null) {
                if (!ValidateVU(card.ValidUntil))
                    return BadRequest("Incorrect ValidUntil");
            }

            if (card.CardOwner != null) {
                if (!ValidateCO(card.CardOwner))
                    return BadRequest("Incorrect CardOwner");
            }

            if (card.CvcCode != null) {
                if (!ValidateCC(card.CvcCode))
                    return BadRequest("Incorrect CvcCode");
            }

            var updated = _service.Update(card);

            if (updated == null)
                return NotFound();

            return Ok(updated);

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
        private bool ValidateCO(string cardOwner) => Regex.IsMatch(cardOwner, @"^[A-Z][a-z]{2,29} [A-Z][a-z]{2,49}$");
        private bool ValidateCC(string cvcCode) => Regex.IsMatch(cvcCode, @"^[0-9]{3,3}$");
    }
}
