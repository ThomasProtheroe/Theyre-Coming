using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public Enemy enemy;
	public AudioClip[] prowlingSounds;
	public AudioClip[] walkSounds;
	public AudioClip[] attackSounds1;
	public AudioClip[] attackSounds2;
	public AudioClip[] attackSounds3;
	public AudioClip[] attackSounds4;
	public AudioClip[] attackSounds5;

	private AudioClip[][] attackSoundMaster;
	private List<Area> areas = new List<Area>();

	private GameObject player;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");

		attackSoundMaster = new AudioClip[5][];
		attackSoundMaster [0] = attackSounds1;
		attackSoundMaster [1] = attackSounds2;
		attackSoundMaster [2] = attackSounds3;
		attackSoundMaster [3] = attackSounds4;
		attackSoundMaster [4] = attackSounds5;

		foreach (GameObject area in GameObject.FindGameObjectsWithTag ("Area")) {
			areas.Add(area.GetComponent<Area>());
		}

		RecipeBook.loadRecipes (System.IO.Path.Combine(Application.streamingAssetsPath, "RecipeMaster.csv"));
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("t")) {
			float xPos = 6.0f;

			spawnEnemy (xPos);
		}
	}

	void spawnEnemy(float xPos) {
		Enemy newEnemy = Instantiate (enemy, new Vector3(xPos, enemy.transform.position.y, 0), Quaternion.identity);

		//TODO Change this to use the spawners area once enemy spawners are built
		foreach (Area area in areas) {
			if (area.name == "Yard") {
				newEnemy.setCurrentArea (area);
			}
		}

		//Select a random walk, prowl and attack sound and assign them to the new enemy
		int vocalType = Random.Range(0,5);
		newEnemy.setProwlSound(prowlingSounds[vocalType]);
		newEnemy.addAttackSound (attackSoundMaster[vocalType]);
		newEnemy.setWalkSound(walkSounds[Random.Range(0,5)]);
	}

	public Transition findRouteToPlayer(Area currentArea) {
		Area playerArea = player.GetComponent<PlayerController> ().getCurrentArea ();

		foreach (Transition transition in currentArea.transitions) {
			if (transition.sibling.transform.parent.gameObject.GetComponent<Area> () == playerArea) {
				return transition;
			}
		}
		//Should never happen (handle as error)
		return new Transition();
	}
}
