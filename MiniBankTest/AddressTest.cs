using Microsoft.AspNetCore.Mvc;
using MiniBankAPI.Controllers;
using MiniBankAPI.Interfaces;
using MiniBankAPI.Models;
using MiniBankAPI.Services;
using System.Collections.Generic;
using Xunit;

namespace MiniBankTest {

    [TestCaseOrderer("MiniBankTest.PriorityOrderer", "MiniBankTest")]
    public class AddressTest {
        private IDbService dbService;
        private AddressInfoController controller;

        public AddressTest() {
            dbService = new DbService();
            controller = new AddressInfoController(dbService);
        }

        [Fact, TestPriority(-5)]
        public void GetReturnsAllItems() {
            var okResult = controller.Get() as OkObjectResult;

            var items = Assert.IsType<List<AddressInfoModel>>(okResult.Value);
            Assert.Equal(5, items.Count);
        }

        [Fact, TestPriority(-5)]
        public void GetFromServerReturnsAllItems() {
            var okResult = controller.GetFromServer("WINSERV019") as OkObjectResult;

            var items = Assert.IsType<List<AddressInfoModel>>(okResult.Value);
            Assert.Equal(2, items.Count);
        }

        [Fact, TestPriority(-5)]
        public void GetByNumberReturnsCorrectItem() {
            var okResult = controller.Get(1) as OkObjectResult;

            var item = Assert.IsType<AddressInfoModel>(okResult.Value);
            Assert.Equal("Reymonta", item.Street);
        }

        [Fact, TestPriority(0)]
        public void PostReturnsCreatedItem() {
            var insert = new AddressInfoModel() { PostalCode = "11-112", City = "Rzeszów", Street = "Gdzieś", BuildingNumber = "123D", FlatNumber = "345F" };
            var okResult = controller.Post(insert, "WINSERV022") as OkObjectResult;

            var item = Assert.IsType<AddressInfoModel>(okResult.Value);
            Assert.Equal("11-112", item.PostalCode);
        }

        [Fact, TestPriority(5)]
        public void RemoveWorks() {
            var okResult = controller.Remove(6, "Winserv022");

            Assert.IsType<OkObjectResult>(okResult);
        }
    }
}
