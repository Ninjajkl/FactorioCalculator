namespace FactorioCalculator.Models.Interfaces;

public interface IItem
{
    string Name { get; set; }
    float QuantityNeededPerSecond { get; set; }
}