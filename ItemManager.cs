using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FactorioCalculator.Items;
public class ItemManager
{
    private static readonly Lazy<ItemManager> _instance = new(() => new ItemManager());
    public static ItemManager Instance => _instance.Value;
    private readonly Dictionary<string, Item> _items;
    public IEnumerable<Item> GetAllLoadedItems() => _items.Values;

    private ItemManager()
    {
        _items = new Dictionary<string, Item>();
    }

    //Method to load an item by name; if already loaded, it retrieves from the cache
    public Item GetItem(string itemName)
    {
        if (_items.TryGetValue(itemName, out var item))
        {
            return item;
        }

        try
        {
            var json = File.ReadAllText($"../../../Items/{itemName}.json");
            var itemData = JsonSerializer.Deserialize<ItemData>(json);

            item = new Item
            {
                Name = itemData.Name,
                BaseTimeToCraft = itemData.BaseTimeToCraft,
                NumCreatedPerCraft = itemData.NumCreatedPerCraft,
                IsRawMaterial = itemData.IsRawMaterial
            };

            _items[itemName] = item;

            // Convert components from Dictionary<string, int> to Dictionary<Item, int>
            foreach (var component in itemData.Components)
            {
                var componentItem = GetItem(component.Key);
                item.Components[componentItem] = component.Value;
            }

            return item;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading or deserializing file for item '{itemName}': {ex.Message}");
            return null;
        }
    }

    //Used for deserialization
    private class ItemData
    {
        public string Name { get; set; }
        public Dictionary<string, int> Components { get; set; }
        public float BaseTimeToCraft { get; set; }
        public int NumCreatedPerCraft { get; set; }
        public bool IsRawMaterial { get; set; }
    }
}