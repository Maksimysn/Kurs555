namespace _555;

public class Products
{
    // Свойство для хранения уникального идентификатора продукта
    public int ProductID { get; set; }

    // Свойство для хранения названия продукта
    public string Name { get; set; }

    // Свойство для хранения описания продукта
    public string Description { get; set; }

    // Свойство для хранения цены продукта
    public double Price { get; set; }

    // Свойство для хранения количества товара в наличии
    public int QuantityAvailable { get; set; }

  

    // Свойство для хранения названия категории товара
    public string CategoryName { get; set; }
}
