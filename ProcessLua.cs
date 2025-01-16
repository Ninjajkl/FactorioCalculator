using FactorioCalculator;
using NLua;
using System.Text.Json;

internal class ProcessLua
{
    public static List<Recipe> LoadRecipesData(Lua lua, List<string> modNames)
    {
        // Create a 'data' table to collect the results
        lua.NewTable("data");

        // Define a stub for 'data:extend'
        lua.DoString(@"
            data.raw = { recipe = {} }
            function data:extend(entries)
                for _, entry in ipairs(entries) do
                    data.raw.recipe[entry.name] = entry
                end
            end
        ");

        foreach (string modName in modNames)
        {
            // Path to the Lua file
            string dataFilePath = $"..\\..\\..\\GameFiles\\{modName}Recipes.lua";

            // Read the Lua script from the file
            string dataScript = File.ReadAllText(dataFilePath);

            // Execute the Lua script
            lua.DoString(dataScript);

            // Check if this Mod has updates
            string path = $"..\\..\\..\\GameFiles\\{modName}DataUpdates.lua";
            if (File.Exists(path))
            {
                // Read the Lua script from the update file
                string[] updateScriptLines = File.ReadAllLines(path);

                // Filter lines that contain 'data.raw.recipe' and handle multi-line entries
                List<string> filteredLines = [];
                bool inRecipeSection = false;
                bool hasBraces = false;
                int openBracesCount = 0;

                foreach (string line in updateScriptLines)
                {
                    if (line.Contains("data.raw.recipe"))
                    {
                        inRecipeSection = true;
                        hasBraces = line.Trim().EndsWith("=");
                    }

                    if (inRecipeSection)
                    {
                        filteredLines.Add(line);

                        // Count opening and closing braces to determine when a section ends
                        openBracesCount += line.Count(c => c == '{');
                        openBracesCount -= line.Count(c => c == '}');

                        if ((openBracesCount == 0 && line.Trim().EndsWith("}")) || !hasBraces)
                        {
                            inRecipeSection = false;
                        }
                    }
                }

                // Join the filtered lines back into a single script
                string filteredLuaScript = string.Join("\n", filteredLines);

                // Execute the Lua script to update the recipes
                lua.DoString(filteredLuaScript);
            }
        }

        if (lua["data.raw.recipe"] is LuaTable luaTable)
        {
            // Convert Lua table to a C# object
            List<object> dataObject = ConvertLuaTableToList(luaTable);

            // Serialize the object to JSON
            string jsonOutput = JsonSerializer.Serialize(dataObject, new JsonSerializerOptions { WriteIndented = true });

            // Write the JSON to a file or console
            //Console.WriteLine(jsonOutput);
            File.WriteAllText($"..\\..\\..\\Recipes.json", jsonOutput);

            // Return the deserialized list of recipes
            return JsonSerializer.Deserialize<List<Recipe>>(jsonOutput, Converter.Settings); ;
        }
        else
        {
            Console.WriteLine("No data found in the Lua file.");
        }

        return null;
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