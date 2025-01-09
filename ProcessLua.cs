using FactorioCalculator;
using NLua;
using System.Text.Json;

internal class ProcessLua
{
    public static List<Recipe> LoadModRecipesData(string modName)
    {
        // Path to the Lua file
        string luaFilePath = $"..\\..\\..\\{modName}.lua";

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
                File.WriteAllText("..\\..\\..\\qualityRecipes.json", jsonOutput);
            }
            else
            {
                Console.WriteLine("No data found in the Lua file.");
            }
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