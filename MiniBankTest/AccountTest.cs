using Microsoft.AspNetCore.Mvc;
using MiniBankAPI.Controllers;
using MiniBankAPI.Interfaces;
using MiniBankAPI.Models;
using MiniBankAPI.Services;
using System.Collections.Generic;
using Xunit;

namespace MiniBankTest {

    [TestCaseOrderer("MiniBankTest.PriorityOrderer", "MiniBankTest")]
    public class AccountTest {
        private IDbService dbService;
        private AccountDetailsController controller;

        public AccountTest() {
            dbService = new DbService();
            controller = new AccountDetailsController(dbService);
        }

        [Fact, TestPriority(-5)]
        public void GetReturnsAllItems() {
            var okResult = controller.Get() as OkObjectResult;

            var items = Assert.IsType<List<AccountDetailsModel>>(okResult.Value);
            Assert.Equal(5, items.Count);
        }

        [Fact, TestPriority(-5)]
        public void GetFromServerReturnsAllItems() {
            var okResult = controller.GetFromServer("WINSERV019") as OkObjectResult;

            var items = Assert.IsType<List<AccountDetailsModel>>(okResult.Value);
            Assert.Equal(2, items.Count);
        }

        [Fact, TestPriority(-5)]
        public void GetByNumberReturnsCorrectItem() {
            var okResult = controller.Get("69123412341234123412341234") as OkObjectResult;

            var item = Assert.IsType<AccountDetailsModel>(okResult.Value);
            Assert.Equal(100.90, item.Balance);
        }

        [Fact, TestPriority(0)]
        public void PostReturnsCreatedItem() {
            var insert = new AccountDetailsModel() { AccountNumber = "12345678901234567890567890", Balance = 0, ContactInfo = 6, CardInfo = "1111222233334469", Division = "WINSERV022" };
            var okResult = controller.Post(insert, "WINSERV022") as OkObjectResult;

            var item = Assert.IsType<AccountDetailsModel>(okResult.Value);
            Assert.Equal("1111222233334469", item.CardInfo);
        }

        [Fact, TestPriority(5)]
        public void RemoveWorks() {
            var okResult = controller.Remove("12345678901234567890567890", "Winserv022");

            Assert.IsType<OkObjectResult>(okResult);
        }
    }
}
