using FactorioCalculator.Items;

var itemManager = ItemManager.Instance;

//FindMostEfficentCombo.Find();
//return;

while (true)
{
    Console.Write("Enter the item name (or type 'exit' to quit): ");
    string itemName = Console.ReadLine();

    if (itemName.ToLower() == "exit")
    {
        break;
    }

    Console.Write("Enter the quantity needed per second: ");
    if (float.TryParse(Console.ReadLine(), out float quantity))
    {
        var item = itemManager.GetItem(itemName);
        if (item != null)
        {
            Console.WriteLine(item.CalculateMachines(quantity));
        }
        else
        {
            Console.WriteLine("Item not found. Please try again.");
        }
    }
    else
    {
        Console.WriteLine("Invalid quantity. Please enter a valid number.");
    }
}
