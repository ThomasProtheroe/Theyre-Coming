using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Recipe {
	private string ingredient1;
	private string ingredient2;

	private GameObject product;

	public Recipe(string firstIngredient, string secondIngredient, string productPath) {
		ingredient1 = firstIngredient;
		ingredient2 = secondIngredient;

		product = (GameObject) Resources.Load("Prefabs/" + productPath);
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

}

public static class RecipeBook {
	private static ArrayList recipes;

	static RecipeBook() {
		recipes = new ArrayList();
	}

	public static void addRecipe(Recipe newRecipe) {
		recipes.Add (newRecipe);
	}

	public static GameObject tryCraft(string item1, string item2) {
		GameObject product = null;
		foreach (Recipe current in recipes) {
			if (current.areIngredients(item1, item2)) {
				product = current.craft ();
				break;
			}
		}
		return product;
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
			Recipe newRecipe = new Recipe (columns [1], columns [2], columns [3]);
			addRecipe (newRecipe);
		}  
	}

	public static ArrayList getRecipes() {
		return recipes;
	}
}