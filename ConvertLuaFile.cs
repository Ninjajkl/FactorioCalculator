using NLua;
using System.Text.Json;

class ConvertLuaFile
{
    public static void convertFile()
    {
        // Path to the Lua file
        string luaFilePath = "C:\\Users\\Ninja\\Documents\\Programming\\Random\\Factorio\\FactorioCalculator\\baseRecipe.lua";

        // Read the Lua script from the file
        string luaScript = File.ReadAllText(luaFilePath);

        // Initialize the Lua interpreter
        using (var lua = new Lua())
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
            var luaTable = lua["data.extendedData"] as LuaTable;

            if (luaTable != null)
            {
                // Convert Lua table to a C# object
                var dataObject = ConvertLuaTableToList(luaTable);

                // Serialize the object to JSON
                string jsonOutput = JsonSerializer.Serialize(dataObject, new JsonSerializerOptions { WriteIndented = true });

                // Write the JSON to a file or console
                Console.WriteLine(jsonOutput);
                File.WriteAllText("C:\\Users\\Ninja\\Documents\\Programming\\Random\\Factorio\\FactorioCalculator\\baseRecipes.json", jsonOutput);
            }
            else
            {
                Console.WriteLine("No data found in the Lua file.");
            }
        }
    }

    // Convert LuaTable to a C# List (supports nested tables)
    static List<object> ConvertLuaTableToList(LuaTable table)
    {
        var list = new List<object>();

        foreach (var value in table.Values)
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
    static Dictionary<string, object> ConvertLuaTableToDictionary(LuaTable table)
    {
        var dictionary = new Dictionary<string, object>();

        foreach (var key in table.Keys)
        {
            var value = table[key];

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
