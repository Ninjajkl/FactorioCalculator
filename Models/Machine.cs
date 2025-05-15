namespace FactorioCalculator.Models;

public class Machine
{
    public readonly MachineType MachineType;
    public readonly Quality Quality;
    public readonly float CraftingSpeed;
    public readonly float EnergyConsumption;
    public readonly int ModuleSlots;
    public readonly float BaseProductivity;

    public Machine(MachineType mt, Quality q)
    {
        MachineType = mt;
        Quality = q;

    }
}
