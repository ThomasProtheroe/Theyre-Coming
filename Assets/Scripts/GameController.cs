using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public Enemy enemy;
	public AudioClip[] prowlingSounds;
	public AudioClip[] walkSounds;

	private GameObject player;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");

		RecipeBook.loadRecipes (System.IO.Path.Combine(Application.streamingAssetsPath, "RecipeMaster.csv"));
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("t")) {
			float xPos;
			if (player.transform.position.x < -30) {
				xPos = -38.0f;
			} else {
				xPos = 6.0f;
			}

			spawnEnemy (xPos);
		}
	}

	void spawnEnemy(float xPos) {
		Enemy newEnemy = Instantiate (enemy, new Vector3(xPos, enemy.transform.position.y, 0), Quaternion.identity);

		//Select a random walk and prowl sound and assign them to the new enemy
		newEnemy.setProwlSound(prowlingSounds[Random.Range(0,5)]);
		newEnemy.setWalkSound(walkSounds[Random.Range(0,5)]);

		newEnemy.startProwlSound ();
	}
}
