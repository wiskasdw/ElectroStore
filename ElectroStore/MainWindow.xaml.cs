using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ElectroStore.DataAccess;
using ElectroStore.Models;
using ElectroStore.Views;
using System;

namespace ElectroStore.Views
{
    public partial class MainWindow : Window
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();
        private List<Product> products;
        private List<Category> categories;

        public MainWindow()
        {
            InitializeComponent();
            LoadCategories();
            LoadProducts();
        }

        private void LoadCategories()
        {
            categories = dbHelper.GetAllCategories();
            CategoryListBox.ItemsSource = categories;
            CategoryListBox.DisplayMemberPath = "CategoryName";
        }

        private void LoadProducts()
        {
            products = dbHelper.GetAllProducts();
            ProductListView.ItemsSource = products;
        }

        private void CategoryListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryListBox.SelectedItem is Category selectedCategory)
            {
                products = dbHelper.GetProductsByCategory(selectedCategory.CategoryID);
                ProductListView.ItemsSource = products;
            }
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            ProductWindow productWindow = new ProductWindow();
            if (productWindow.ShowDialog() == true)
            {
                LoadProducts(); // Обновляем список товаров
            }
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int productIdToDelete = (int)button.CommandParameter;

            MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить товар с ID {productIdToDelete}?",
                                                        "Подтверждение удаления",
                                                        MessageBoxButton.YesNo,
                                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    dbHelper.DeleteProduct(productIdToDelete);
                    LoadProducts(); // Обновляем список товаров
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}