using ChurrascoChallenge.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace ChurrascoChallenge.Controllers
{
    public class ProductsController : Controller
    {
        private readonly MySqlConnection _dbConnection;

        public ProductsController(MySqlConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = new List<Product>();
            string query = "SELECT * FROM products";

            try
            {
                await _dbConnection.OpenAsync();
                using (var cmd = new MySqlCommand(query, _dbConnection))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            Id = reader.GetInt32("id"),
                            SKU = reader.GetInt32("SKU"),
                            Name = reader.GetString("name"),
                            Picture = reader.GetString("picture"),
                            Price = reader.GetDecimal("price"),
                            Currency = reader.GetString("currency"),
                            Code = reader.IsDBNull(reader.GetOrdinal("code")) ? (int?)null : reader.GetInt32("code"),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description")
                        });
                    }
                }
            }
            catch (MySqlException ex)
            {
                ModelState.AddModelError(string.Empty, " ERROR " + ex.Message);
            }
            finally
            {
                await _dbConnection.CloseAsync();
            }

            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product model)
        {
            if (ModelState.IsValid)
            {
                string query = "INSERT INTO products (SKU, name, picture, price, currency, code, description) VALUES (@SKU, @Name, @Picture, @Price, @Currency, @Code, @Description)";

                try
                {
                    using (var cmd = new MySqlCommand(query, _dbConnection))
                    {
                        cmd.Parameters.AddWithValue("@SKU", model.SKU);
                        cmd.Parameters.AddWithValue("@Name", model.Name);
                        cmd.Parameters.AddWithValue("@Picture", model.Picture);
                        cmd.Parameters.AddWithValue("@Price", model.Price);
                        cmd.Parameters.AddWithValue("@Currency", model.Currency);
                        cmd.Parameters.AddWithValue("@Code", model.Code.HasValue ? (object)model.Code.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@Description", model.Description ?? (object)DBNull.Value);

                        await _dbConnection.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }

                    return RedirectToAction("Index");
                }
                catch (MySqlException ex)
                {
                    ModelState.AddModelError(string.Empty, "ERROR" + ex.Message);
                }
                finally
                {
                    await _dbConnection.CloseAsync();
                }
            }           
            return View(model);
        }
    }
}
