using Microsoft.AspNetCore.Mvc;
using MiniBankAPI.Controllers;
using MiniBankAPI.Interfaces;
using MiniBankAPI.Models;
using MiniBankAPI.Services;
using System.Collections.Generic;
using Xunit;

namespace MiniBankTest {
    [TestCaseOrderer("MiniBankTest.PriorityOrderer", "MiniBankTest")]
    public class CardTest {
        private IDbService dbService;
        private CardInfoController controller;

        public CardTest() {
            dbService = new DbService();
            controller = new CardInfoController(dbService);
        }

        [Fact, TestPriority(-5)]
        public void GetReturnsAllItems() {
            var okResult = controller.Get() as OkObjectResult;

            var items = Assert.IsType<List<CardInfoModel>>(okResult.Value);
            Assert.Equal(5, items.Count);
        }

        [Fact, TestPriority(-5)]
        public void GetFromServerReturnsAllItems() {
            var okResult = controller.GetFromServer("WINSERV019") as OkObjectResult;

            var items = Assert.IsType<List<CardInfoModel>>(okResult.Value);
            Assert.Equal(2, items.Count);
        }

        [Fact, TestPriority(-5)]
        public void GetByNumberReturnsCorrectItem() {
            var okResult = controller.Get("1111222233334444") as OkObjectResult;

            var item = Assert.IsType<CardInfoModel>(okResult.Value);
            Assert.Equal("Jan Paluch", item.CardOwner);
        }

        [Fact, TestPriority(0)]
        public void PostReturnsCreatedItem() {
            var insert = new CardInfoModel() { CardNumber = "1111222233334469", ValidUntil = "10/23", CardOwner = "Ktos Ktos", CvcCode = "123" };
            var okResult = controller.Post(insert, "WINSERV022") as OkObjectResult;

            var item = Assert.IsType<CardInfoModel>(okResult.Value);
            Assert.Equal("Ktos Ktos", item.CardOwner);
        }

        [Fact, TestPriority(5)]
        public void RemoveWorks() {
            var okResult = controller.Remove("1111222233334469", "Winserv022");

            Assert.IsType<OkObjectResult>(okResult);
        }
    }
}
