using System;
using System.Windows;
using ElectroStore.DataAccess;
using ElectroStore.Models;
using System.Collections.Generic;

namespace ElectroStore.Views
{
    public partial class ProductWindow : Window
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();
        private Product product; // Может быть null, если это добавление нового товара
        private List<Category> categories;

        public ProductWindow()
        {
            InitializeComponent();
            LoadCategories();
        }

        public ProductWindow(Product product) : this() // Конструктор для редактирования существующего товара
        {
            this.product = product;
            LoadCategories();
            FillProductData();
        }

        private void LoadCategories()
        {
            categories = dbHelper.GetAllCategories();
            CategoryComboBox.ItemsSource = categories;
            CategoryComboBox.DisplayMemberPath = "CategoryName";

        }

        private void FillProductData()
        {
            ProductNameTextBox.Text = product.ProductName;
            DescriptionTextBox.Text = product.Description;
            PriceTextBox.Text = product.Price.ToString();
            QuantityTextBox.Text = product.Quantity.ToString();
            ImageURLTextBox.Text = product.ImageURL;

            // Выбор категории в ComboBox
            foreach (Category category in categories)
            {
                if (category.CategoryID == product.CategoryID)
                {
                    CategoryComboBox.SelectedItem = category;
                    break;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string productName = ProductNameTextBox.Text;
                string description = DescriptionTextBox.Text;
                double price = Convert.ToDouble(PriceTextBox.Text);
                int quantity = Convert.ToInt32(QuantityTextBox.Text);
                string imageURL = ImageURLTextBox.Text;
                Category selectedCategory = CategoryComboBox.SelectedItem as Category;

                if (selectedCategory == null)
                {
                    MessageBox.Show("Пожалуйста, выберите категорию.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (product == null)
                {
                    // Добавление нового товара
                    Product newProduct = new Product
                    {
                        ProductName = productName,
                        CategoryID = selectedCategory.CategoryID,
                        Description = description,
                        Price = price,
                        Quantity = quantity,
                        ImageURL = imageURL
                    };
                    dbHelper.AddProduct(newProduct);
                    MessageBox.Show("Товар успешно добавлен.");
                }
                else
                {
                    // Обновление существующего товара
                    product.ProductName = productName;
                    product.CategoryID = selectedCategory.CategoryID;
                    product.Description = description;
                    product.Price = price;
                    product.Quantity = quantity;
                    product.ImageURL = imageURL;

                    dbHelper.UpdateProduct(product);
                    MessageBox.Show("Товар успешно обновлен.");
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}