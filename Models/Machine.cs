using FactorioCalculator.Models.Interfaces;

namespace FactorioCalculator.Models;

public readonly record struct Assembler(
    MachineType MachineType,
    Quality Quality,
    float CraftingSpeed,
    float EnergyConsumption,
    float BaseProductivity,
    int ModuleSlots
) : IMachine
{
    public MachineCategory MachineCategory => MachineCategory.Assembly;
}

public readonly record struct Miner(
    MachineType MachineType,
    Quality Quality,
    float MiningSpeed,
    float EnergyConsumption,
    int ModuleSlots,
    float BaseProductivity,
    float ResourceDrain
) : IMachine
{
    public MachineCategory MachineCategory => MachineCategory.Mining;
}

public readonly record struct Furnace(
    MachineType MachineType,
    Quality Quality,
    float CraftingSpeed,
    float EnergyConsumption,
    int ModuleSlots
) : IMachine
{
    public MachineCategory MachineCategory => MachineCategory.Furnace;
}

public readonly record struct OffshorePump(
    MachineType MachineType,
    Quality Quality,
    int PumpingSpeed,
    int ModuleSlots
) : IMachine
{
    public MachineCategory MachineCategory => MachineCategory.OffshorePump;
}
