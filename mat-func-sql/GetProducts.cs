using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Collections.Generic;
using mat_func_sql.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.FileSystemGlobbing;
using static System.Net.WebRequestMethods;

namespace mat_func_sql
{
    public static class GetProducts
    {
        [FunctionName("GetProducts")]
        public static async Task<IActionResult> RunProducts(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {

            List<Product> products = new List<Product>();
            var query = "select ProductID, productName,Quandity from Products";

            SqlConnection conn = GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand(query, conn);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Product product = new Product()
                    {
                        ProductID = reader.GetInt32(0),
                        ProductName = reader.GetString(1),
                        Quandity = reader.GetInt32(2)
                    };
                    products.Add(product);
                }
            }
            conn.Close();
            return new OkObjectResult(products);
        }

        private static SqlConnection GetConnection()
        {
            // string connectionstring = "Server=tcp:mathews.database.windows.net,1433;Initial Catalog=mathews;Persist Security Info=False;User ID=matuser;Password=M@thews1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            string connectionstring = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_SQLConnection");
            
            return new SqlConnection(connectionstring);
        }

        [FunctionName("GetProductById")]
        public static async Task<IActionResult> RunProductById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var prductid = int.Parse(req.Query["id"]);
            var query = string.Format("select ProductID, productName,Quandity from Products where ProductID = {0}", prductid);


            try
            {
                SqlConnection conn = GetConnection();
                conn.Open();

                SqlCommand cmd = new SqlCommand(query, conn);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Product product = new Product();
                    reader.Read();
                    product.ProductID = reader.GetInt32(0);
                    product.ProductName = reader.GetString(1);
                    product.Quandity = reader.GetInt32(2);

                    var response = product;
                    conn.Close();
                    return new OkObjectResult(response);
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                //conn.Close();
                return new OkObjectResult(message);
            }
        }
    }
}
