using MiniBankAPI.Interfaces;
using MiniBankAPI.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MiniBankAPI.Services {
    public class DbService : IDbService {
        private readonly SqlConnection ctx;
        private readonly List<string> servers;

        public DbService() {
            ctx = new DbContext().dataContext;
            servers = new List<string> { "WINSERV019", "WINSERV022" };
        }

        public SqlConnection Get() {
            return ctx;
        }

        public List<string> Servers() {
            return servers;
        }
    }
}
