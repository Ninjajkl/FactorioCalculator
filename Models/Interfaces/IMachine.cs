namespace FactorioCalculator.Models.Interfaces;

public interface IMachine
{
    MachineType MachineType { get; }
    Quality Quality { get; }
    MachineCategory MachineCategory { get; }
    int ModuleSlots { get; }
}