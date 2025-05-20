using FactorioCalculator.Models.Interfaces;

namespace FactorioCalculator.Models;

public class Assembler(MachineType mt, Quality q, float cs, float es, int ms, float bp) : IMachine
{
    public MachineType MachineType { get; } = mt;
    public Quality Quality { get; } = q;
    public MachineCategory MachineCategory { get; } = MachineCategory.Assembly;
    public float CraftingSpeed { get; } = cs;
    public float EnergyConsumption { get; } = es;
    public int ModuleSlots { get; } = ms;
    public float BaseProductivity { get; } = bp;
}

public class Miner(MachineType mt, Quality q, float ms, float ec, int mods, float bp, float rd) : IMachine
{
    public MachineType MachineType { get; } = mt;
    public Quality Quality { get; } = q;
    public MachineCategory MachineCategory { get; } = MachineCategory.Mining;
    public float MiningSpeed { get; } = ms;
    public float EnergyConsumption { get; } = ec;
    public int ModuleSlots { get; } = mods;
    public float BaseProductivity { get; } = bp;
    public float ResourceDrain { get; } = rd;
}

public class Furnace(MachineType mt, Quality q, float cs, float es, int ms) : IMachine
{
    public MachineType MachineType { get; } = mt;
    public Quality Quality { get; } = q;
    public MachineCategory MachineCategory { get; } = MachineCategory.Furnace;
    public float CraftingSpeed { get; } = cs;
    public float EnergyConsumption { get; } = es;
    public int ModuleSlots { get; } = ms;
}

public class OffshorePump(MachineType mt, Quality q, int ps) : IMachine
{
    public MachineType MachineType { get; } = mt;
    public Quality Quality { get; } = q;
    public MachineCategory MachineCategory { get; } = MachineCategory.OffshorePump;
    public int PumpingSpeed { get; }
}