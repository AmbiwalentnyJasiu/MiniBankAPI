using System.Data.Linq;

namespace MiniBankAPI {
    public class DbService : IDbService {
        private DataContext ctx;

        public DbService() {
            ctx = new DbContext().dataContext;
        }

        public DataContext Get() {
            return ctx;
        }
    }
}
