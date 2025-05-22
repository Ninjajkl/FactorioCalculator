using FactorioCalculator.Global;
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
        Ingredient baseIngredient = new()
        {
            Name = itemName,
            Amount = 1
        };
        //Get the preferred machine + modules combo for this machine
        (IMachine machineUsed, List<Module> modulesUsed) = FindPreferredMachine(recipe);

        (float machinesUsed, float ingredientsNeededPerSec, float totalEnergyConsumption) = CalculateFromMachine(numPerSec, item, baseIngredient, machineUsed, modulesUsed);
        item.Machines = machinesUsed;
        CalculateBlueprintRecursively(item, ingredientsNeededPerSec);
        DisplayBlueprint(item);
    }

    private void CalculateBlueprintRecursively(Item item, float parentNumPerSec)
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

            //Get the preferred machine + modules combo for this machine
            (IMachine machineUsed, List<Module> modulesUsed) = FindPreferredMachine(ingredientRecipe);

            //Get the Item + Recipe for this ingredient
            Item ingredientItem = new(ingredient.Name, ingredientRecipe);

            item.Components.Add(ingredientItem);

            (float machinesUsed, float ingredientsNeededPerSec, float totalEnergyConsumption) = CalculateFromMachine(parentNumPerSec, ingredientItem, ingredient, machineUsed, modulesUsed);

            ingredientItem.Machines = machinesUsed;

            //Recursively calculate for subcomponents with increased depth
            CalculateBlueprintRecursively(ingredientItem, ingredientsNeededPerSec);
        }
    }

    #region Calculations

    private static (float machinesUsed, float ingredientsNeededPerSec, float totalEnergyConsumption) CalculateFromMachine(float pNumPerSec, Item ingredientItem, Ingredient ingredient, IMachine machineUsed, List<Module> modulesUsed)
    {
        //Sum each modifier from all modules
        float totalSpeedModifier = modulesUsed.Sum(m => m.SpeedModifier);
        float totalProductivityModifier = modulesUsed.Sum(m => m.ProductivityModifier);
        //Min energy consuption is 20%, so need to clamp
        float totalEfficiencyModifier = (float)Math.Max(modulesUsed.Sum(m => m.EnergyModifier), -0.8);

        //Machine-specific values
        float baseSpeed = 1f;
        float baseProductivity = 0f;
        float baseEnergyConsumption = 0;

        switch (machineUsed.MachineCategory)
        {
            case MachineCategory.Assembly:
                if (machineUsed is Assembler assembler)
                {
                    baseProductivity = assembler.BaseProductivity;
                    baseSpeed = assembler.CraftingSpeed;
                    baseEnergyConsumption = assembler.EnergyConsumption;
                    break;
                }
                else
                {
                    throw new Exception($"Failed to cast {machineUsed} as Assembler");
                }

            case MachineCategory.Mining:
                if (machineUsed is Miner miner)
                {
                    baseProductivity = miner.BaseProductivity;
                    baseSpeed = miner.MiningSpeed;
                    baseEnergyConsumption = miner.EnergyConsumption;
                    break;
                }
                else
                {
                    throw new Exception($"Failed to cast {machineUsed} as Miner");
                }

            case MachineCategory.Furnace:
                if (machineUsed is Furnace furnace)
                {
                    baseSpeed = furnace.CraftingSpeed;
                    baseEnergyConsumption = furnace.EnergyConsumption;
                    break;
                }
                else
                {
                    throw new Exception($"Failed to cast {machineUsed} as Furnace");
                }

            case MachineCategory.OffshorePump:
                if (machineUsed is OffshorePump offshorePump)
                {
                    baseSpeed = offshorePump.PumpingSpeed;
                    break;
                }
                else
                {
                    throw new Exception($"Failed to cast {machineUsed} as OffshorePump");
                }

            default:
                throw new Exception($"Unknown machine category: {machineUsed.MachineCategory}");
        }

        //Add base productivity for applicable machines
        totalProductivityModifier += baseProductivity;

        //How many items are created per machine craft
        float numItemsCreatedPerCraft = ingredientItem.ItemsCreatedPerCraft * (1 + totalProductivityModifier);

        //How many items are needed per second to fulfill the parent's needs
        float numItemsNeededPerSec = ingredient.Amount * pNumPerSec;

        //How many ingredients I need per sec (not scaled by amount of each item per craft)
        float ingredientsNeededPerSec = numItemsNeededPerSec / numItemsCreatedPerCraft;

        //Machine crafting speed + speed modifiers
        float machineSpeed = baseSpeed * (1 + totalSpeedModifier);

        //Get the crafting time of this component
        float secondsPerCraft = (float)ingredientItem.Recipe.EnergyRequired / machineSpeed;

        //How many items are created per machine each second
        float itemsCreatedPerMachinePerSec = numItemsCreatedPerCraft / secondsPerCraft;

        //How many machines are needed
        float machinesNeeded = numItemsNeededPerSec / itemsCreatedPerMachinePerSec;

        //The total energy cost of all machines per sec, assuming constant use
        float energyConsumption = machinesNeeded * baseEnergyConsumption * (1 + totalEfficiencyModifier);

        return (machinesNeeded, ingredientsNeededPerSec, energyConsumption);
    }

    #endregion Calculations

    #region Search and Prompt Preferred

    private Recipe FindPreferredRecipe(string itemName)
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
        Console.WriteLine($"\n\nNo preferred recipe found for {itemName}, which recipe is preferred?");
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

    private (IMachine, List<Module>) FindPreferredMachine(Recipe recipe)
    {
        if (_profile.PreferredMachineForRecipe.TryGetValue(recipe.Name, out (IMachine machine, List<Module>) entry))
        {
            return entry;
        }
        Console.WriteLine($"\nNo preferred machine setup for recipe {recipe.Name} w/ category {recipe.Category}");

        MachineType machineType = FindPreferredMachineForCategory(recipe.Category);

        Quality quality;
        while (true)
        {
            Console.WriteLine($"\nWhat quality is the machine? 'Normal', 'Uncommon', 'Rare', 'Epic', 'Legendary'");
            string strQuality = Console.ReadLine();

            if (Enum.TryParse(strQuality, true, out Quality q))
            {
                quality = q;
                break;
            }
            else
            {
                Console.WriteLine("Invalid quality. Please try again.");
            }
        }

        IMachine machine = GlobalVariables.Instance.Machines[(machineType, quality)];

        List<Module> modulesUsed = [];
        if (machine.ModuleSlots > 0)
        {
            modulesUsed = PromptForModules(machine.ModuleSlots);
        }
        _profile.AddPreferredMachineForRecipe(recipe.Name, machine, modulesUsed);
        PrintMachineAndModules(machine, modulesUsed);
        return (machine, modulesUsed);
    }

    private MachineType FindPreferredMachineForCategory(string category)
    {
        if (_profile.PreferredMachineForCategory.TryGetValue(category, out MachineType machine))
        {
            return machine;
        }
        Console.WriteLine($"No preferred machine found for category {category}. Which machine is preferred?\n");

        List<MachineType> validMachines = GlobalVariables.Instance.CategoryToMachineMap[category];

        foreach (MachineType machineType in validMachines)
        {
            Console.WriteLine($"- {machineType}");
        }
        while (true)
        {
            Console.WriteLine("\nEnter the name of the preferred machine:");
            string machineName = Console.ReadLine();
            if (Enum.TryParse(machineName, out MachineType mT) && validMachines.Contains(mT))
            {
                _profile.AddPreferredMachineForCategory(category, mT);
                return mT;
            }
            else
            {
                Console.WriteLine("Invalid machine name. Please try again.");
            }
        }
    }

    private static List<Module> PromptForModules(int moduleSlots)
    {
        List<Module> modules = [];
        Console.WriteLine($"\nThis machine has {moduleSlots} module slots.");

        Console.Write($"Skip Modules? (y/n)");
        string skip = Console.ReadLine()?.Trim().ToLower();
        if (skip == "y")
        {
            //All slots skipped
            return modules;
        }

        Console.Write("Do you want to fill all slots with the same module type and quality? (y/n): ");
        string bulk = Console.ReadLine()?.Trim().ToLower();

        if (bulk == "y")
        {
            Console.Write("Leave all slots empty? (y/n): ");
            string leaveEmpty = Console.ReadLine()?.Trim().ToLower();
            if (leaveEmpty == "y")
            {
                return modules;
            }

            ModuleType moduleType = PromptForModuleType();
            Quality quality = PromptForModuleQuality();
            Module module = GlobalVariables.Instance.Modules[(moduleType, quality)];
            for (int i = 0; i < moduleSlots; i++)
            {
                modules.Add(module);
            }

            return modules;
        }

        for (int i = 0; i < moduleSlots; i++)
        {
            Console.WriteLine($"\nSlot {i + 1}:");
            Console.Write("Leave remaining slots empty? (y/n): ");
            string leaveEmpty = Console.ReadLine()?.Trim().ToLower();
            if (leaveEmpty == "y")
            {
                return modules;
            }
            ModuleType moduleType = PromptForModuleType();
            Quality quality = PromptForModuleQuality();
            Module module = GlobalVariables.Instance.Modules[(moduleType, quality)];
            modules.Add(module);
        }
        return modules;
    }

    private static ModuleType PromptForModuleType()
    {
        while (true)
        {
            Console.WriteLine("\nAvailable module types:");
            foreach (ModuleType type in Enum.GetValues<ModuleType>())
            {
                Console.WriteLine($"- {type}");
            }

            Console.Write("Enter module type: ");
            string input = Console.ReadLine();
            if (Enum.TryParse(input, true, out ModuleType moduleType))
            {
                return moduleType;
            }

            Console.WriteLine("Invalid module type. Please try again.");
        }
    }

    private static Quality PromptForModuleQuality()
    {
        while (true)
        {
            Console.WriteLine("\nAvailable qualities:");
            foreach (Quality q in Enum.GetValues<Quality>())
            {
                Console.WriteLine($"- {q}");
            }

            Console.Write("Enter module quality: ");
            string input = Console.ReadLine();
            if (Enum.TryParse(input, true, out Quality quality))
            {
                return quality;
            }

            Console.WriteLine("Invalid quality. Please try again.");
        }
    }

    #endregion Search and Prompt Preferred

    #region Display

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

    private void PrintMachineAndModules(IMachine machine, List<Module> modules)
    {
        Console.WriteLine($"\nSelected Machine:");
        Console.WriteLine($"- Type: {machine.MachineType}");
        Console.WriteLine($"- Quality: {machine.Quality}");
        Console.WriteLine($"- Category: {machine.MachineCategory}");
        Console.WriteLine($"- Module Slots: {machine.ModuleSlots}");

        if (modules.Count > 0)
        {
            Console.WriteLine("Modules:");
            for (int i = 0; i < modules.Count; i++)
            {
                Module mod = modules[i];
                Console.WriteLine($"  Slot {i + 1}: {mod.ModuleType} ({mod.Quality})");
            }
        }
        else
        {
            Console.WriteLine("No modules installed.");
        }
        Console.WriteLine("");
    }

    #endregion Display
}