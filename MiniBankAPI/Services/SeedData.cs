using MiniBankAPI.Interfaces;
using System;
using System.Data.SqlClient;

namespace MiniBankAPI.Services {
    public static class SeedData {
        public static void Init(IDbService dbService) {
            var conn = dbService.Get();

            var seed = new SqlCommand("EXEC dbo.Initialize", conn);

            try {
                seed.ExecuteNonQuery();
            } catch (Exception e){
                Console.WriteLine(e.Message);
            }
        }
    }
}
