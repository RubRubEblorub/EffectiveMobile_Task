using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ILogger = NLog.ILogger;

namespace EffectiveMobileTask;

public class DataBase
{
    private const string _dataBaseFileName = "DataBase.json";
    private const string _resultFileName = "SortedOrdersList.txt";
    private static string _filePath = Environment.CurrentDirectory + Path.DirectorySeparatorChar;
    private static string _dataBase = _filePath + _dataBaseFileName;
    private static string _deliveryOrder = _filePath + _resultFileName;

    private readonly ILogger<DataBase> _logger;

    public DataBase(ILogger<DataBase> logger)
    {
        _logger = logger;
    }
    
    public void CreateDataBaseJsonFile()
    {
        if (!File.Exists(_dataBase))
        {
            File.Create(_dataBase).Dispose();
            
            //Pre-generated order list on first start up
            List<Order> orders =
            [
                new (1, RandomNumberGenerator.GetInt32(1, 99), RandomNumberGenerator.GetInt32(1, 5), DateTime.Today.ToString("yyyy-MM-dd") + " 12:00:00"),
                new (2, RandomNumberGenerator.GetInt32(1, 99), RandomNumberGenerator.GetInt32(1, 5), DateTime.Today.ToString("yyyy-MM-dd") + " 12:15:00"),
                new (3, RandomNumberGenerator.GetInt32(1, 99), RandomNumberGenerator.GetInt32(1, 5), DateTime.Today.ToString("yyyy-MM-dd") + " 13:30:00"),
                new (4, RandomNumberGenerator.GetInt32(1, 99), RandomNumberGenerator.GetInt32(1, 5), DateTime.Today.ToString("yyyy-MM-dd") + " 15:45:00"),
                new (5, RandomNumberGenerator.GetInt32(1, 99), RandomNumberGenerator.GetInt32(1, 5), DateTime.Today.ToString("yyyy-MM-dd") + " 16:10:00"),
                new (6, RandomNumberGenerator.GetInt32(1, 99), RandomNumberGenerator.GetInt32(1, 5), DateTime.Today.ToString("yyyy-MM-dd") + " 18:00:00"),
                new (7, RandomNumberGenerator.GetInt32(1, 99), RandomNumberGenerator.GetInt32(1, 5), DateTime.Today.ToString("yyyy-MM-dd") + " 18:15:00"),
                new (8, RandomNumberGenerator.GetInt32(1, 99), RandomNumberGenerator.GetInt32(1, 5), DateTime.Today.ToString("yyyy-MM-dd") + " 19:55:00"),
                new (9, RandomNumberGenerator.GetInt32(1, 99), RandomNumberGenerator.GetInt32(1, 5), DateTime.Today.ToString("yyyy-MM-dd") + " 20:20:00"),
                new (10, RandomNumberGenerator.GetInt32(1, 99), RandomNumberGenerator.GetInt32(1, 5), DateTime.Today.ToString("yyyy-MM-dd") + " 20:35:00")
            ];
        
            File.WriteAllText(_dataBase, JsonSerializer.Serialize(orders));
            
            _logger.LogDebug(1, "Database file was created at '{Path}' and filled with pre-generated list '{OrdersList}'", _dataBase, orders.ToString());
        }
    }
    
    private List<Order> FetchOrdersFromJson()
    {
        List<Order> orders = JsonSerializer.Deserialize<List<Order>>(File.ReadAllText(_dataBase));
        
        _logger.LogDebug(2, "Info from DB was deserialized in '{OrdersList}' from DB file", orders.ToString());

        return orders;
    }
    
    private void UploadUsersToJson(List<Order> orders)
    {
        var json = JsonSerializer.Serialize(orders);

        File.WriteAllText(_dataBase, json);
        
        _logger.LogDebug(3, "List '{OrdersList}' was serialized and saved in DB file: '{Path}'", orders.ToString(), _dataBase);
    }

    private void SaveOrder(Order order)
    {
        List<Order> orders = FetchOrdersFromJson();

        orders.Add(order);

        var addedOrder = JsonSerializer.Serialize(orders);

        File.WriteAllText(_dataBase, addedOrder);
        
        _logger.LogDebug(5, "New order '{Order}' was saved to DB", order);
    }
    
    public void CreateOrder()
    {
        Console.WriteLine("Enter order weight:");
        
        int weight;
        while (!int.TryParse(Console.ReadLine(), out weight))
        {
            Console.WriteLine("Please enter a valid weight number.");
        }
        
        Console.WriteLine("Choose city district for delivery:");
        foreach (var district in Order.GetCityDistricts())
        {
            Console.WriteLine($"{(int)district} – {district}");
        }
        
        int cityDistrict;
        while (!int.TryParse(Console.ReadLine(), out cityDistrict) && cityDistrict > 0 && cityDistrict <= 4)
        {
            Console.WriteLine("Please choose a district from the list.");
        }
        
        Console.WriteLine("Enter delivery date and time (final format will be yyyy-MM-dd HH:mm:ss):\n Note: use ':' as a separator sign for Time to avoid App misunderstanding");
        
        DateTime deliveryDate;
        while (!DateTime.TryParse(Console.ReadLine(), out deliveryDate))
        {
            Console.WriteLine("Please enter a valid delivery date.");
        }

        Order newOrder = new (GetLastOrderId() + 1, weight, cityDistrict, deliveryDate.ToString("yyyy-MM-dd HH:mm:ss"));
        
        _logger.LogDebug(4, "New order '{Order}' was created", newOrder);
        
        SaveOrder(newOrder);
        
        Console.WriteLine("New order was successfully created.\n" +
                          "<————————————————————>\n" +
                          "Returning to main menu...");
    }
    
    public void DeleteOrder()
    {
        List<Order> orders = FetchOrdersFromJson();
        
        PrintOrdersList(orders);
        
        Console.WriteLine("Enter order id for deletion:");
        
        int orderId;
        while (!int.TryParse(Console.ReadLine(), out orderId))
        {
            Console.WriteLine("Please, enter a valid Id number.");
        }

        Order order = orders.FirstOrDefault(u => u.GetId() == orderId);

        if (order != null)
        {
            orders.Remove(order);
            
            _logger.LogDebug(6, "Order '{Order}' was deleted from the list '{OrdersList}'", order, orders.ToString());
            
            UploadUsersToJson(orders);
            
            Console.WriteLine($"Order with id {orderId} has been deleted.");
        }
        else
        {
            Console.WriteLine($"Order with id {orderId} does not exist.");
        }
        
        Console.WriteLine("<————————————————————>\n" +
                          "Returning to main menu...");
    }

    private int GetLastOrderId()
    {
        List<Order> orders = FetchOrdersFromJson();
        
        return orders.Count == 0 ? 0 : orders.Last().GetId();
    }
    
    //For App console use
    public void PrintOrdersList()
    {
        Console.WriteLine("Orders list:");
        
        List<Order> orders = FetchOrdersFromJson();

        foreach (var order in orders)
        {
            Console.WriteLine(order);
        }
        
        Console.WriteLine("<————————————————————>");
    }
    
    //For inner use
    private void PrintOrdersList(List<Order> orders)
    {
        Console.WriteLine("Orders list:");
        
        foreach (var order in orders)
        {
            Console.WriteLine(order);
        }
        
        Console.WriteLine("<————————————————————>");
    }

    public void SortOrders()
    {
        List<Order> orders = FetchOrdersFromJson();
        
        PrintOrdersList(orders);
        
        Console.WriteLine("Select city district:");
        foreach (var district in Order.GetCityDistricts())
        {
            Console.WriteLine($"{(int)district} – {district}");
        }
        
        int _cityDistrict;
        while (!int.TryParse(Console.ReadLine(), out _cityDistrict) && _cityDistrict > 0 && _cityDistrict <= 4)
        {
            Console.WriteLine("Please choose a district from the list.");
        }
        string _cityDistrictString = Order.GetCityDistrictNameFromEnum(_cityDistrict);

        List<Order> filteredOrders = [];
        
        Console.WriteLine("Orders filtered by city district:");
        
        foreach (var order in orders)
        {
            if (order.GetCityDistrict() == _cityDistrictString)
            {
                filteredOrders.Add(order);
                Console.WriteLine(order);
            }
        }

        if (filteredOrders.Count == 0)
        {
            Console.WriteLine("No matches found, filtered orders list is empty.\n" +
                              "<————————————————————>\n" +
                              "Returning to main menu...");
            return;
        }
        
        Console.WriteLine("Enter first delivery date:");
        
        DateTime _firstDeliveryDateTime;
        while (!DateTime.TryParse(Console.ReadLine(), out _firstDeliveryDateTime))
        {
            Console.WriteLine("Please enter a valid delivery date.");
        }
        
        Console.WriteLine("Enter last delivery date (leave empty to sort within 30 minutes):");
        
        DateTime _lastDeliveryDateTime = _firstDeliveryDateTime.AddMinutes(30);
        
        string input = Console.ReadLine();
        while (!string.IsNullOrEmpty(input) && !DateTime.TryParse(input, out _lastDeliveryDateTime))
        {
            Console.WriteLine("Please enter a valid delivery date.");
            input = Console.ReadLine();
        }

        string sortedResult = string.Empty;
        foreach (var order in filteredOrders)
        {
            if (order._deliveryDateTime >= _firstDeliveryDateTime && order._deliveryDateTime <= _lastDeliveryDateTime)
            {
                sortedResult += order.ToString();
                sortedResult += "\n";
            }
        }
        
        if (sortedResult == string.Empty)
        {
            Console.WriteLine("No matches found, sorted orders list is empty.\n" +
                              "<————————————————————>\n" +
                              "Returning to main menu...");
            return;
        }

        Console.WriteLine($"Sorted orders:\n{sortedResult}");
        
        if (!File.Exists(_deliveryOrder))
        {
            File.Create(_deliveryOrder).Dispose();
            
            _logger.LogDebug(7, "File with orders sorting result was created at: '{Path}'", _deliveryOrder);
        }
        
        File.WriteAllText(_deliveryOrder, sortedResult);
        
        _logger.LogDebug(8, "File was filled with last sorting results");
        
        Console.WriteLine($"Sorted order was saved to '{_deliveryOrder}'\n" +
                          "<————————————————————>\n" +
                          "Returning to main menu...");
    }
}