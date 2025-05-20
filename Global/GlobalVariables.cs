using CsvHelper;
using FactorioCalculator.Models;
using FactorioCalculator.Models.Interfaces;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FactorioCalculator.Global;
public sealed class GlobalVariables
{
    private static readonly Lazy<GlobalVariables> _instance = new(() => new GlobalVariables());
    public static GlobalVariables Instance => _instance.Value;

    public static string DefaultProfile = "default";
    public Dictionary<string, List<MachineType>> CategoryToMachineMap { get; private set; }
    public Dictionary<(ModuleType, Quality), Module> Modules { get; private set; }
    public Dictionary<(MachineType, Quality), IMachine> Machines { get; private set; } = [];

    private readonly JsonSerializerOptions options = new() { Converters = { new JsonStringEnumConverter() } };


    private GlobalVariables()
    {
        DeserializeCategoryToMachineMap();
        LoadModules();
        LoadMachines();
    }

    private void DeserializeCategoryToMachineMap()
    {
        string json = File.ReadAllText($"..\\..\\..\\ModData\\CustomData\\CategoryToMachineMap.json");
        CategoryToMachineMap = JsonSerializer.Deserialize<Dictionary<string, List<MachineType>>>(json, options) ?? [];
    }

    private void LoadModules()
    {
        Dictionary<(ModuleType, Quality), Module> map = [];
        using StreamReader reader = new("..\\..\\..\\ModData\\CustomData\\Modules.csv");
        using CsvReader csv = new(reader, CultureInfo.InvariantCulture);

        csv.Read();
        csv.ReadHeader();
        while (csv.Read())
        {
            string? moduleTypeStr = csv.GetField("ModuleType");
            string? qualityStr = csv.GetField("Quality");
            if (!Enum.TryParse(moduleTypeStr, out ModuleType moduleType))
            {
                continue;
            }

            if (!Enum.TryParse(qualityStr, out Quality quality))
            {
                continue;
            }

            float speed = csv.GetField<float>("SpeedModifier");
            float energy = csv.GetField<float>("EnergyModifier");
            float productivity = csv.GetField<float>("ProductivityModifier");
            float qualityMod = csv.GetField<float>("QualityModifier");

            Module module = new(moduleType, quality, speed, energy, productivity, qualityMod);
            map[(moduleType, quality)] = module;
        }
        Modules = map;
    }

    private void LoadMachines()
    {
        LoadMiners();
        LoadAssemblers();
        LoadFurnaces();
        LoadOffshorePumps();
    }

    private void LoadMiners()
    {
        using StreamReader reader = new("..\\..\\..\\ModData\\CustomData\\Miners.csv");
        using CsvReader csv = new(reader, CultureInfo.InvariantCulture);
        csv.Read();
        csv.ReadHeader();
        while (csv.Read())
        {
            string? machineTypeStr = csv.GetField("MachineType");
            string? qualityStr = csv.GetField("Quality");
            if (!Enum.TryParse(machineTypeStr, out MachineType machineType))
            {
                continue;
            }
            if (!Enum.TryParse(qualityStr, out Quality quality))
            {
                continue;
            }
            float miningSpeed = csv.GetField<float>("MiningSpeed");
            float energyConsumption = csv.GetField<float>("EnergyConsumption");
            int moduleSlots = csv.GetField<int>("ModuleSlots");
            float baseProductivity = csv.GetField<float>("BaseProductivity");
            float resourceDrain = csv.GetField<float>("ResourceDrain");
            IMachine machine = new Miner(machineType, quality, miningSpeed, energyConsumption, moduleSlots, baseProductivity, resourceDrain);
            Machines[(machineType, quality)] = machine;
        }
    }

    private void LoadAssemblers()
    {
        using StreamReader reader = new("..\\..\\..\\ModData\\CustomData\\Assemblers.csv");
        using CsvReader csv = new(reader, CultureInfo.InvariantCulture);
        csv.Read();
        csv.ReadHeader();
        while (csv.Read())
        {
            string? machineTypeStr = csv.GetField("MachineType");
            string? qualityStr = csv.GetField("Quality");
            if (!Enum.TryParse(machineTypeStr, out MachineType machineType))
            {
                continue;
            }
            if (!Enum.TryParse(qualityStr, out Quality quality))
            {
                continue;
            }
            float craftingSpeed = csv.GetField<float>("CraftingSpeed");
            float energyConsumption = csv.GetField<float>("EnergyConsumption");
            int moduleSlots = csv.GetField<int>("ModuleSlots");
            float baseProductivity = csv.GetField<float>("BaseProductivity");
            IMachine machine = new Assembler(machineType, quality, craftingSpeed, energyConsumption, moduleSlots, baseProductivity);
            Machines[(machineType, quality)] = machine;
        }
    }

    private void LoadFurnaces()
    {
        using StreamReader reader = new("..\\..\\..\\ModData\\CustomData\\Furnaces.csv");
        using CsvReader csv = new(reader, CultureInfo.InvariantCulture);
        csv.Read();
        csv.ReadHeader();
        while (csv.Read())
        {
            string? machineTypeStr = csv.GetField("MachineType");
            string? qualityStr = csv.GetField("Quality");
            if (!Enum.TryParse(machineTypeStr, out MachineType machineType))
            {
                continue;
            }
            if (!Enum.TryParse(qualityStr, out Quality quality))
            {
                continue;
            }
            float craftingSpeed = csv.GetField<float>("CraftingSpeed");
            float energyConsumption = csv.GetField<float>("EnergyConsumption");
            int moduleSlots = csv.GetField<int>("ModuleSlots");
            IMachine machine = new Furnace(machineType, quality, craftingSpeed, energyConsumption, moduleSlots);
            Machines[(machineType, quality)] = machine;
        }
    }

    private void LoadOffshorePumps()
    {
        using StreamReader reader = new("..\\..\\..\\ModData\\CustomData\\OffshorePump.csv");
        using CsvReader csv = new(reader, CultureInfo.InvariantCulture);
        csv.Read();
        csv.ReadHeader();
        while (csv.Read())
        {
            string? machineTypeStr = csv.GetField("MachineType");
            string? qualityStr = csv.GetField("Quality");
            if (!Enum.TryParse(machineTypeStr, out MachineType machineType))
            {
                continue;
            }
            if (!Enum.TryParse(qualityStr, out Quality quality))
            {
                continue;
            }
            int pumpingSpeed = csv.GetField<int>("PumpingSpeed");
            IMachine machine = new OffshorePump(machineType, quality, pumpingSpeed);
            Machines[(machineType, quality)] = machine;
        }
    }
}