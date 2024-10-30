namespace EffectiveMobileTask;

public class ConsoleInterface
{
    private readonly DataBase _dataBaseServiceProvider;

    public ConsoleInterface(DataBase dataBaseServiceProvider)
    {
        _dataBaseServiceProvider = dataBaseServiceProvider;
        
        Console.WriteLine("Welcome to Orders Sorting Application for Delivery Service!");
    }
    
    public void InitConsole()
    {
        Console.WriteLine("Available commands:\n" +
                          "1 – Print orders list\n" +
                          "2 – Add new order\n" +
                          "3 – Delete an order\n" +
                          "4 – Filter/Sort orders\n" +
                          "0 – Exit");
        
        switch (Console.ReadLine())
        {
            case "1":
                _dataBaseServiceProvider.PrintOrdersList();
                break;
            case "2":
                _dataBaseServiceProvider.CreateOrder();
                break;
            case "3":
                _dataBaseServiceProvider.DeleteOrder();
                break;
            case "4":
                _dataBaseServiceProvider.SortOrders();
                break;
            case "0":
                Console.WriteLine(" <————— Exiting... —————>");
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Please enter a valid command number");
                break;
        }
        
        InitConsole();
    }
}