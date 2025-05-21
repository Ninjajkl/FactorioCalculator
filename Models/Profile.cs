using FactorioCalculator.Global;
using FactorioCalculator.Models.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FactorioCalculator.Models;

public class Profile
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private string _name;
    private List<string> _mods;
    private Dictionary<string, int> _globalProductivity;
    private Dictionary<string, string> _preferredRecipes;
    private HashSet<string> _rawMaterials;
    private Dictionary<string, MachineType> _preferredMachineForCategory;
    private Dictionary<string, (IMachine, List<Module>)> _preferredMachineForRecipe;
    private Dictionary<string, SerializableMachine> _serializedPreferredMachineForRecipe;

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

    public Dictionary<string, MachineType> PreferredMachineForCategory
    {
        get => _preferredMachineForCategory;
        set
        {
            _preferredMachineForCategory = value;
            Save();
        }
    }

    public void AddPreferredMachineForCategory(string category, MachineType mt)
    {
        _preferredMachineForCategory[category] = mt;
        Save();
    }

    [JsonIgnore]
    public Dictionary<string, (IMachine, List<Module>)> PreferredMachineForRecipe
    {
        get => _preferredMachineForRecipe;
        set
        {
            _preferredMachineForRecipe = value;
            Save();
        }
    }

    public Dictionary<string, SerializableMachine> SerializedPreferredMachineForRecipe
    {
        get => _serializedPreferredMachineForRecipe;
        set
        {
            _serializedPreferredMachineForRecipe = value;
            Save();
        }
    }

    public void AddPreferredMachineForRecipe(string recipeName, IMachine machine, List<Module> modules)
    {
        _preferredMachineForRecipe[recipeName] = (machine, modules);
        _serializedPreferredMachineForRecipe[recipeName] = new SerializableMachine
        {
            MachineType = machine.MachineType,
            Quality = machine.Quality,
            Modules = modules
                .Select(m => new SerializableModule
                {
                    ModuleType = m.ModuleType,
                    Quality = m.Quality
                })
                .ToList()
        };
        Save();
    }

    public void InitializeAfterDeserialization()
    {
        _mods ??= [];
        _globalProductivity ??= [];
        _preferredRecipes ??= [];
        _rawMaterials ??= [];
        _preferredMachineForCategory ??= [];
        _preferredMachineForRecipe ??= [];
        _serializedPreferredMachineForRecipe ??= [];

        // LINQ: Convert serialized to non-serialized preferred machines
        _preferredMachineForRecipe = _serializedPreferredMachineForRecipe
            .Select(kvp =>
            {
                string recipeName = kvp.Key;
                SerializableMachine serialMachine = kvp.Value;

                // Convert SerializableMachines to real Machines
                IMachine machine = GlobalVariables.Instance.Machines[(serialMachine.MachineType, serialMachine.Quality)];

                // Convert SerializableModules to real Modules
                List<Module> modules = serialMachine.Modules
                    .Select(m => GlobalVariables.Instance.Modules[(m.ModuleType, m.Quality)])
                    .ToList() ?? [];

                return new { recipeName, machine, modules };
            })
            .Where(x => x != null)
            .ToDictionary(
                x => x.recipeName,
                x => (x.machine, x.modules)
            );
    }

    public void Save()
    {
        string filePath = $"..\\..\\..\\Profiles\\{_name}.json";
        File.WriteAllText(filePath, JsonSerializer.Serialize(this, _jsonOptions));
    }
}

public class SerializableMachine
{
    public MachineType MachineType { get; set; }
    public Quality Quality { get; set; }
    public List<SerializableModule> Modules { get; set; }
}

public class SerializableModule
{
    public ModuleType ModuleType { get; set; }
    public Quality Quality { get; set; }
}