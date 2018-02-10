using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public Enemy enemy;
	public GameObject blackFade;
	public Text gameOverText;

	public AudioClip[] prowlingSounds;
	public AudioClip[] walkSounds;
	public AudioClip[] attackSounds1;
	public AudioClip[] attackSounds2;
	public AudioClip[] attackSounds3;
	public AudioClip[] attackSounds4;
	public AudioClip[] attackSounds5;

	public List<EnemySpawn> spawnZones = new List<EnemySpawn> ();

	private AudioClip[][] attackSoundMaster;
	private List<Area> areas = new List<Area> ();

	private GameObject player;
	private SpawnInstance nextSpawn;
	private string devMode;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");

		attackSoundMaster = new AudioClip[5][];
		attackSoundMaster [0] = attackSounds1;
		attackSoundMaster [1] = attackSounds2;
		attackSoundMaster [2] = attackSounds3;
		attackSoundMaster [3] = attackSounds4;
		attackSoundMaster [4] = attackSounds5;

		devMode = Scenes.getParam ("devMode");
		if (devMode == null) {
			devMode = "false";
		}

		foreach (GameObject area in GameObject.FindGameObjectsWithTag ("Area")) {
			areas.Add(area.GetComponent<Area>());
		}

		RecipeBook.loadRecipes (System.IO.Path.Combine(Application.streamingAssetsPath, "RecipeMaster.csv"));

		if (devMode == "false") {
			nextSpawn = SpawnMap.getNextSpawn ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if ((Scenes.getParam("devMode") == "true") && Input.GetKeyDown ("t")) {
			spawnEnemyRand ();
		}

		if (nextSpawn != null && Time.timeSinceLevelLoad >= nextSpawn.spawnTime && Scenes.getParam("devMode") == "false") {
			for (int i = 0; i < nextSpawn.spawnCount; i++) {
				spawnEnemyRand ();
			}
			nextSpawn = SpawnMap.getNextSpawn ();
		}
	}

	void spawnEnemyRand() {
		//Randomly select a spawn location based on spawn weighting
		List<EnemySpawn> randomizer = new List<EnemySpawn>();
		foreach (EnemySpawn spawner in spawnZones) {
			for(int i=0; i < spawner.weight; i++) {
				randomizer.Add (spawner);
			}
		}
		EnemySpawn spawnZone = randomizer[Random.Range(0, randomizer.Count)];
		float spawnLocX = spawnZone.transform.position.x;
		spawnLocX += Random.Range (spawnZone.maxOffset * -1, spawnZone.maxOffset + 1);

		//Create and position the enemy
		Enemy newEnemy = Instantiate (enemy, new Vector3(spawnLocX, enemy.transform.position.y, 0), Quaternion.identity);
		//Invulnerable while they are spawning in
		enemy.isInvunlerable = true;

		//TODO Change this to use the spawners area once enemy spawners are built
		Area spawnArea = spawnZone.GetComponentInParent<Area>();
		newEnemy.setCurrentArea (spawnArea);
		Debug.Log (newEnemy.getCurrentArea());

		//Select a random walk, prowl and attack sound and assign them to the new enemy
		int vocalType = Random.Range(0,5);
		newEnemy.setProwlSound(prowlingSounds[vocalType]);
		newEnemy.addAttackSound (attackSoundMaster[vocalType]);
		newEnemy.setWalkSound(walkSounds[Random.Range(0,5)]);

		//Fade the enemy sprite in from black
		StartCoroutine("spawnFade", newEnemy);
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

	public void fadeToMenu() {
		//Set any existing enemies to idle
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		foreach (GameObject enemy in enemies) {
			enemy.GetComponent<Enemy> ().deactivate ();
		}

		StartCoroutine ("deathFade");
	}

	IEnumerator spawnFade(Enemy enemy) {
		SpriteRenderer enemySprite = enemy.gameObject.GetComponent<SpriteRenderer> ();
		enemySprite.color = new Color(0.0f, 0.0f, 0.0f);

		for (float f = 0.0f; f < 1.0f; f += 0.02f) {
			Color c = enemySprite.color;
			c.r = f;
			c.g = f;
			c.b = f;
			enemySprite.color = c;

			yield return null;
		}

		enemy.isInvunlerable = false;
		enemy.activate ();
	}

	IEnumerator deathFade() {
		blackFade.SetActive (true);
		SpriteRenderer sprite = blackFade.GetComponent<SpriteRenderer> ();
		for (float f = 0.0f; f < 1.0f; f += 0.003f) {
			Color spriteColor = sprite.color;
			Color textColor = gameOverText.color;

			spriteColor.a = f;
			textColor.a = f;

			sprite.color = spriteColor;
			gameOverText.color = textColor;

			yield return null;
		}

		yield return new WaitForSeconds (2.0f);

		SceneManager.LoadScene ("MainMenu");
	}
}
