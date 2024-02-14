using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MySql.Data.MySqlClient;

namespace _555;

public partial class E_Toba : Window
{
    private Toba _tobaForm;
    private Products _selectedProduct;
    private string _connString = "server=localhost;database=Kurs;port=3306;User Id=sammy;password=Andr%123";
    public E_Toba(Toba tobaForm, Products selectedProduct)
{
    _tobaForm = tobaForm;
    _selectedProduct = selectedProduct;
    InitializeComponent();

    // Заполняем поля формы данными выбранного продукта
    NameTextBox.Text = _selectedProduct.Name;
    DescriptionTextBox.Text = _selectedProduct.Description;
    QuantityAvailableTextBox.Text = _selectedProduct.QuantityAvailable.ToString();
    PriceTextBox.Text = _selectedProduct.Price.ToString();
    CategoryComboBox.ItemsSource = GetCategories();
    CategoryComboBox.SelectedItem = _selectedProduct.CategoryName;
}

    private List<string> GetCategories()
    {
        List<string> categories = new List<string>();

        using (MySqlConnection connection = new MySqlConnection(_connString))
        {
            connection.Open();

            string sql = "SELECT DISTINCT Name FROM Categories";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    categories.Add(reader.GetString("Name"));
                }
            }
        }

        return categories;
    }

private void UpdateProduct(int productId, string productName, string description, int quantityAvailable, double price, string categoryName)
{
    int categoryId = GetCategoryId(categoryName);

    if (categoryId != -1)
    {
        using (MySqlConnection connection = new MySqlConnection(_connString))
        using (MySqlCommand command = connection.CreateCommand())
        {
            connection.Open();

            command.CommandText = "UPDATE Products " +
                                  "SET Name = @ProductName, " +
                                  "Description = @Description, " +
                                  "QuantityAvailable = @QuantityAvailable, " +
                                  "Price = @Price, " +
                                  "Category_id = @CategoryID " +
                                  "WHERE ProductID = @ProductID";

            command.Parameters.AddWithValue("@ProductID", productId);
            command.Parameters.AddWithValue("@ProductName", productName);
            command.Parameters.AddWithValue("@Description", description);
            command.Parameters.AddWithValue("@QuantityAvailable", quantityAvailable);
            command.Parameters.AddWithValue("@Price", price);
            command.Parameters.AddWithValue("@CategoryID", categoryId);

            command.ExecuteNonQuery();
        }

        _tobaForm.ShowTable();
        Close();
    }
    else
    {
        Console.WriteLine("Не удалось получить CategoryID");
    }
}

private int GetCategoryId(string categoryName)
{
    using (MySqlConnection connection = new MySqlConnection(_connString))
    {
        connection.Open();

        string sqlSelect = "SELECT CategoryID FROM Categories WHERE Name = @CategoryName";

        using (MySqlCommand command = new MySqlCommand(sqlSelect, connection))
        {
            command.Parameters.AddWithValue("@CategoryName", categoryName);

            object result = command.ExecuteScalar();

            return (result != null) ? Convert.ToInt32(result) : -1;
        }
    }
}

private void SaveChanges_Click(object? sender, RoutedEventArgs e)
{
    try
    {
        int productId = _selectedProduct.ProductID;
        string productName = NameTextBox.Text;
        string description = DescriptionTextBox.Text;
        int quantityAvailable = int.Parse(QuantityAvailableTextBox.Text);
        double price = double.Parse(PriceTextBox.Text);
        string categoryName = CategoryComboBox.SelectedItem?.ToString();

        UpdateProduct(productId, productName, description, quantityAvailable, price, categoryName);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при сохранении изменений: {ex.Message}");
    }
}
}