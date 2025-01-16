using NLua;

namespace FactorioCalculator;
public class RecipeManager
{
    private readonly Dictionary<string, Recipe> _recipes;
    private readonly Dictionary<string, List<Recipe>> _itemRecipesMap;

    public Dictionary<string, Recipe> GetAllRecipes()
    {
        return _recipes;
    }

    public Dictionary<string, List<Recipe>> GetAllItemRecipes()
    {
        return _itemRecipesMap;
    }

    public RecipeManager(List<string> modList = null)
    {
        modList ??= [];
        //ORDER MATTERS! 
        modList.Insert(0, "base");
        _recipes = [];
        _itemRecipesMap = [];

        List<Recipe> recipesList = ProcessLua.LoadRecipesData(new Lua(), modList);
        foreach (Recipe r in recipesList)
        {
            _recipes[r.Name] = r;

            foreach (Result result in r.Results.Values)
            {
                if (!_itemRecipesMap.ContainsKey(result.Name))
                {
                    _itemRecipesMap[result.Name] = [];
                }
                _itemRecipesMap[result.Name].Add(r);
            }
        }
    }
}