using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using MiniBankAPI.Models.Centrala;
using System.Text.RegularExpressions;
using MiniBankAPI.Interfaces.Centrala;

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
        public IActionResult GetAllCardInfos() {
           return Ok(_service.GetAllItems());
        }

        [HttpGet("id")]
        public IActionResult GetCardInfo(string cardNumber) {
            if (!ValidateCN(cardNumber))
                return BadRequest("Incorrect card number");

            var card = _service.GetByCardNumber(cardNumber);

            if (card == null)
                return NotFound();

            return Ok(card);
        }

        //[HttpPost]
        //public IActionResult PostCardInfo(string cardNumber, string validUntil, string cardOwner, string cvcCode) {
        //    var newCard = new CardInfoModel(cardNumber, validUntil, cardOwner, cvcCode);
        //    var cards = db.dataContext.GetTable<CardInfoModel>();
        //    bool result = false;
        //    if (ValidateInput(cardNumber, validUntil, cardOwner, cvcCode)) {
        //        cards.InsertOnSubmit(newCard);
        //        result = true;
        //    }

        //    db.dataContext.SubmitChanges();
        //    return result ? Ok() : BadRequest("Incorrect data");
        //}

        [HttpPost]
        public IActionResult PostCardInfo([FromBody] CardInfoModel card) {
           
            if (!ValidateInput(card)) {
                return BadRequest("Incorrect input");
            }

            var inserted = _service.Add(card);

            return CreatedAtAction("Get", new { cardNumber = inserted.CardNumber }, inserted);
        }

        [HttpPatch]
        public IActionResult UpdateCardInfo([FromBody] CardInfoModel card) {

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

            var updated = _service.UpdateByCardNumber(card);

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

        private bool ValidateCN(string cardNumber) => Regex.Match(cardNumber, @"^\d{16,16}$").Success;
        private bool ValidateVU(string validUntil) => Regex.Match(validUntil, @"^[0-1]\d\/\d{2,2}$").Success;
        private bool ValidateCO(string cardOwner) => Regex.Match(cardOwner, @"^[A-Z][a-z]{2,29} [A-Z][a-z]{2,49}$").Success;
        private bool ValidateCC(string cvcCode) => Regex.Match(cvcCode, @"^[0-9]{3,3}$").Success;
    }
}
