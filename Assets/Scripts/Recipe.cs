using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Recipe {
	private string ingredient1;
	private string ingredient2;

	private GameObject product;
	private string byProduct; 
	private int craftingCostOverride;

	public Recipe(string firstIngredient, string secondIngredient, string productPath, string byProductType, string costOverride) {
		ingredient1 = firstIngredient;
		ingredient2 = secondIngredient;

		product = (GameObject) Resources.Load("Prefabs/" + productPath);
		byProduct = byProductType;
		if (!string.IsNullOrEmpty(costOverride)) {
			craftingCostOverride = int.Parse(costOverride);
		}
	}

	public bool areIngredients(string item1, string item2) {
		if ((item1 == ingredient1 && item2 == ingredient2) || (item1 == ingredient2 && item2 == ingredient1)) {
			return true;
		}
		return false;
	}
		
	public GameObject craft() {
		return Object.Instantiate (product);
	}

	public string getByProduct() {
		return byProduct;
	}

	public int getCostOverride() {
		return craftingCostOverride;
	}

	public int getCraftingCost(string phase) {
		int baseCost;
		if (craftingCostOverride > 0) {
			baseCost = craftingCostOverride;
		} else {
			baseCost = Constants.STAMINA_COST_CRAFT_DEFAULT;
		}
		
		int cost = 0;
		if (phase == "downtime") {
			cost = baseCost;
		} else if (phase == "siege") {
			cost = (int)Mathf.Floor((float)baseCost / 2);
		}

		return cost;
	}

}

public class Result {
	public GameObject product;
	public string byProduct;
}

public static class RecipeBook {
	private static ArrayList recipes;

	static RecipeBook() {
		recipes = new ArrayList();
	}

	public static void addRecipe(Recipe newRecipe) {
		recipes.Add (newRecipe);
	}

	public static Result tryCraft(string item1, string item2) {
		Result result = null;
		foreach (Recipe current in recipes) {
			if (current.areIngredients(item1, item2)) {
				result = new Result ();
				result.product = current.craft ();
				result.product.GetComponent<Item> ().setCraftingCostOverride(current.getCostOverride());
				result.byProduct = current.getByProduct();
				break;
			}
		}
		return result;
	}

	public static bool canCraft(string item1, string item2) {
		bool craftable = false;
		foreach (Recipe current in recipes) {
			if (current.areIngredients(item1, item2)) {
				craftable = true;
				break;
			}
		}
		return craftable;
	}

	public static void loadRecipes(string path) {
		string line = "";
		StreamReader reader = new StreamReader(path); 

		while((line = reader.ReadLine()) != null)  
		{  
			string[] columns = line.Split (',');
			
			Recipe newRecipe = new Recipe (columns [1], columns [2], columns [3], columns [4], columns [5]);
			addRecipe (newRecipe);
		}  
	}

	public static ArrayList getRecipes() {
		return recipes;
	}

	public static int getCraftingCost(string item1, string item2, string phase) {
		int cost = -1;
		foreach (Recipe current in recipes) {
			if (current.areIngredients(item1, item2)) {
				cost = current.getCraftingCost(phase);
				break;
			}
		}
		return cost;
	}
}