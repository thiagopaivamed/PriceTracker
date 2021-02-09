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
    public class ProductsController : Controller
    {
        private readonly string _connectionString = "User Id=postgres;Password=admin;Host=localhost;Port=5432;Database=PriceTracker";
        private readonly NpgsqlConnection _connection;

        public ProductsController()
        {
            _connection = new NpgsqlConnection(_connectionString);
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                List<Product> products = new List<Product>();
                string selectQuery = "SELECT * FROM Product";
                NpgsqlCommand cmd = new NpgsqlCommand(selectQuery, _connection);

                await _connection.OpenAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

                while(await reader.ReadAsync())
                {
                    Product product = new Product();
                    product.ProductId = Convert.ToInt32(reader["ProductId"]);
                    product.ProductName = reader["ProductName"].ToString();
                    product.ProductLink = reader["ProductLink"].ToString();
                    products.Add(product);
                }

                return View(products);
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

        [HttpGet]
        public IActionResult NewProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewProduct(Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string insertQuery = "INSERT INTO Product(ProductName, ProductLink) VALUES(@ProductName, @ProductLink)";
                    NpgsqlCommand cmd = new NpgsqlCommand(insertQuery, _connection);
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                    cmd.Parameters.AddWithValue("@ProductLink", product.ProductLink);

                    await _connection.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    return RedirectToAction(nameof(Index));
                }

                return View(product);
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

        [HttpGet]
        public async Task<IActionResult> UpdateProduct(int productId)
        {
            try
            {
                Product product = new Product();
                string selectQuery = String.Format("SELECT * FROM Product WHERE ProductId = {0}", productId);
                NpgsqlCommand cmd = new NpgsqlCommand(selectQuery, _connection);

                await _connection.OpenAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

                while(await reader.ReadAsync())
                {
                    product.ProductId = Convert.ToInt32(reader["ProductId"]);
                    product.ProductName = reader["ProductName"].ToString();
                    product.ProductLink = reader["ProductLink"].ToString();
                }

                return View(product);
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

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(int productId, Product product)
        {
            try
            {
                if (productId != product.ProductId)
                    return NotFound();

                if (ModelState.IsValid)
                {
                    string updateQuery = "UPDATE Product SET ProductName = @ProductName, ProductLink = @ProductLink WHERE ProductId = @ProductId";
                    NpgsqlCommand cmd = new NpgsqlCommand(updateQuery, _connection);
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@ProductId", product.ProductId);
                    cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                    cmd.Parameters.AddWithValue("@ProductLink", product.ProductLink);

                    await _connection.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    return RedirectToAction(nameof(Index));
                }

                return View(product);
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

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            try
            {
                string deleteQuery = "DELETE FROM Product WHERE ProductId = @ProductId";
                NpgsqlCommand cmd = new NpgsqlCommand(deleteQuery, _connection);

                cmd.Parameters.AddWithValue("@ProductId", productId);

                await _connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return RedirectToAction(nameof(Index));
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

        public async Task<IActionResult> DetailsProduct(int productId)
        {
            try
            {
                ViewData["ProductId"] = productId;

                List<ProductInformation> productsInformation = new List<ProductInformation>();
                string selectQuery = String.Format("SELECT Value, Day, Month, Year FROM ProductInformation WHERE ProductId = {0}", productId);

                NpgsqlCommand cmd = new NpgsqlCommand(selectQuery, _connection);

                await _connection.OpenAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

                while(await reader.ReadAsync())
                {
                    ProductInformation productInformation = new ProductInformation();
                    productInformation.Value = Convert.ToDouble(reader["Value"]);
                    productInformation.Day = Convert.ToInt32(reader["Day"]);
                    productInformation.Month = Convert.ToInt32(reader["Month"]);
                    productInformation.Year = Convert.ToInt32(reader["Year"]);

                    productsInformation.Add(productInformation);
                }

                return View(productsInformation);
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

        public async Task<JsonResult> GraphicData(int productId, int startDay, int endDay, int month, int year)
        {
            try
            {
                List<ProductInformation> productsInformation = new List<ProductInformation>();
                string selectQuery = String.Format("SELECT Value, Day, Month, Year " +
                    "FROM ProductInformation " +
                    "WHERE ProductId = {0} AND Year = {1} AND Month = {2} AND (Day >= {3} AND Day <= {4})", productId, year, month, startDay, endDay);
                NpgsqlCommand cmd = new NpgsqlCommand(selectQuery, _connection);

                await _connection.OpenAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

                while(await reader.ReadAsync())
                {
                    ProductInformation productInformation = new ProductInformation();
                    productInformation.Value = Convert.ToDouble(reader["Value"]);
                    productInformation.Day = Convert.ToInt32(reader["Day"]);
                    productInformation.Month = Convert.ToInt32(reader["Month"]);
                    productInformation.Year = Convert.ToInt32(reader["Year"]);

                    productsInformation.Add(productInformation);
                }

                return Json(productsInformation);
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
