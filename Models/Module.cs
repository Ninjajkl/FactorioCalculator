using System.Globalization;
using System.Text;

namespace FactorioCalculator.Models;

public class Module
{
    public readonly ModuleType ModuleType;
    public readonly Quality Quality;
    public readonly float SpeedModifier;
    public readonly float EnergyModifier;
    public readonly float ProductivityModifier;
    public readonly float QualityModifier;

    private static readonly Dictionary<(ModuleType, Quality), (float speed, float energy, float productivity, float quality)> Modifiers = new()
        {
            // SpeedModule
            { (ModuleType.SpeedModule, Quality.Normal), (0.2f, 0.5f, 0f, -0.01f) },
            { (ModuleType.SpeedModule, Quality.Uncommon), (0.26f, 0.5f, 0f, -0.01f) },
            { (ModuleType.SpeedModule, Quality.Rare), (0.32f, 0.5f, 0f, -0.01f) },
            { (ModuleType.SpeedModule, Quality.Epic), (0.38f, 0.5f, 0f, -0.01f) },
            { (ModuleType.SpeedModule, Quality.Legendary), (0.5f, 0.5f, 0f, -0.01f) },

            // SpeedModule2
            { (ModuleType.SpeedModule2, Quality.Normal), (0.3f, 0.6f, 0f, -0.015f) },
            { (ModuleType.SpeedModule2, Quality.Uncommon), (0.39f, 0.6f, 0f, -0.015f) },
            { (ModuleType.SpeedModule2, Quality.Rare), (0.48f, 0.6f, 0f, -0.015f) },
            { (ModuleType.SpeedModule2, Quality.Epic), (0.57f, 0.6f, 0f, -0.015f) },
            { (ModuleType.SpeedModule2, Quality.Legendary), (0.75f, 0.6f, 0f, -0.015f) },

            // SpeedModule3
            { (ModuleType.SpeedModule3, Quality.Normal), (0.5f, 0.7f, 0f, -0.025f) },
            { (ModuleType.SpeedModule3, Quality.Uncommon), (0.65f, 0.7f, 0f, -0.025f) },
            { (ModuleType.SpeedModule3, Quality.Rare), (0.80f, 0.7f, 0f, -0.025f) },
            { (ModuleType.SpeedModule3, Quality.Epic), (0.95f, 0.7f, 0f, -0.025f) },
            { (ModuleType.SpeedModule3, Quality.Legendary), (1.25f, 0.7f, 0f, -0.025f) },

            // ProductivityModule
            { (ModuleType.ProductivityModule, Quality.Normal), (-0.05f, 0.4f, 0.04f, 0f) },
            { (ModuleType.ProductivityModule, Quality.Uncommon), (-0.05f, 0.4f, 0.05f, 0f) },
            { (ModuleType.ProductivityModule, Quality.Rare), (-0.05f, 0.4f, 0.06f, 0f) },
            { (ModuleType.ProductivityModule, Quality.Epic), (-0.05f, 0.4f, 0.07f, 0f) },
            { (ModuleType.ProductivityModule, Quality.Legendary), (-0.05f, 0.4f, 0.10f, 0f) },

            // ProductivityModule2
            { (ModuleType.ProductivityModule2, Quality.Normal), (-0.10f, 0.6f, 0.06f, 0f) },
            { (ModuleType.ProductivityModule2, Quality.Uncommon), (-0.10f, 0.6f, 0.07f, 0f) },
            { (ModuleType.ProductivityModule2, Quality.Rare), (-0.10f, 0.6f, 0.09f, 0f) },
            { (ModuleType.ProductivityModule2, Quality.Epic), (-0.10f, 0.6f, 0.11f, 0f) },
            { (ModuleType.ProductivityModule2, Quality.Legendary), (-0.10f, 0.6f, 0.15f, 0f) },

            // ProductivityModule3
            { (ModuleType.ProductivityModule3, Quality.Normal), (-0.15f, 0.8f, 0.10f, 0f) },
            { (ModuleType.ProductivityModule3, Quality.Uncommon), (-0.15f, 0.8f, 0.13f, 0f) },
            { (ModuleType.ProductivityModule3, Quality.Rare), (-0.15f, 0.8f, 0.16f, 0f) },
            { (ModuleType.ProductivityModule3, Quality.Epic), (-0.15f, 0.8f, 0.19f, 0f) },
            { (ModuleType.ProductivityModule3, Quality.Legendary), (-0.15f, 0.8f, 0.25f, 0f) },

            // EfficiencyModule
            { (ModuleType.EfficiencyModule, Quality.Normal), (0f, -0.3f, 0f, 0f) },
            { (ModuleType.EfficiencyModule, Quality.Uncommon), (0f, -0.39f, 0f, 0f) },
            { (ModuleType.EfficiencyModule, Quality.Rare), (0f, -0.48f, 0f, 0f) },
            { (ModuleType.EfficiencyModule, Quality.Epic), (0f, -0.57f, 0f, 0f) },
            { (ModuleType.EfficiencyModule, Quality.Legendary), (0f, -0.75f, 0f, 0f) },

            // EfficiencyModule2
            { (ModuleType.EfficiencyModule2, Quality.Normal), (0f, -0.4f, 0f, 0f) },
            { (ModuleType.EfficiencyModule2, Quality.Uncommon), (0f, -0.52f, 0f, 0f) },
            { (ModuleType.EfficiencyModule2, Quality.Rare), (0f, -0.64f, 0f, 0f) },
            { (ModuleType.EfficiencyModule2, Quality.Epic), (0f, -0.76f, 0f, 0f) },
            { (ModuleType.EfficiencyModule2, Quality.Legendary), (0f, -1f, 0f, 0f) },

            // EfficiencyModule3
            { (ModuleType.EfficiencyModule3, Quality.Normal), (0f, -0.5f, 0f, 0f) },
            { (ModuleType.EfficiencyModule3, Quality.Uncommon), (0f, -0.65f, 0f, 0f) },
            { (ModuleType.EfficiencyModule3, Quality.Rare), (0f, -0.80f, 0f, 0f) },
            { (ModuleType.EfficiencyModule3, Quality.Epic), (0f, -0.95f, 0f, 0f) },
            { (ModuleType.EfficiencyModule3, Quality.Legendary), (0f, -1.25f, 0f, 0f) },

            // QualityModule
            { (ModuleType.QualityModule, Quality.Normal), (-0.05f, 0f, 0f, 0.01f) },
            { (ModuleType.QualityModule, Quality.Uncommon), (-0.05f, 0f, 0f, 0.013f) },
            { (ModuleType.QualityModule, Quality.Rare), (-0.05f, 0f, 0f, 0.016f) },
            { (ModuleType.QualityModule, Quality.Epic), (-0.05f, 0f, 0f, 0.019f) },
            { (ModuleType.QualityModule, Quality.Legendary), (-0.05f, 0f, 0f, 0.025f) },

            // QualityModule2
            { (ModuleType.QualityModule2, Quality.Normal), (-0.05f, 0f, 0f, 0.02f) },
            { (ModuleType.QualityModule2, Quality.Uncommon), (-0.05f, 0f, 0f, 0.026f) },
            { (ModuleType.QualityModule2, Quality.Rare), (-0.05f, 0f, 0f, 0.032f) },
            { (ModuleType.QualityModule2, Quality.Epic), (-0.05f, 0f, 0f, 0.038f) },
            { (ModuleType.QualityModule2, Quality.Legendary), (-0.05f, 0f, 0f, 0.05f) },

            // QualityModule3
            { (ModuleType.QualityModule3, Quality.Normal), (-0.05f, 0f, 0f, 0.025f) },
            { (ModuleType.QualityModule3, Quality.Uncommon), (-0.05f, 0f, 0f, 0.032f) },
            { (ModuleType.QualityModule3, Quality.Rare), (-0.05f, 0f, 0f, 0.04f) },
            { (ModuleType.QualityModule3, Quality.Epic), (-0.05f, 0f, 0f, 0.047f) },
            { (ModuleType.QualityModule3, Quality.Legendary), (-0.05f, 0f, 0f, 0.062f) },
        };

    public static void ExportModifiersToCsv()
    {
        StringBuilder sb = new();
        sb.AppendLine("ModuleType,Quality,SpeedModifier,EnergyModifier,ProductivityModifier,QualityModifier");

        foreach (KeyValuePair<(ModuleType, Quality), (float speed, float energy, float productivity, float quality)> kvp in Modifiers)
        {
            (ModuleType moduleType, Quality quality) = kvp.Key;
            (float speed, float energy, float productivity, float qualityMod) = kvp.Value;
            sb.AppendLine($"{moduleType},{quality},{speed.ToString(CultureInfo.InvariantCulture)},{energy.ToString(CultureInfo.InvariantCulture)},{productivity.ToString(CultureInfo.InvariantCulture)},{qualityMod.ToString(CultureInfo.InvariantCulture)}");
        }

        File.WriteAllText($"..\\..\\..\\ModData\\CustomData\\Modules.csv", sb.ToString());
    }

    public Module(ModuleType mt, Quality q)
    {
        ModuleType = mt;
        Quality = q;

        if (!Modifiers.TryGetValue((mt, q), out (float speed, float energy, float productivity, float quality) mods))
        {
            throw new ArgumentException("Invalid ModuleType/Quality combination");
        }

        SpeedModifier = mods.speed;
        EnergyModifier = mods.energy;
        ProductivityModifier = mods.productivity;
        QualityModifier = mods.quality;
    }
}
