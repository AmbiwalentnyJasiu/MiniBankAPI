using System.Data.Linq;

namespace MiniBankAPI {
    public class DbContext {
        public DataContext dataContext;
        public DbContext() {
            dataContext = new DataContext(@"
                Data Source =.;
                Initial Catalog = JanekCentrala;
                Integrated Security = True");
        }
    }
}
