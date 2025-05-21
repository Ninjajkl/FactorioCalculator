using FactorioCalculator.Global;
using FactorioCalculator.Helpers;
using FactorioCalculator.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

JsonSerializerOptions _jsonOptions = new()
{
    WriteIndented = true,
    Converters = { new JsonStringEnumConverter() }
};

Profile profile;

do
{
    Console.WriteLine($"What Profile?");
    string profileName = Console.ReadLine();
    if (string.IsNullOrEmpty(profileName))
    {
        profileName = GlobalVariables.DefaultProfile;
    }

    try
    {
        string json = File.ReadAllText($"..\\..\\..\\Profiles\\{profileName}.json");
        profile = JsonSerializer.Deserialize<Profile>(json, _jsonOptions);
        profile.InitializeAfterDeserialization();
    }
    catch
    {
        profile = null;
        Console.WriteLine("Invalid Profile Name\n");
    }

} while (profile is null);

RecipeManager recipeManager = new(profile);
Dictionary<string, Recipe> recipes = recipeManager.GetAllRecipes();
Dictionary<string, List<Recipe>> itemRecipesMap = recipeManager.GetAllItemRecipes();

while (true)
{
    Console.Write("Enter the item name (or enter to quit): ");
    string itemName = Console.ReadLine();

    if (itemName == "")
    {
        break;
    }

    if (itemRecipesMap.ContainsKey(itemName))
    {
        Console.Write("Enter the quantity per second: ");
        float quantityPerSec = float.Parse(Console.ReadLine());
        recipeManager.GetItemBlueprint(itemName, quantityPerSec);
    }
    else
    {
        Console.WriteLine($"No recipes found for {itemName}.");
    }
}