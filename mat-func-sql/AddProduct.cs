using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using mat_func_sql.Models;
using System.Data.SqlClient;
using System.Data;

namespace mat_func_sql
{
    public static class AddProduct
    {
        [FunctionName("AddProduct")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Product product = JsonConvert.DeserializeObject<Product>(requestBody);

            SqlConnection conn = GetConnection();
            conn.Open();
            var query = "INSERT INTO Products (ProductID, ProductName, Quandity) VALUES(@param1,@param2,@param3)";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@param1", SqlDbType.Int).Value = product.ProductID;
                cmd.Parameters.Add("@param2", SqlDbType.VarChar, 200).Value = product.ProductName;
                cmd.Parameters.Add("@param3", SqlDbType.Int).Value = product.Quandity;

                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
          

            return new OkObjectResult(product);
        }

        private static SqlConnection GetConnection()
        {
            // string connectionstring = "Server=tcp:mathews.database.windows.net,1433;Initial Catalog=mathews;Persist Security Info=False;User ID=matuser;Password=M@thews1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            string connectionstring = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_SQLConnection");

            return new SqlConnection(connectionstring);
        }


    }
}
