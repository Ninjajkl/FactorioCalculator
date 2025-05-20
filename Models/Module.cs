namespace FactorioCalculator.Models;

public readonly record struct Module(
    ModuleType ModuleType,
    Quality Quality,
    float SpeedModifier,
    float EnergyModifier,
    float ProductivityModifier,
    float QualityModifier
);