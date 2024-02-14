using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MySql.Data.MySqlClient;

namespace _555;

public partial class D_Toba : Window
{
    private Toba _tobaForm;
    private string _connString = "server=localhost;database=Kurs;port=3306;User Id=sammy;password=Andr%123";
    public D_Toba(Toba tobaForm)
    {
        InitializeComponent();
        
        _tobaForm = tobaForm;

        // Заполните ComboBox данными
        CategoryComboBox.ItemsSource = GetCategories();
        CategoryComboBox.SelectedIndex = 0;
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

    private void AddProducts_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                CategoryComboBox.SelectedItem == null)
            {
                Console.WriteLine("Please fill in all fields.");
                return;
            }

            int categoryId = GetOrCreateCategoryId(CategoryComboBox.SelectedItem?.ToString());

            if (categoryId != -1)
            {
                InsertProduct(NameTextBox.Text, DescriptionTextBox.Text, double.Parse(PriceTextBox.Text), int.Parse(QuantityAvailableTextBox.Text), categoryId);
                _tobaForm.ShowTable();
                Close();
            }
            else
            {
                Console.WriteLine("Failed to get CategoryID");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding product: {ex.Message}");
        }
    }

    private int GetOrCreateCategoryId(string categoryName)
    {
        using (MySqlConnection connection = new MySqlConnection(_connString))
        {
            connection.Open();

            string sqlSelect = "SELECT CategoryID FROM Categories WHERE Name = @Name";
            string sqlInsert = "INSERT INTO Categories (Name) VALUES (@Name); SELECT LAST_INSERT_ID();";

            using (MySqlCommand command = new MySqlCommand(sqlSelect, connection))
            {
                command.Parameters.AddWithValue("@Name", categoryName);

                object result = command.ExecuteScalar();

                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    command.CommandText = sqlInsert;
                    result = command.ExecuteScalar();

                    return (result != null) ? Convert.ToInt32(result) : -1;
                }
            }
        }
    }

    private void InsertProduct(string productName, string description, double price, int quantityAvailable, int categoryId)
    {
        string sql = "INSERT INTO Products (Name, Description, Price, QuantityAvailable, Category_id) " +
                     "VALUES (@Name, @Description, @Price, @QuantityAvailable, @Category_id)";

        using (MySqlConnection connection = new MySqlConnection(_connString))
        using (MySqlCommand command = new MySqlCommand(sql, connection))
        {
            connection.Open();

            command.Parameters.AddWithValue("@Name", productName);
            command.Parameters.AddWithValue("@Description", description);
            command.Parameters.AddWithValue("@Price", price);
            command.Parameters.AddWithValue("@QuantityAvailable", quantityAvailable);
            command.Parameters.AddWithValue("@Category_id", categoryId);

            command.ExecuteNonQuery();
        }
    }
}