using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using ElectroStore.Models;

namespace ElectroStore.DataAccess
{
    public class DatabaseHelper
    {
        private string dbPath = "ElectroStore.db"; // Имя файла базы данных

        public DatabaseHelper()
        {
            // Создаем базу данных, если она не существует
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
                CreateTables();
            }
        }

        private SQLiteConnection GetConnection()
        {
            SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            connection.Open();
            return connection;
        }

        private void CreateTables()
        {
            using (SQLiteConnection connection = GetConnection())
            {
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    // Создание таблицы Categories
                    command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Categories (
                            CategoryID INTEGER PRIMARY KEY AUTOINCREMENT,
                            CategoryName TEXT NOT NULL
                        );
                    ";
                    command.ExecuteNonQuery();

                    // Создание таблицы Products
                    command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Products (
                            ProductID INTEGER PRIMARY KEY AUTOINCREMENT,
                            ProductName TEXT NOT NULL,
                            CategoryID INTEGER,
                            Description TEXT,
                            Price REAL,
                            Quantity INTEGER,
                            ImageURL TEXT,
                            FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
                        );
                    ";
                    command.ExecuteNonQuery();

                    // Добавление тестовых категорий (пример)
                    command.CommandText = "INSERT INTO Categories (CategoryName) SELECT 'Холодильники' WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = 'Холодильники');";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO Categories (CategoryName) SELECT 'Стиральные машины' WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = 'Стиральные машины');";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO Categories (CategoryName) SELECT 'Телевизоры' WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = 'Телевизоры');";
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();
            using (SQLiteConnection connection = GetConnection())
            {
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Products", connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product
                            {
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                ProductName = reader["ProductName"].ToString(),
                                CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                Description = reader["Description"].ToString(),
                                Price = Convert.ToDouble(reader["Price"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                ImageURL = reader["ImageURL"].ToString()
                            };
                            products.Add(product);
                        }
                    }
                }
            }
            return products;
        }

        public List<Category> GetAllCategories()
        {
            List<Category> categories = new List<Category>();
            using (SQLiteConnection connection = GetConnection())
            {
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Categories", connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Category category = new Category
                            {
                                CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                CategoryName = reader["CategoryName"].ToString()
                            };
                            categories.Add(category);
                        }
                    }
                }
            }
            return categories;
        }

        public void AddProduct(Product product)
        {
            using (SQLiteConnection connection = GetConnection())
            {
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        INSERT INTO Products (ProductName, CategoryID, Description, Price, Quantity, ImageURL)
                        VALUES (@ProductName, @CategoryID, @Description, @Price, @Quantity, @ImageURL);
                    ";
                    command.Parameters.AddWithValue("@ProductName", product.ProductName);
                    command.Parameters.AddWithValue("@CategoryID", product.CategoryID);
                    command.Parameters.AddWithValue("@Description", product.Description);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@Quantity", product.Quantity);
                    command.Parameters.AddWithValue("@ImageURL", product.ImageURL);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateProduct(Product product)
        {
            using (SQLiteConnection connection = GetConnection())
            {
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        UPDATE Products
                        SET ProductName = @ProductName,
                            CategoryID = @CategoryID,
                            Description = @Description,
                            Price = @Price,
                            Quantity = @Quantity,
                            ImageURL = @ImageURL
                        WHERE ProductID = @ProductID;
                    ";
                    command.Parameters.AddWithValue("@ProductID", product.ProductID);
                    command.Parameters.AddWithValue("@ProductName", product.ProductName);
                    command.Parameters.AddWithValue("@CategoryID", product.CategoryID);
                    command.Parameters.AddWithValue("@Description", product.Description);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@Quantity", product.Quantity);
                    command.Parameters.AddWithValue("@ImageURL", product.ImageURL);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteProduct(int productId)
        {
            using (SQLiteConnection connection = GetConnection())
            {
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "DELETE FROM Products WHERE ProductID = @ProductID;";
                    command.Parameters.AddWithValue("@ProductID", productId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Product> GetProductsByCategory(int categoryId)
        {
            List<Product> products = new List<Product>();
            using (SQLiteConnection connection = GetConnection())
            {
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Products WHERE CategoryID = @CategoryID", connection))
                {
                    command.Parameters.AddWithValue("@CategoryID", categoryId);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product
                            {
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                ProductName = reader["ProductName"].ToString(),
                                CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                Description = reader["Description"].ToString(),
                                Price = Convert.ToDouble(reader["Price"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                ImageURL = reader["ImageURL"].ToString()
                            };
                            products.Add(product);
                        }
                    }
                }
            }
            return products;
        }
    }
}