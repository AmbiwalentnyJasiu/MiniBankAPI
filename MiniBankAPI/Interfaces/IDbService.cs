using System.Collections.Generic;
using System.Data.SqlClient;

namespace MiniBankAPI.Interfaces {
    public interface IDbService {
        SqlConnection Get();

        List<string> Servers();
    }
}
