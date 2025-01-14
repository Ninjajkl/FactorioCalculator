using FactorioCalculator;
using NLua;
using System.Text.Json;

internal class ProcessLua
{
    public static List<Recipe> LoadModRecipesData(string modName)
    {
        // Path to the Lua file
        string luaFilePath = $"..\\..\\..\\{modName}Recipes.lua";

        // Read the Lua script from the file
        string luaScript = File.ReadAllText(luaFilePath);

        // Initialize the Lua interpreter
        using (Lua lua = new())
        {
            // Create a 'data' table to collect the results
            lua.NewTable("data");

            // Define a stub for 'data:extend'
            lua.DoString(@"
                data.extendedData = {}
                function data:extend(entries)
                    for _, entry in ipairs(entries) do
                        table.insert(data.extendedData, entry)
                    end
                end
            ");

            // Execute the Lua script
            lua.DoString(luaScript);

            // Retrieve the 'data.extendedData' table

            if (lua["data.extendedData"] is LuaTable luaTable)
            {
                // Convert Lua table to a C# object
                List<object> dataObject = ConvertLuaTableToList(luaTable);

                // Serialize the object to JSON
                string jsonOutput = JsonSerializer.Serialize(dataObject, new JsonSerializerOptions { WriteIndented = true });

                // Write the JSON to a file or console
                Console.WriteLine(jsonOutput);
                File.WriteAllText($"..\\..\\..\\{modName}Recipes.json", jsonOutput);

                // Return the deserialized list of recipes
                return JsonSerializer.Deserialize<List<Recipe>>(jsonOutput, Converter.Settings); ;
            }
            else
            {
                Console.WriteLine("No data found in the Lua file.");
            }
        }
        return null;
    }

    public static void UpdateRecipesData(string modName, List<Recipe> recipes)
    {
        // Read the Lua script from the update file
        string[] luaScriptLines = File.ReadAllLines($"..\\..\\..\\{modName}DataUpdates.lua");

        // Filter lines that contain 'data.raw.recipe' and handle multi-line entries
        List<string> filteredLines = [];
        bool inRecipeSection = false;
        int openBracesCount = 0;

        foreach (string line in luaScriptLines)
        {
            if (line.Contains("data.raw.recipe"))
            {
                inRecipeSection = true;
            }

            if (inRecipeSection)
            {
                filteredLines.Add(line);

                // Count opening and closing braces to determine when a section ends
                openBracesCount += line.Count(c => c == '{');
                openBracesCount -= line.Count(c => c == '}');

                if (openBracesCount == 0 && line.Trim().EndsWith("}"))
                {
                    inRecipeSection = false;
                }
            }
        }

        // Join the filtered lines back into a single script
        string filteredLuaScript = string.Join("\n", filteredLines);

        // Initialize the Lua interpreter
        using (Lua lua = new())
        {
            // Load the existing recipes into the Lua environment
            lua["data"] = new { raw = new { recipe = recipes } };

            // Execute the Lua script to update the recipes
            lua.DoString(filteredLuaScript);

            // Retrieve the updated recipes
            if (lua["data.raw.recipe"] is LuaTable luaTable)
            {
                // Convert Lua table to a C# object
                List<object> updatedDataObject = ConvertLuaTableToList(luaTable);

                // Serialize the object to JSON
                string updatedJsonOutput = JsonSerializer.Serialize(updatedDataObject, new JsonSerializerOptions { WriteIndented = true });

                // Write the updated JSON to a file or console
                Console.WriteLine(updatedJsonOutput);
                File.WriteAllText($"..\\..\\..\\{modName}DataUpdates.json", updatedJsonOutput);
            }
            else
            {
                Console.WriteLine("No data found in the Lua file.");
            }
        }
    }

    public static void PrintLuaTable(LuaTable luaTable)
    {
        foreach (object? key in luaTable.Keys)
        {
            Console.WriteLine($"{key}: {luaTable[key]}");
        }
    }

    public static void LoadAndPrintRecipes(LuaTable recipes)
    {
        using (Lua lua = new())
        {
            // Load the existing recipes into the Lua environment
            lua["data"] = new { raw = new { recipe = recipes } };

            // Print the contents of lua["data"]
            if (lua["data"] is LuaTable dataTable)
            {
                PrintLuaTable(dataTable);
            }
            else
            {
                Console.WriteLine("No data found in the Lua environment.");
            }
        }

    // Convert LuaTable to a C# List (supports nested tables)
    private static List<object> ConvertLuaTableToList(LuaTable table)
    {
        List<object> list = [];

        foreach (object? value in table.Values)
        {
            if (value is LuaTable subTable)
            {
                // Handle nested tables
                list.Add(ConvertLuaTableToDictionary(subTable));
            }
            else
            {
                list.Add(value);
            }
        }

        return list;
    }

    // Convert LuaTable to a C# Dictionary
    private static Dictionary<string, object> ConvertLuaTableToDictionary(LuaTable table)
    {
        Dictionary<string, object> dictionary = [];

        foreach (object? key in table.Keys)
        {
            object value = table[key];

            if (value is LuaTable subTable)
            {
                dictionary[key.ToString()] = ConvertLuaTableToDictionary(subTable);
            }
            else
            {
                dictionary[key.ToString()] = value;
            }
        }

        return dictionary;
    }
}