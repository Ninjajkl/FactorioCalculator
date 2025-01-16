using FactorioCalculator;

List<string> mods = [];

Console.WriteLine("Mods?");
if (Console.ReadLine().ToLower() is "y" or "yes")
{
    Console.WriteLine("Use All Mods?");
    if (Console.ReadLine().ToLower() is "y" or "yes")
    {
        mods = ["elevatedRails", "quality", "spaceAge"];
    }
    else
    {
        Console.WriteLine("Individual mod selection not implemented");
        return;
    }
}

RecipeManager recipeManager = new(mods);
Dictionary<string, Recipe> recipes = recipeManager.GetAllRecipes();
Dictionary<string, List<Recipe>> itemRecipesMap = recipeManager.GetAllItemRecipes();

/*
Dictionary<string, Recipe> recipes = recipeManager.GetAllRecipes();

foreach (KeyValuePair<string, Recipe> kvp in recipes)
{
    Console.WriteLine($"{kvp.Value}");
}

Dictionary<string, List<Recipe>> itemRecipesMap = recipeManager.GetAllItemRecipes();

foreach (KeyValuePair<string, List<Recipe>> itemRecipes in itemRecipesMap)
{
    Console.WriteLine($"{itemRecipes.Key}:");
    foreach (Recipe recipe in itemRecipes.Value)
    {
        Console.WriteLine($"\t{recipe.Name}");
    }
}
*/
return;


/*
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
        Item item = itemManager.GetItem(itemName);
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
*/