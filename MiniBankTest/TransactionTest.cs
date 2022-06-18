using Microsoft.AspNetCore.Mvc;
using MiniBankAPI.Controllers;
using MiniBankAPI.Interfaces;
using MiniBankAPI.Models;
using MiniBankAPI.Services;
using System.Collections.Generic;
using Xunit;

namespace MiniBankTest {
    [TestCaseOrderer("MiniBankTest.PriorityOrderer", "MiniBankTest")]
    public class TransactionTest {
        private IDbService dbService;
        private TransactionController controller;

        public TransactionTest() {
            dbService = new DbService();
            controller = new TransactionController(dbService);
        }

        [Fact, TestPriority(-5)]
        public void GetReturnsAllItems() {
            var okResult = controller.Get() as OkObjectResult;

            var items = Assert.IsType<List<TransactionModel>>(okResult.Value);
            Assert.Equal(5, items.Count);
        }

        [Fact, TestPriority(-5)]
        public void GetFromServerReturnsAllItems() {
            var okResult = controller.GetFromServer("WINSERV019") as OkObjectResult;

            var items = Assert.IsType<List<TransactionModel>>(okResult.Value);
            Assert.Equal(2, items.Count);
        }

        [Fact, TestPriority(-5)]
        public void GetByNumberReturnsCorrectItemsCount() {
            var okResult = controller.Get("90432132142143143243213214") as OkObjectResult;

            var item = Assert.IsType<List<TransactionModel>>(okResult.Value);
            Assert.Single(item);
        }

        [Fact, TestPriority(0)]
        public void PostReturnsCreatedItem() {
            var insert = new TransactionModel() { SenderID = "90432132142143143243213214", Amount = 20.20, DateSent = "12/21/2020", ReceiverID = "81121223233434454556566161" };
            var okResult = controller.Post(insert) as OkObjectResult;

            var item = Assert.IsType<TransactionModel>(okResult.Value);
            Assert.Equal("81121223233434454556566161", item.ReceiverID);
        }
    }
}
