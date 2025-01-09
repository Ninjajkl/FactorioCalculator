namespace FactorioCalculator
{
    public class Recipe
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public bool Enabled { get; set; }
        public double EnergyRequired { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public List<Result> Results { get; set; }
        public bool? AllowProductivity { get; set; }
        public string Icon { get; set; }
        public string Subgroup { get; set; }
        public string Order { get; set; }
        public bool? AllowDecomposition { get; set; }
        public bool? ShowAmountInTitle { get; set; }
        public CraftingMachineTint CraftingMachineTint { get; set; }
        public List<SurfaceCondition> SurfaceConditions { get; set; }
        public bool? ResultIsAlwaysFresh { get; set; }
    }

    public class Ingredient
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
    }

    public class Result
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public double? Probability { get; set; }
    }

    public class CraftingMachineTint
    {
        public Color Primary { get; set; }
        public Color Secondary { get; set; }
    }

    public class SurfaceCondition
    {
        public string Property { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
    }

    public class Color
    {
        public double R { get; set; }
        public double G { get; set; }
        public double B { get; set; }
        public double A { get; set; }
    }

}
