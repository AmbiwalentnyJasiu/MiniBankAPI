using Microsoft.AspNetCore.Mvc;
using MiniBankAPI.Controllers.Centrala;
using MiniBankAPI.Interfaces.Centrala;
using MiniBankAPI.Models.Centrala;
using MiniBankAPITests.Fakers.Centrala;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MiniBankAPITests.ControllerTests.Centrala {
    public class CardInfoControllerTest {
        private readonly CardInfoController _controller;
        private readonly ICardInfoService _service;

        public CardInfoControllerTest() {
            _service = new CardInfoServiceFake();
            _controller = new CardInfoController(_service);
        }

        [Fact]
        public void Get_ReturnsOk() {
            var result = _controller.Get();

            Assert.IsType<OkObjectResult>(result as OkObjectResult);
        }

        [Fact]
        public void Get_ReturnsAllItems() {
            var result = _controller.Get() as OkObjectResult;

            var cards = Assert.IsType<List<CardInfoModel>>(result.Value);
            Assert.Equal(3, cards.Count);
        }

        [Fact]
        public void GetByCardNumber_ReturnNotFound() {
            var number = "0000000000000000";

            var result = _controller.Get(number);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetByCardNumber_ReturnsOk() {
            var number = "1111222211112222";

            var result = _controller.Get(number);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetByCardNumber_ReturnsCorrectCard() {
            var number = "1111222211112222";

            var result = _controller.Get(number) as OkObjectResult;

            Assert.IsType<CardInfoModel>(result.Value);
            Assert.Equal(number, (result.Value as CardInfoModel).CardNumber);
        }

        [Fact]
        public void Add_InvalidMissing_ReturnsBadRequest() {
            var missingDataCard = new CardInfoModel() {
                CardNumber = "1111111122222222",
                CvcCode = "123",
                ValidUntil = "10/22"
            };

            var response = _controller.Post(missingDataCard);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public void Add_InvalidIncorrectCvcCode_ReturnsBadRequest() {
            var incorrectDataCard = new CardInfoModel() {
                CardNumber = "1111111122222222",
                CvcCode = "123s",
                CardOwner = "Jozef Gozka",
                ValidUntil = "10/22"
            };

            var response = _controller.Post(incorrectDataCard);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public void Add_InvalidIncorrectCardNumber_ReturnsBadRequest() {
            var incorrectDataCard = new CardInfoModel() {
                CardNumber = "11s1111122222222",
                CvcCode = "123",
                CardOwner = "Jozef Gozka",
                ValidUntil = "10/22"
            };

            var response = _controller.Post(incorrectDataCard);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public void Add_InvalidIncorrectCardOwner_ReturnsBadRequest() {
            var incorrectDataCard = new CardInfoModel() {
                CardNumber = "1111111122222222",
                CvcCode = "123",
                CardOwner = "Jozef gozka",
                ValidUntil = "10/22"
            };

            var response = _controller.Post(incorrectDataCard);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public void Add_InvalidIncorrectValidUntil_ReturnsBadRequest() {
            var incorrectDataCard = new CardInfoModel() {
                CardNumber = "1111111122222222",
                CvcCode = "123",
                CardOwner = "Jozef Gozka",
                ValidUntil = "10's2"
            };

            var response = _controller.Post(incorrectDataCard);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public void Add_Valid_ReturnsCreated() {
            var correctDataCard = new CardInfoModel() {
                CardNumber = "1111111122222222",
                CvcCode = "123",
                CardOwner = "Jozef Gozka",
                ValidUntil = "10/22"
            };

            var response = _controller.Post(correctDataCard);

            Assert.IsType<CreatedAtActionResult>(response);
        }

        [Fact]
        public void Add_Valid_ReturnsCreatedCard() {
            var correctDataCard = new CardInfoModel() {
                CardNumber = "1111111122222222",
                CvcCode = "123",
                CardOwner = "Jozef Gozka",
                ValidUntil = "10/22"
            };

            var response = _controller.Post(correctDataCard) as CreatedAtActionResult;
            var item = response.Value as CardInfoModel;

            Assert.IsType<CardInfoModel>(item);
            Assert.Equal("Jozef Gozka", item.CardOwner);
        }

        [Fact]
        public void Update_InvalidCardNumber_ReturnsNotFound() {
            var incorrectDataCard = new CardInfoModel() {
                CardNumber = "0000000000000000",
                CvcCode = "123",
                CardOwner = "Jozef Gozka",
                ValidUntil = "10/22"
            };

            var result = _controller.Update(incorrectDataCard);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Update_InvalidCvcCode_ReturnsBadRequest() {
            var incorrectDataCard = new CardInfoModel() {
                CardNumber = "1111222211112222",
                CvcCode = "123s",
                CardOwner = "Jozef Gozka",
                ValidUntil = "10/22"
            };

            var result = _controller.Update(incorrectDataCard);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Update_InvalidCardOwner_ReturnsBadRequest() {
            var incorrectDataCard = new CardInfoModel() {
                CardNumber = "1111222211112222",
                CvcCode = "123",
                CardOwner = "Jozef gozka",
                ValidUntil = "10/22"
            };

            var result = _controller.Update(incorrectDataCard);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Update_InvalidValidUntil_ReturnsBadRequest() {
            var incorrectDataCard = new CardInfoModel() {
                CardNumber = "1111222211112222",
                CvcCode = "123",
                CardOwner = "Jozef Gozka",
                ValidUntil = "14/22"
            };

            var result = _controller.Update(incorrectDataCard);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Update_ValidData_ReturnsOk() {
            var correctDataCard = new CardInfoModel() {
                CardNumber = "1111222211112222",
                CvcCode = "123",
                CardOwner = "Jozef Gozka",
                ValidUntil = "10/22"
            };

            var result = _controller.Update(correctDataCard);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Update_ValidData_ReturnsOkCorrectCard() {
            var correctDataCard = new CardInfoModel() {
                CardNumber = "1111222211112222",
                CvcCode = "123",
                CardOwner = "Jozef Gozka",
                ValidUntil = "10/22"
            };

            var result = (_controller.Update(correctDataCard) as OkObjectResult).Value as CardInfoModel;

            Assert.IsType<CardInfoModel>(result);

            Assert.Equal("Jozef Gozka", result.CardOwner);
        }
    }
}

