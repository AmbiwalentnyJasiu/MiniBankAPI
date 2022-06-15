using System.Data.Linq;

namespace MiniBankAPI {
    public interface IDbService {
        DataContext Get();
    }
}
