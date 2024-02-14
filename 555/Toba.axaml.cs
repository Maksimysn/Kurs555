using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MySql.Data.MySqlClient;

namespace _555;

public partial class Toba : Window
{
    private Products _selectedProducts;
    private MySqlConnection _connection;
    private List<Products> _products;
    private string _connString = "server=localhost;database=Kurs;port=3306;User Id=sammy;password=Andr%123";
    public Toba()
    {
        InitializeComponent();
        
        ShowTable();
        _connection = new MySqlConnection(_connString);
        ProductsGrid.SelectionChanged += ProductsGrid_SelectionChanged;
        SearchTextBox.TextChanged += SearchTextBox_TextChanged;

        CategoryComboBox.SelectionChanged += CategoryComboBox_SelectionChanged;
        CategoryComboBox.ItemsSource = GetCategories();
    }

    private List<string> GetCategories()
    {
        List<string> categories = new List<string>();
    
        using (MySqlConnection connection = new MySqlConnection(_connString))
        {
            connection.Open();

            string sql = "SELECT DISTINCT Name FROM Categories";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                categories.Add(reader.GetString("Name"));
            }

            reader.Close();
        }
    
        return categories;
    }

    
    public void ShowTable()
    {
        string sql = "SELECT Products.ProductID, Products.Name, Products.Description, Products.Price, Products.QuantityAvailable, Categories.Name AS CategoryName " +
                     "FROM Products " +
                     "JOIN Categories ON Products.Category_id = Categories.CategoryID";

        _products = new List<Products>(); // Инициализируем коллекцию _products здесь

        using (MySqlConnection connection = new MySqlConnection(_connString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var currentProduct = new Products
                    {
                        ProductID = reader.GetInt32("ProductID"),
                        Name = reader.GetString("Name"),
                        Description = reader.GetString("Description"),
                        Price = reader.GetDouble("Price"),
                        QuantityAvailable = reader.GetInt32("QuantityAvailable"),
                        CategoryName = reader.GetString("CategoryName")
                    };

                    _products.Add(currentProduct); // Добавляем продукт в коллекцию _products
                }
            }
        }

        ProductsGrid.ItemsSource = _products; // Устанавливаем источник данных для DataGrid
    }


    private void FilterByCategory(string category)
    {
        string sql = "SELECT Products.ProductID, Products.Name, Products.Description, Products.Price, Products.QuantityAvailable, Categories.Name AS CategoryName " +
                     "FROM Products " +
                     "JOIN Categories ON Products.Category_id = Categories.CategoryID " +
                     "WHERE Categories.Name = @CategoryName";

        List<Products> filteredProducts = new List<Products>();

        using (MySqlConnection connection = new MySqlConnection(_connString))
        {
            connection.Open();

            MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@CategoryName", category);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var currentProduct = new Products
                    {
                        ProductID = reader.GetInt32("ProductID"),
                        Name = reader.GetString("Name"),
                        Description = reader.GetString("Description"),
                        Price = reader.GetDouble("Price"),
                        QuantityAvailable = reader.GetInt32("QuantityAvailable"),
                        CategoryName = reader.GetString("CategoryName")
                    };

                    filteredProducts.Add(currentProduct);
                }
            }
        }

        ProductsGrid.ItemsSource = filteredProducts;
    }

    
    
    private void ProductsGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
         if (ProductsGrid.SelectedItem is Products selectedProducts)
         {
             _selectedProducts = selectedProducts;
         }
    }

    private void OpenNextForm_Click(object? sender, RoutedEventArgs e)
    {
        var toba_DForm = new D_Toba(this);
        toba_DForm.Show();
    }

    private void DeleteProductsGrid_Click(object? sender, RoutedEventArgs e)
    {
        if (_selectedProducts != null)
        {
            DeleteProducts(_selectedProducts.ProductID);
        }
    }

    private void DeleteProducts(int productID)
    {
        using (_connection = new MySqlConnection(_connString))
        {
            _connection.Open();
            string queryString = $"DELETE FROM Products WHERE ProductID = {productID}";
            MySqlCommand command = new MySqlCommand(queryString, _connection);
            command.ExecuteNonQuery();
        }

        ShowTable();
    }

    private void EditProductsGrid_Click(object? sender, RoutedEventArgs e)
    {
        if (_selectedProducts != null)
        {
            // Открываем окно редактирования с выбранными данными
            var editForm = new E_Toba(this, _selectedProducts);
            editForm.Show();
        }
        else
        {
            Console.WriteLine("Выберите продукт для редактирования");
        }
    }

    private void CategoryComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (CategoryComboBox.SelectedItem is string selectedCtegory)
        {
            FilterByCategory(selectedCtegory);
        }
    }

    private void ResetFilterButton_Click(object? sender, RoutedEventArgs e)
    {
        // Сбросить фильтр и отобразить полную таблицу
         ShowTable();
         
         // Очистить выбор в ComboBox
         CategoryComboBox.SelectedItem = null;
    }
    
    private void SearchTextBox_TextChanged(object sender, RoutedEventArgs e)
    {
        // Trim - это метод, который удаляет пробелы или другие указанные символы в начале и конце строки
        string searchQuery = SearchTextBox.Text.Trim();
        if (!string.IsNullOrEmpty(searchQuery))
        {
            // Выполните поиск по наименованию товара и цене и обновите данные в DataGrid
            SearchAndRefreshTable(searchQuery);
        }
        else
        {
            // Если поле поиска пусто, отобразите все данные
            ShowTable();
        }
    }

    private void SearchAndRefreshTable(string searchQuery)
    {
        if (_products == null || _products.Count == 0)
        {
            return;
        }

        searchQuery = searchQuery.ToLower();

        // Фильтруем коллекцию _products по критериям поиска
        var filteredProducts = _products
            .Where(p =>
                p.Name.ToLower().Contains(searchQuery) ||
                p.Price.ToString().Contains(searchQuery))
            .ToList();

        // Обновляем отображаемые данные в DataGrid
        ProductsGrid.ItemsSource = filteredProducts;
    }

    
    
    
}