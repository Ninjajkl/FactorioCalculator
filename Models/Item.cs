using FactorioCalculator.Models.Interfaces;

namespace FactorioCalculator.Models;

public class Item : IItem
{
    public string Name { get; set; }
    public Recipe Recipe { get; set; }
    public List<IItem> Components { get; set; } = [];
    public float QuantityNeededPerSecond { get; set; }

    public float TotalMachines => Components.OfType<Item>().Sum(c => c.TotalMachines) + Machines;
    public float Machines { get; set; } = -1;
    public long ItemsCreatedPerCraft => itemsCreatedPerCraft == -1 ? FindItemsCreatedPerCraft() : itemsCreatedPerCraft;
    private long itemsCreatedPerCraft { get; set; } = -1;

    public Item(string name, Recipe recipe)
    {
        Name = name;
        Recipe = recipe;
    }

    public long FindItemsCreatedPerCraft()
    {
        Dictionary<string, Result>.ValueCollection results = Recipe.Results.Values;

        foreach (Result result in results)
        {
            if (result.Name == Name)
            {
                return result.Amount;
            }
        }
        throw new Exception($"Item {Name} not found in recipe {Recipe} results.");

    }
}