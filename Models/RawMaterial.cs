using FactorioCalculator.Models.Interfaces;

namespace FactorioCalculator.Models;

public class RawMaterial : IItem
{
    public string Name { get; set; }
    public float QuantityNeededPerSecond { get; set; }

    public RawMaterial(string name, float quantityNeededPerSecond)
    {
        Name = name;
        QuantityNeededPerSecond = quantityNeededPerSecond;
    }
}