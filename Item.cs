using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FactorioCalculator;

public class Item
{
    public string Name { get; set; }
    public Dictionary<Item, int> Components { get; set; }
    public float BaseTimeToCraft { get; set; }
    public int NumCreatedPerCraft { get; set; }
    public bool IsRawMaterial { get; set; }

    public Item()
    {
        Name = "Unnamed";
        Components = new();
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Item: {Name}");
        sb.AppendLine($"  BaseTimeToCraft: {BaseTimeToCraft}");
        sb.AppendLine($"  NumCreatedPerCraft: {NumCreatedPerCraft}");
        sb.AppendLine($"  IsRawMaterial: {IsRawMaterial}");
        sb.AppendLine("  Components:");

        // Initialize a dictionary to hold raw material totals
        var rawMaterialTotals = new Dictionary<string, float>();

        // Call recursive function to append components with level 1 indentation
        AppendComponents(sb, Components, NumCreatedPerCraft, 1, rawMaterialTotals);

        // Print total raw material components
        sb.AppendLine("  Total Raw Material Components:");
        foreach (var kvp in rawMaterialTotals)
        {
            sb.AppendLine($"    - {kvp.Key}: {kvp.Value}");
        }

        return sb.ToString();
    }

    private void AppendComponents(StringBuilder sb, Dictionary<Item, int> components, float quantity, int depth, Dictionary<string, float> rawMaterialTotals)
    {
        foreach (var component in components)
        {
            // Calculate the total quantity of this component based on the multiplier
            float totalQuantity = component.Value * quantity / component.Key.NumCreatedPerCraft;

            // Add indentation based on depth (2 spaces per depth level)
            sb.AppendLine($"{new string(' ', depth * 2)}- {component.Key.Name}: {totalQuantity}");

            // If the component is a raw material, sum its total quantity
            if (component.Key.IsRawMaterial)
            {
                if (rawMaterialTotals.ContainsKey(component.Key.Name))
                {
                    rawMaterialTotals[component.Key.Name] += totalQuantity;
                }
                else
                {
                    rawMaterialTotals[component.Key.Name] = totalQuantity;
                }
            }
            else
            {
                // Recursively add subcomponents with increased depth
                AppendComponents(sb, component.Key.Components, totalQuantity, depth + 1, rawMaterialTotals);
            }
        }
    }

    public string CalculateMachines(float productionRatePerSecond)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Calculating machines needed for {productionRatePerSecond} {Name} per second:");

        // Calculate machines needed for the base item itself
        float baseNeededPerSecond = (1 / (float)NumCreatedPerCraft) * productionRatePerSecond; // 1 base item needed
        float machinesForBase = baseNeededPerSecond * BaseTimeToCraft;

        // Print the number of machines needed for the base item
        sb.AppendLine($"  - {Name}: {Math.Ceiling(machinesForBase)} machines");

        // Calculate machines needed for the base item with increased depth
        int totalAssemblers = CalculateMachinesRecursively(sb, Components, productionRatePerSecond, 2); // Increased depth for components

        // Print total number of assemblers needed
        sb.AppendLine($"Total assemblers needed for {Name}: {totalAssemblers + machinesForBase} machines");

        return sb.ToString();
    }

    private int CalculateMachinesRecursively(StringBuilder sb, Dictionary<Item, int> components, float productionRatePerSecond, int depth)
    {
        int totalMachines = 0; // Initialize the total machines counter

        foreach (var component in components)
        {
            // Check if the component is a raw material; if so, skip it
            if (component.Key.IsRawMaterial)
            {
                continue;
            }

            // Calculate the total quantity needed per second for this component
            float totalNeededPerSecond = ((float)component.Value / component.Key.NumCreatedPerCraft) * productionRatePerSecond;

            // Get the crafting time of this component
            float craftingTime = component.Key.BaseTimeToCraft;

            // Calculate the number of machines needed based on crafting time
            float machinesNeeded = totalNeededPerSecond * craftingTime;

            // Add indentation based on depth (2 spaces per depth level)
            sb.AppendLine($"{new string(' ', depth * 2)}- {component.Key.Name}: {Math.Ceiling(machinesNeeded)} machines");

            // Add to total machines count
            totalMachines += (int)Math.Ceiling(machinesNeeded);

            // Recursively calculate for subcomponents with increased depth
            totalMachines += component.Key.CalculateMachinesRecursively(sb, component.Key.Components, totalNeededPerSecond, depth + 1);
        }

        return totalMachines; // Return the total number of machines calculated
    }

}