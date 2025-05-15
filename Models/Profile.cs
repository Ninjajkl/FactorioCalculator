using System.Text.Json;

namespace FactorioCalculator.Models;

public class Profile
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    private string _name;
    private List<string> _mods;
    private Dictionary<string, int> _globalProductivity;
    private Dictionary<string, string> _preferredRecipes;
    private HashSet<string> _rawMaterials;
    private Dictionary<string, string> _preferredMachineForCategory;

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            Save();
        }
    }

    public List<string> Mods
    {
        get => _mods;
        set
        {
            _mods = value;
            Save();
        }
    }

    public Dictionary<string, int> GlobalProductivity
    {
        get => _globalProductivity;
        set
        {
            _globalProductivity = value;
            Save();
        }
    }

    public Dictionary<string, string> PreferredRecipes
    {
        get => _preferredRecipes;
        set
        {
            _preferredRecipes = value;
            Save();
        }
    }

    public void AddPreferredRecipe(string item, string recipe)
    {
        _preferredRecipes[item] = recipe;
        Save();
    }

    public bool RemovePreferredRecipe(string item)
    {
        bool removed = _preferredRecipes.Remove(item);
        if (removed)
        {
            Save();
        }

        return removed;
    }

    public HashSet<string> RawMaterials
    {
        get => _rawMaterials;
        set
        {
            _rawMaterials = value;
            Save();
        }
    }

    public void AddRawMaterial(string item)
    {
        _rawMaterials.Add(item);
        Save();
    }

    public bool RemoveRawMaterial(string item)
    {
        bool removed = _rawMaterials.Remove(item);
        if (removed)
        {
            Save();
        }

        return removed;
    }

    public Dictionary<string, string> PreferredMachineForCategory
    {
        get => _preferredMachineForCategory;
        set
        {
            _preferredMachineForCategory = value;
            Save();
        }
    }

    public void AddPreferredMachineForCategory(string item, string recipe)
    {
        _preferredRecipes[item] = recipe;
        Save();
    }

    public void InitializeAfterDeserialization()
    {
        _mods ??= [];
        _globalProductivity ??= [];
        _preferredRecipes ??= [];
        _rawMaterials ??= [];
    }

    public void Save()
    {
        string filePath = $"..\\..\\..\\Profiles\\{_name}.json";
        File.WriteAllText(filePath, JsonSerializer.Serialize(this, _jsonOptions));
    }
}