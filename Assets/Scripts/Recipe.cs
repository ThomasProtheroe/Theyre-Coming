using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Recipe {
	private string ingredient1;
	private string ingredient2;

	private GameObject product;
	private string byProduct; 

	public Recipe(string firstIngredient, string secondIngredient, string productPath, string byProductType) {
		ingredient1 = firstIngredient;
		ingredient2 = secondIngredient;

		product = (GameObject) Resources.Load("Prefabs/" + productPath);
		byProduct = byProductType;
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
			Recipe newRecipe = new Recipe (columns [1], columns [2], columns [3], columns [4]);
			addRecipe (newRecipe);
		}  
	}

	public static ArrayList getRecipes() {
		return recipes;
	}
}