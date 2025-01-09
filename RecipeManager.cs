using System.Text.Json;

namespace FactorioCalculator;
public class RecipeManager
{
    private readonly Dictionary<string, Recipe> _recipes;
    public Dictionary<string, Recipe> GetAllRecipes()
    {
        return _recipes;
    }

    public RecipeManager(List<string> modList = null)
    {
        modList ??= [];
        //ORDER MATTERS! 
        modList.Insert(0, "base");
        _recipes = [];
        foreach (string mod in modList)
        {
            string jsonFilePath = $"../../../{mod}Recipes.json";
            string jsonString = File.ReadAllText(jsonFilePath);
            List<Recipe>? recipes = JsonSerializer.Deserialize<List<Recipe>>(jsonString, Converter.Settings);
            foreach (Recipe recipe in recipes)
            {
                _recipes[recipe.Name] = recipe;
            }
        }
    }

    /*
    //Method to load an recipe by name; if already loaded, it retrieves from the cache
    public Recipe GetRecipe(string recipeName)
    {
        if (_recipes.TryGetValue(recipeName, out Recipe? recipe))
        {
            return recipe;
        }

        try
        {
            string json = File.ReadAllText($"../../../Recipes/{recipeName}.json");
            var recipeData = JsonSerializer.Deserialize<RecipeData>(json);

            recipe = new Recipe
            {
                Name = recipeData.Name,
                BaseTimeToCraft = recipeData.BaseTimeToCraft,
                NumCreatedPerCraft = recipeData.NumCreatedPerCraft,
                IsRawMaterial = recipeData.IsRawMaterial
            };

            _recipes[recipeName] = recipe;

            // Convert components from Dictionary<string, int> to Dictionary<Recipe, int>
            foreach (var component in recipeData.Components)
            {
                Recipe componentRecipe = GetRecipe(component.Key);
                recipe.Components[componentRecipe] = component.Value;
            }

            return recipe;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading or deserializing file for recipe '{recipeName}': {ex.Message}");
            return null;
        }
    }
    */
}