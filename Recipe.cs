namespace FactorioCalculator
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public partial class Recipe
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("allow_productivity")]
        public bool? AllowProductivity { get; set; }

        [JsonPropertyName("type")]
        public RecipeType Type { get; set; }

        [JsonPropertyName("results")]
        public Dictionary<string, Result> Results { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("show_amount_in_title")]
        public bool? ShowAmountInTitle { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("energy_required")]
        public double? EnergyRequired { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("allow_decomposition")]
        public bool? AllowDecomposition { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("order")]
        public string Order { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("subgroup")]
        public string Subgroup { get; set; }

        [JsonPropertyName("ingredients")]
        public Dictionary<string, Ingredient> Ingredients { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("enabled")]
        public bool? Enabled { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("crafting_machine_tint")]
        public CraftingMachineTint CraftingMachineTint { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("surface_conditions")]
        public SurfaceConditions SurfaceConditions { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("result_is_always_fresh")]
        public bool? ResultIsAlwaysFresh { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("hide_from_signal_gui")]
        public bool? HideFromSignalGui { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("auto_recycle")]
        public bool? AutoRecycle { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("hidden")]
        public bool? Hidden { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("main_product")]
        public string MainProduct { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("localised_name")]
        public LocalisedName LocalisedName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("always_show_products")]
        public bool? AlwaysShowProducts { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("allow_quality")]
        public bool? AllowQuality { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("preserve_products_in_machine_output")]
        public bool? PreserveProductsInMachineOutput { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("hide_from_player_crafting")]
        public bool? HideFromPlayerCrafting { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("icons")]
        public Dictionary<string, Icon> Icons { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("always_show_made_in")]
        public bool? AlwaysShowMadeIn { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("requester_paste_multiplier")]
        public long? RequesterPasteMultiplier { get; set; }

        public override string ToString()
        {
            return $"Recipe Name: {Name}\n" +
                   $"Type: {Type}\n" +
                   $"Energy Required: {EnergyRequired}\n" +
                   $"Ingredients:\n{IngredientsToString()}\n" +
                   $"Results:\n{ResultsToString()}";
        }

        private string IngredientsToString()
        {
            List<string> ingredientsList = new();
            foreach (Ingredient ingredient in Ingredients.Values)
            {
                ingredientsList.Add($"\t{ingredient.Amount}x {ingredient.Name} ({ingredient.Type})");
            }
            return string.Join("\n", ingredientsList);
        }

        private string ResultsToString()
        {
            List<string> resultsList = new();
            foreach (Result result in Results.Values)
            {
                resultsList.Add($"\t{result.Amount}x {result.Name} ({result.Type})");
            }
            return string.Join("\n", resultsList);
        }
    }

    public partial class CraftingMachineTint
    {
        [JsonPropertyName("primary")]
        public PrimaryClass Primary { get; set; }

        [JsonPropertyName("secondary")]
        public PrimaryClass Secondary { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("quaternary")]
        public QuaternaryClass Quaternary { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("tertiary")]
        public QuaternaryClass Tertiary { get; set; }
    }

    public partial class PrimaryClass
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("r")]
        public double? R { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("g")]
        public double? G { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("b")]
        public double? B { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("a")]
        public double? A { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("1")]
        public double? The1 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("2")]
        public double? The2 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("3")]
        public long? The3 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("4")]
        public long? The4 { get; set; }
    }

    public partial class QuaternaryClass
    {
        [JsonPropertyName("r")]
        public double R { get; set; }

        [JsonPropertyName("g")]
        public double G { get; set; }

        [JsonPropertyName("b")]
        public double B { get; set; }

        [JsonPropertyName("a")]
        public double A { get; set; }
    }

    public partial class Icon
    {
        [JsonPropertyName("icon")]
        public string IconIcon { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("scale")]
        public double? Scale { get; set; }
    }

    public partial class Ingredient
    {
        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        [JsonPropertyName("type")]
        public IngredientType Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("ignored_by_stats")]
        public long? IgnoredByStats { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("fluidbox_multiplier")]
        public long? FluidboxMultiplier { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("fluidbox_index")]
        public long? FluidboxIndex { get; set; }
    }

    public partial class LocalisedName
    {
        [JsonPropertyName("1")]
        public string The1 { get; set; }
    }

    public partial class Result
    {
        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        [JsonPropertyName("type")]
        public IngredientType Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("probability")]
        public double? Probability { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("percent_spoiled")]
        public double? PercentSpoiled { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("ignored_by_stats")]
        public long? IgnoredByStats { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("ignored_by_productivity")]
        public long? IgnoredByProductivity { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("temperature")]
        public long? Temperature { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("show_details_in_recipe_tooltip")]
        public bool? ShowDetailsInRecipeTooltip { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("fluidbox_index")]
        public long? FluidboxIndex { get; set; }
    }

    public partial class SurfaceConditions
    {
        [JsonPropertyName("1")]
        public The1 The1 { get; set; }
    }

    public partial class The1
    {
        [JsonPropertyName("property")]
        public Property Property { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("min")]
        public long? Min { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("max")]
        public long? Max { get; set; }
    }

    public enum IngredientType { Fluid, Item };

    public enum Property { Gravity, MagneticField, Pressure };

    public enum RecipeType { Recipe };

    public static class Converter
    {
        public static readonly JsonSerializerOptions Settings = new(JsonSerializerDefaults.General)
        {
            Converters =
            {
                IngredientTypeConverter.Singleton,
                PropertyConverter.Singleton,
                RecipeTypeConverter.Singleton,
                new DateOnlyConverter(),
                new TimeOnlyConverter(),
                IsoDateTimeOffsetConverter.Singleton
            },
        };
    }

    internal class IngredientTypeConverter : JsonConverter<IngredientType>
    {
        public override bool CanConvert(Type t)
        {
            return t == typeof(IngredientType);
        }

        public override IngredientType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();
            return value switch
            {
                "fluid" => IngredientType.Fluid,
                "item" => IngredientType.Item,
                _ => throw new Exception("Cannot unmarshal type IngredientType"),
            };
        }

        public override void Write(Utf8JsonWriter writer, IngredientType value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case IngredientType.Fluid:
                    JsonSerializer.Serialize(writer, "fluid", options);
                    return;
                case IngredientType.Item:
                    JsonSerializer.Serialize(writer, "item", options);
                    return;
            }
            throw new Exception("Cannot marshal type IngredientType");
        }

        public static readonly IngredientTypeConverter Singleton = new();
    }

    internal class PropertyConverter : JsonConverter<Property>
    {
        public override bool CanConvert(Type t)
        {
            return t == typeof(Property);
        }

        public override Property Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();
            return value switch
            {
                "gravity" => Property.Gravity,
                "magnetic-field" => Property.MagneticField,
                "pressure" => Property.Pressure,
                _ => throw new Exception("Cannot unmarshal type Property"),
            };
        }

        public override void Write(Utf8JsonWriter writer, Property value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case Property.Gravity:
                    JsonSerializer.Serialize(writer, "gravity", options);
                    return;
                case Property.MagneticField:
                    JsonSerializer.Serialize(writer, "magnetic-field", options);
                    return;
                case Property.Pressure:
                    JsonSerializer.Serialize(writer, "pressure", options);
                    return;
            }
            throw new Exception("Cannot marshal type Property");
        }

        public static readonly PropertyConverter Singleton = new();
    }

    internal class RecipeTypeConverter : JsonConverter<RecipeType>
    {
        public override RecipeType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();
            if (value == "recipe")
            {
                return RecipeType.Recipe;
            }
            throw new Exception("Cannot unmarshal type RecipeType");
        }

        public override void Write(Utf8JsonWriter writer, RecipeType value, JsonSerializerOptions options)
        {
            if (value == RecipeType.Recipe)
            {
                writer.WriteStringValue("recipe");
                return;
            }
            throw new Exception("Cannot marshal type RecipeType");
        }

        public static readonly RecipeTypeConverter Singleton = new();
    }

    public class DateOnlyConverter : JsonConverter<DateOnly>
    {
        private readonly string serializationFormat;
        public DateOnlyConverter() : this(null) { }

        public DateOnlyConverter(string? serializationFormat)
        {
            this.serializationFormat = serializationFormat ?? "yyyy-MM-dd";
        }

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();
            return DateOnly.Parse(value!);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(serializationFormat));
        }
    }

    public class TimeOnlyConverter : JsonConverter<TimeOnly>
    {
        private readonly string serializationFormat;

        public TimeOnlyConverter() : this(null) { }

        public TimeOnlyConverter(string? serializationFormat)
        {
            this.serializationFormat = serializationFormat ?? "HH:mm:ss.fff";
        }

        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();
            return TimeOnly.Parse(value!);
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(serializationFormat));
        }
    }

    internal class IsoDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        public override bool CanConvert(Type t)
        {
            return t == typeof(DateTimeOffset);
        }

        private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
        private string? _dateTimeFormat;
        private CultureInfo? _culture;

        public DateTimeStyles DateTimeStyles { get; set; } = DateTimeStyles.RoundtripKind;

        public string? DateTimeFormat
        {
            get => _dateTimeFormat ?? string.Empty;
            set => _dateTimeFormat = string.IsNullOrEmpty(value) ? null : value;
        }

        public CultureInfo Culture
        {
            get => _culture ?? CultureInfo.CurrentCulture;
            set => _culture = value;
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            string text;


            if ((DateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
                    || (DateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
            {
                value = value.ToUniversalTime();
            }

            text = value.ToString(_dateTimeFormat ?? DefaultDateTimeFormat, Culture);

            writer.WriteStringValue(text);
        }

        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? dateText = reader.GetString();

            if (string.IsNullOrEmpty(dateText) == false)
            {
                if (!string.IsNullOrEmpty(_dateTimeFormat))
                {
                    return DateTimeOffset.ParseExact(dateText, _dateTimeFormat, Culture, DateTimeStyles);
                }
                else
                {
                    return DateTimeOffset.Parse(dateText, Culture, DateTimeStyles);
                }
            }
            else
            {
                return default;
            }
        }


        public static readonly IsoDateTimeOffsetConverter Singleton = new();
    }
}
