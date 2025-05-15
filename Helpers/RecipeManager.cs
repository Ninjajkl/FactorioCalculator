using FactorioCalculator.Models;
using FactorioCalculator.Models.Interfaces;
using NLua;

namespace FactorioCalculator.Helpers;
public class RecipeManager
{
    private readonly Dictionary<string, Recipe> _recipes;
    private readonly Dictionary<string, List<Recipe>> _itemRecipesMap;

    private readonly Profile _profile;

    public Dictionary<string, Recipe> GetAllRecipes()
    {
        return _recipes;
    }

    public Dictionary<string, List<Recipe>> GetAllItemRecipes()
    {
        return _itemRecipesMap;
    }

    public RecipeManager(Profile p)
    {
        _profile = p;
        List<string> modList = _profile.Mods;
        modList ??= [];
        //ORDER MATTERS!
        _recipes = [];
        _itemRecipesMap = [];

        List<Recipe> recipesList = ProcessLua.LoadRecipesData(new Lua(), modList);
        foreach (Recipe r in recipesList)
        {
            _recipes[r.Name] = r;

            foreach (Result result in r.Results.Values)
            {
                if (!_itemRecipesMap.ContainsKey(result.Name))
                {
                    _itemRecipesMap[result.Name] = [];
                }
                _itemRecipesMap[result.Name].Add(r);
            }
        }
    }

    public void GetItemBlueprint(string itemName, float numPerSec)
    {
        Recipe recipe = FindPreferredRecipe(itemName);
        if (recipe == null)
        {
            Console.WriteLine($"Item {itemName} is a raw material.");
            return;
        }
        Item item = new(itemName, recipe);
        float craftsPerSec = numPerSec / item.ItemsCreatedPerCraft;
        float craftingTime = (float)item.Recipe.EnergyRequired;
        item.Machines = craftsPerSec * craftingTime;
        CalculateBlueprintRecursively(item, craftsPerSec);
        DisplayBlueprint(item);
    }

    public void CalculateBlueprintRecursively(Item item, float parentNumPerSec)
    {
        foreach (Ingredient ingredient in item.Recipe.Ingredients.Values)
        {
            //Get the Item + Recipe for this ingredient
            Recipe ingredientRecipe = FindPreferredRecipe(ingredient.Name);

            if (_profile.RawMaterials.Contains(ingredient.Name))
            {
                //If this is a raw material, create a RawMaterial object
                RawMaterial rawMaterial = new(ingredient.Name, ingredient.Amount * parentNumPerSec);
                item.Components.Add(rawMaterial);
                continue;
            }

            //Get the Item + Recipe for this ingredient
            Item ingredientItem = new(ingredient.Name, ingredientRecipe);

            item.Components.Add(ingredientItem);

            //Calculate the number of ingredients needed per second for this ingredient
            //Amount of Ingredients needed per craft Divided by how many Ingredients are created per craft
            //Multiplied by quanitity needed per second
            float numPerSecond = (float)ingredient.Amount / ingredientItem.ItemsCreatedPerCraft * parentNumPerSec;

            //Get the crafting time of this component
            //It's stored as the energy required, but that's the same as the crafting time
            //Ridiculous honestly
            float craftingTime = (float)ingredientItem.Recipe.EnergyRequired;

            //Calculate the number of machines needed based on crafting time
            //This calculation is currently off as no assembly machine speed is taken into account, and none have a speed of 1
            ingredientItem.Machines = numPerSecond * craftingTime;

            //Recursively calculate for subcomponents with increased depth
            CalculateBlueprintRecursively(ingredientItem, numPerSecond);
        }
    }

    public Recipe FindPreferredRecipe(string itemName)
    {
        if (_profile.RawMaterials.Contains(itemName))
        {
            return null;
        }
        if (!_itemRecipesMap.TryGetValue(itemName, out List<Recipe> recipes))
        {
            _profile.AddRawMaterial(itemName);
            return null;
        }
        //Check if already set a preferred recipe
        if (_profile.PreferredRecipes.TryGetValue(itemName, out string rName))
        {
            if (_recipes.TryGetValue(rName, out Recipe recipe))
            {
                return recipe;
            }
        }

        //Multiple recipes and no set recipe for this profile
        //Ask User
        Console.WriteLine($"\n\nMultiple recipes found for {itemName}, which recipe is preferred?");
        foreach (Recipe recipe in recipes)
        {
            Console.WriteLine($"\n{recipe}");
        }
        Console.WriteLine($"\n");
        while (true)
        {
            Console.WriteLine("Enter the name of the preferred recipe (or 'raw' if its a raw material):");
            string recipeName = Console.ReadLine();

            if (recipeName == "raw")
            {
                _profile.AddRawMaterial(itemName);
                return null;
            }
            else if (_recipes.TryGetValue(recipeName, out Recipe outRecipe) && recipes.Contains(outRecipe))
            {
                _profile.AddPreferredRecipe(itemName, recipeName);
                return outRecipe;
            }
            else
            {
                Console.WriteLine("Invalid recipe name. Please try again.");
            }
        }

    }

    public static void DisplayBlueprint(Item item, int depth = 0)
    {
        // Indentation for hierarchy
        string indent = new(' ', depth * 2);

        // Display this item's name and machines needed
        Console.WriteLine($"{indent}- {item.Name}: {item.Machines} machines");

        // Recurse for each component
        foreach (IItem component in item.Components)
        {
            if (component is Item)
            {
                DisplayBlueprint(component as Item, depth + 1);
            }
            else
            {
                // For non-Item (e.g., RawMaterial), just display the name
                Console.WriteLine($"{new string(' ', (depth + 1) * 2)}- {component.QuantityNeededPerSecond} {component.Name}/s");
            }
        }
    }
}