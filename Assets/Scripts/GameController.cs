using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public int prepTime;
	public Enemy enemy;
	public GameObject blackFade;
	[SerializeField]
	private Text gameOverText;
	[SerializeField]
	private Text timerText;
	[SerializeField]
	private Text skipText;
	[SerializeField]
	private DialogPanel dialogPanel;
	[SerializeField]
	private DescriptionPanel descriptionPanel;
	[SerializeField]
	private Sprite[] dialogSprites;
	[SerializeField]
	private AudioClip[] cinematicSounds;

	public AudioSource source;
	public MusicController musicPlayer;
	public AudioClip[] prowlingSounds;
	public AudioClip[] walkSounds;
	public AudioClip[] attackSounds1;
	public AudioClip[] attackSounds2;
	public AudioClip[] attackSounds3;
	public AudioClip[] attackSounds4;
	public AudioClip[] attackSounds5;

	public List<EnemySpawn> spawnZones = new List<EnemySpawn> ();
	public Dictionary<string, List<EnemyCorpse>> corpseDict = new Dictionary<string, List<EnemyCorpse>> ();

	private AudioClip[][] attackSoundMaster;
	private List<Area> areas = new List<Area> ();
	private List<Area> checkedAreas = new List<Area> ();
	private List<Area> searchedAreas = new List<Area> ();

	private GameObject player;
	private PlayerController playerCon;
	private SpawnInstance nextSpawn;
	private float timer;
	private string phase;
	private string currentCinematic;
	private string devMode;
	[SerializeField]
	private int maxCorpseCount;
	[SerializeField]
	private int corpseCount;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		playerCon = player.GetComponent<PlayerController> ();
		source = GetComponent<AudioSource> ();

		SpawnMap.rebuildMap ();

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

		timer = 0.0f;
		if (devMode == "false") {
			startIntro ();
		} else {
			changePhase("siege");
			onSiegePhase ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;

		if (phase == "prep") {
			TimeSpan timeSpan = TimeSpan.FromSeconds((prepTime - Mathf.Floor (timer)));
			timerText.text = string.Format ("{0:D2}:{1:d2}", timeSpan.Minutes, timeSpan.Seconds);
			//timerText.text = (prepTime - Mathf.Floor (timer)).ToString();
			if (Mathf.Floor (timer) >= prepTime) {
				timerText.enabled = false;
				changePhase ("siege");
				onSiegePhase ();
			}
		} else if (phase == "siege") {
			if (nextSpawn != null && timer >= nextSpawn.spawnTime && Scenes.getParam ("devMode") == "false") {
				for (int i = 0; i < nextSpawn.spawnCount; i++) {
					Invoke("spawnEnemyRand", i * 0.4f);
				}
				nextSpawn = SpawnMap.getNextSpawn ();
			}
		}

		//Skip cinematics
		if (currentCinematic != null && Input.GetButtonDown("interact")) {
			switch (currentCinematic) {
			case "playIntro":
				StopCoroutine ("playIntro");
				StopCoroutine ("introFade");
				hideDialog ();
				stopIntro ();
				break;
			}
		}

		if ((Scenes.getParam("devMode") != "false") && Input.GetKeyDown ("t")) {
			spawnEnemyRand ();
		}
	}

	void changePhase(string newPhase) {
		phase = newPhase;
		musicPlayer.changeMusic (phase);
		timer = 0.0f;
	}

	private void onSiegePhase() {
		//Change front door to broken version
		GameObject.FindGameObjectWithTag("FrontDoor").GetComponent<FrontDoor> ().setSiegeMode();
		if (Scenes.getParam("devMode") != "false") {
			return;
		}
		spawnEnemy (spawnZones[1]);
	}

	void spawnEnemyRand() {
		//Randomly select a spawn location based on spawn weighting
		List<EnemySpawn> randomizer = new List<EnemySpawn>();
		foreach (EnemySpawn spawner in spawnZones) {
			for(int i=0; i < spawner.weight; i++) {
				randomizer.Add (spawner);
			}
		}
		EnemySpawn spawnZone = randomizer[UnityEngine.Random.Range(0, randomizer.Count)];

		spawnEnemy (spawnZone);
	}

	void spawnEnemy(EnemySpawn spawnZone) {
		float spawnLocX = spawnZone.transform.position.x;
		spawnLocX += UnityEngine.Random.Range (spawnZone.maxOffset * -1, spawnZone.maxOffset + 1);

		//Create and position the enemy
		Enemy newEnemy = Instantiate (enemy, new Vector3(spawnLocX, enemy.transform.position.y, 0), Quaternion.identity);
		//Invulnerable while they are spawning in
		enemy.isInvunlerable = true;

		Area spawnArea = spawnZone.GetComponentInParent<Area>();
		newEnemy.setCurrentArea (spawnArea);

		//Select a random walk, prowl and attack sound and assign them to the new enemy
		int vocalType = UnityEngine.Random.Range(0,5);
		newEnemy.setProwlSound(prowlingSounds[vocalType]);
		newEnemy.addAttackSound (attackSoundMaster[vocalType]);
		newEnemy.setWalkSound(walkSounds[UnityEngine.Random.Range(0,5)]);

		//Fade the enemy sprite in from black
		StartCoroutine("spawnFade", newEnemy);
	}

	public void addEnemyCorpse(EnemyCorpse corpse, string areaName) {
		List<EnemyCorpse> corpseList;
		if (corpseDict.ContainsKey (areaName)) {
			corpseList = corpseDict [areaName];
		} else {
			corpseList = new List<EnemyCorpse> ();
		}

		corpseList.Add (corpse);
		corpseDict[areaName] = corpseList;
		corpseCount++;
	}

	public void clearCorpses() {
		PlayerController playerCon = player.GetComponent<PlayerController> ();


		while (corpseCount > maxCorpseCount) {
			//Pick a random area, excluding the players current area so they don't see the despawns
			List<string> keyList = new List<string> (corpseDict.Keys);
			keyList.Remove(playerCon.currentArea.name);
			List<EnemyCorpse> corpseList = corpseDict [keyList [UnityEngine.Random.Range (0, keyList.Count)]];
			GameObject targetCorpse = corpseList [0].gameObject;
			corpseList.RemoveAt(0);
			Destroy(targetCorpse);
			corpseCount--;

			if (corpseList.Count == 0) {
				corpseDict.Remove (playerCon.currentArea.name);
			} else {
				corpseDict [playerCon.currentArea.name] = corpseList;
			}
		}
	}


	public Transition findRouteToPlayer(Area currentArea) {
		checkedAreas = new List<Area> ();
		searchedAreas = new List<Area> ();

		return searchForPlayer(currentArea, player.GetComponent<PlayerController> ().getCurrentArea ());
	}

	private Transition searchForPlayer(Area startingArea, Area playerArea, Transition closest=null) {
		Area transArea;
		foreach (Transition transition in startingArea.transitions) {
			transArea = transition.sibling.transform.parent.GetComponent<Area> ();
			if (checkedAreas.Contains(transArea)) {
				continue;
			}
			if (transArea == playerArea) {
				return transition;
			}
			checkedAreas.Add(transArea);
		}
		searchedAreas.Add(startingArea);

		Transition target = null;
		foreach(Transition transition in startingArea.transitions) {
			transArea = transition.sibling.transform.parent.GetComponent<Area> ();
			if (searchedAreas.Contains(transArea)) {
				continue;
			}
			if (closest == null) {
				closest = transition;
			}

			target = searchForPlayer(transArea, playerArea, closest);
			if (target) {
				return closest;
			}
		}

		return null;
	}

	public void fadeToMenu() {
		//Set any existing enemies to idle
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		foreach (GameObject enemy in enemies) {
			enemy.GetComponent<Enemy> ().deactivate ();
		}

		StartCoroutine ("deathFade");
	}

	public void showDialog(Dialog dialog) {
		if (descriptionPanel.gameObject.activeSelf) {
			hideDescription ();
		}
		dialogPanel.showDialog (dialog);
	}

	public void hideDialog() {
		dialogPanel.hideDialog ();
	}

	public void showDescription(string description) {
		if (dialogPanel.gameObject.activeSelf) {
			return;
		}
		descriptionPanel.showDescription (description);
	}

	public void hideDescription() {
		descriptionPanel.hideDescription ();
	}

	private void startIntro() {
		skipText.gameObject.SetActive (true);
		playerCon.itemSlot1.gameObject.SetActive(false);
		playerCon.itemSlot2.gameObject.SetActive(false);

		playerCon.enableCinematicControl (true);

		StartCoroutine ("introFade");
		StartCoroutine ("playIntro");
	}

	private void stopIntro() {
		blackFade.SetActive (false);
		changePhase("prep");
		timerText.enabled = true;
		nextSpawn = SpawnMap.getNextSpawn ();

		skipText.gameObject.SetActive (false);
		playerCon.itemSlot1.gameObject.SetActive(true);
		playerCon.itemSlot2.gameObject.SetActive(true);
		playerCon.enableCinematicControl (false);
		currentCinematic = null;
	}

	IEnumerator playIntro() {
		currentCinematic = "playIntro";

		source.PlayOneShot (cinematicSounds[0]);
		playerCon.moveInput = -1.0f;

		yield return new WaitForSeconds (0.7f);

		playerCon.moveInput = 0.0f;

		yield return new WaitForSeconds (1.0f);

		showDialog (new Dialog("Fiona:\nThat was way too close...", dialogSprites[0], 4.0f));

		yield return new WaitForSeconds (5.0f);

		playerCon.flipPlayer ();

		yield return new WaitForSeconds (1.0f);

		showDialog (new Dialog("What the hell were those things?", dialogSprites[0], 4.0f));

		yield return new WaitForSeconds (5.0f);

		playerCon.flipPlayer ();

		yield return new WaitForSeconds (1.0f);

		showDialog (new Dialog("They were right behind me, they'll be here any minute. Need to get ready for them. This knife isn't going to cut it.", dialogSprites[0], 5.0f));

		yield return new WaitForSeconds (3.0f);

		stopIntro ();
	}

	IEnumerator introFade() {
		SpriteRenderer sprite = blackFade.GetComponent<SpriteRenderer> ();
		Color startColor = sprite.color;
		startColor.a = 1.0f;
		blackFade.SetActive (true);
		for (float f = 1.0f; f > 0.9f; f -= 0.0015f) {
			Color spriteColor = sprite.color;

			spriteColor.a = f;

			sprite.color = spriteColor;

			yield return null;
		}
		for (float f = 0.9f; f > 0.6f; f -= 0.003f) {
			Color spriteColor = sprite.color;

			spriteColor.a = f;

			sprite.color = spriteColor;

			yield return null;
		}
		for (float f = 0.6f; f > 0.0f; f -= 0.006f) {
			Color spriteColor = sprite.color;

			spriteColor.a = f;

			sprite.color = spriteColor;

			yield return null;
		}
		blackFade.SetActive (false);
	}

	IEnumerator spawnFade(Enemy enemy) {
		SpriteRenderer enemySprite = enemy.gameObject.GetComponent<SpriteRenderer> ();
		enemySprite.color = new Color(0.0f, 0.0f, 0.0f);

		for (float f = 0.0f; f < 1.0f; f += 0.015f) {
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
		SpriteRenderer sprite = blackFade.GetComponent<SpriteRenderer> ();
		Color startColor = sprite.color;
		startColor.a = 0.0f;
		sprite.color = startColor;
		blackFade.SetActive (true);
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
