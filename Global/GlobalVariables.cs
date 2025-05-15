using CsvHelper;
using FactorioCalculator.Models;
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
    public record struct ModuleData(float Speed, float Energy, float Productivity, float Quality);
    public Dictionary<(ModuleType, Quality), ModuleData> Modules { get; private set; }



    private GlobalVariables()
    {
        DeserializeCategoryToMachineMap();
        LoadModules();
    }

    private void DeserializeCategoryToMachineMap()
    {
        JsonSerializerOptions options = new()
        {
            Converters = { new JsonStringEnumConverter() }
        };

        string json = File.ReadAllText($"..\\..\\..\\ModData\\CustomData\\CategoryToMachineMap.json");
        CategoryToMachineMap = JsonSerializer.Deserialize<Dictionary<string, List<MachineType>>>(json, options) ?? [];
    }

    private void LoadModules()
    {
        Dictionary<(ModuleType, Quality), ModuleData> map = [];
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

            map[(moduleType, quality)] = new ModuleData(speed, energy, productivity, qualityMod);
        }
        Modules = map;
    }
}