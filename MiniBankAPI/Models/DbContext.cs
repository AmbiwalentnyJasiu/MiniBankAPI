using System.Data.SqlClient;

namespace MiniBankAPI.Models {
    public class DbContext {
        public SqlConnection dataContext;
        public DbContext() {
            dataContext = new SqlConnection(@"
                Data Source =.;
                Initial Catalog = JanekCentrala;
                Integrated Security = True");
            dataContext.Open();
        }

        ~DbContext() {
            dataContext.Close();
        }
    }
}
