using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using PriceTracker.Models;

namespace PriceTracker.Controllers
{
    public class ProductsInformationController : Controller
    {
        private readonly string _connectionString = "User Id=postgres;Password=admin;Host=localhost;Port=5432;Database=PriceTracker";
        private readonly NpgsqlConnection _connection;

        public ProductsInformationController()
        {
            _connection = new NpgsqlConnection(_connectionString);
        }

        [HttpGet]
        public IActionResult NewInformation(int productId)
        {
            ProductInformation productInformation = new ProductInformation
            {
                ProductId = productId
            };

            return View(productInformation);
        }

       [HttpPost]
       public async Task<IActionResult> NewInformation(ProductInformation productInformation)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string insertQuery = "INSERT INTO ProductInformation(ProductId, Value, Day, Month, Year) VALUES(@ProductId, @Value, @Day, @Month, @Year)";
                    NpgsqlCommand cmd = new NpgsqlCommand(insertQuery, _connection);
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@ProductId", productInformation.ProductId);
                    cmd.Parameters.AddWithValue("@Value", productInformation.Value);
                    cmd.Parameters.AddWithValue("@Day", productInformation.Day);
                    cmd.Parameters.AddWithValue("@Month", productInformation.Month);
                    cmd.Parameters.AddWithValue("@Year", productInformation.Year);

                    await _connection.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    return RedirectToAction("DetailsProduct", "Products", new { productId = productInformation.ProductId });
                }

                return View(productInformation);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
    }
}
