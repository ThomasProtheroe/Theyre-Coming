using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public GameObject pauseMenu;
	public GameObject blackFade;
	[SerializeField]
	private GameObject mainCamera;
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
	public KeySpawn[] keySpawns;

	[Header("General Settings")]
	public int prepTime;

	[Header("Audio Control")]
	public AudioSource source;
	public MusicController musicPlayer;
	public SoundController soundCon;
	public AudioClip[] prowlingSounds;
	public AudioClip[] walkSounds;
	private AudioClip[][] attackSoundMaster;
	public AudioClip[] attackSounds1;
	public AudioClip[] attackSounds2;
	public AudioClip[] attackSounds3;
	public AudioClip[] attackSounds4;
	public AudioClip[] attackSounds5;

	//Audio multipliers
	public float masterVolume;
	public float musicVolume;
	public float effectsVolume;

	[Header("Enemies")]
	public Enemy enemy;
	public BossGramps boss;
	public List<EnemySpawn> spawnZones = new List<EnemySpawn> ();
	[HideInInspector]
	public Dictionary<string, List<EnemyCorpse>> corpseDict = new Dictionary<string, List<EnemyCorpse>> ();
	private List<Enemy> enemies;

	private Dictionary<string, Dictionary<string, string>> pathfindingMap;

	private GameObject player;
	private PlayerController playerCon;
	private SpawnInstance nextSpawn;
	private Cinematic nextCinematic;

	//Stat tracking
	[HideInInspector]
	public int[] killTotals;
	private int itemsCrafted;

	private bool isGameOver;
	[HideInInspector]
	public bool isPaused;
	[HideInInspector]
	private bool timerRunning;
	private bool bossKilled;
	private bool noMoreEnemySpawns;
	private float timer;
	private string phase;
	private string currentCinematic;
	private string devMode;
	[SerializeField]
	private int maxCorpseCount;
	[SerializeField]
	private int corpseCount;
	private Queue<float> doorShakeMap;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		playerCon = player.GetComponent<PlayerController> ();
		source = GetComponent<AudioSource> ();

		SpawnMap.rebuildMap ();
		CinematicMap.rebuildMap ();

		enemies = new List<Enemy> ();

		nextSpawn = SpawnMap.getNextSpawn ();
		nextCinematic = CinematicMap.getNextCinematic ();

		attackSoundMaster = new AudioClip[5][];
		attackSoundMaster [0] = attackSounds1;
		attackSoundMaster [1] = attackSounds2;
		attackSoundMaster [2] = attackSounds3;
		attackSoundMaster [3] = attackSounds4;
		attackSoundMaster [4] = attackSounds5;

		doorShakeMap = new Queue<float>();
		doorShakeMap.Enqueue(71.33f);
		doorShakeMap.Enqueue(72.92f);
		doorShakeMap.Enqueue(74.25f);
		doorShakeMap.Enqueue(75.69f);
		doorShakeMap.Enqueue(77.35f);
		doorShakeMap.Enqueue(80.15f);
		doorShakeMap.Enqueue(82.38f);
		doorShakeMap.Enqueue(83.80f);
		doorShakeMap.Enqueue(86.15f);

		buildPathingMap ();

		//Load settings
		updateVolume();

		devMode = Scenes.getParam ("devMode");
		if (devMode == null) {
			devMode = "false";
		}
			
		RecipeBook.loadRecipes (System.IO.Path.Combine(Application.streamingAssetsPath, "RecipeMaster.csv"));

		spawnGarageKey ();

		timer = 0.0f;
		timerRunning = true;
		Time.timeScale = 1.0f;

		killTotals = new int[6];

		if (devMode == "false") {
			startIntro ();
		} else {
			changePhase("siege");
			onSiegePhase ();
		}

		StartCoroutine ("CheckVictory");
	}
	
	// Update is called once per frame
	void Update () {
		if (isGameOver) {
			return;
		}

		if (timerRunning) {
			timer += Time.deltaTime;
		}

		if (phase == "prep") {
			TimeSpan timeSpan = TimeSpan.FromSeconds ((prepTime - Mathf.Floor (timer)));
			timerText.text = string.Format ("{0:D2}:{1:d2}", timeSpan.Minutes, timeSpan.Seconds);

			if (doorShakeMap.Count > 0 && doorShakeMap.Peek() < timer) {
				doorShakeMap.Dequeue ();
				GameObject.FindGameObjectWithTag("FrontDoor").GetComponent<FrontDoor> ().shakeDoor();
			}

			if (Mathf.Floor (timer) >= prepTime) {
				timerText.enabled = false;
				changePhase ("siege");
				onSiegePhase ();
			}
		} else if (phase == "siege") {
			if (Scenes.getParam ("devMode") == "false" && !noMoreEnemySpawns) {
				checkForEnemySpawns ();
			}
		}

		checkForCinematics ();

		//Skip cinematics
		if (currentCinematic != null && Input.GetButtonDown ("interact")) {
			switch (currentCinematic) {
			case "playIntro":
				StopCoroutine ("playIntro");
				StopCoroutine ("introFade");
				hideDialog ();
				stopIntro ();
				break;
			}
		}

		//Pause Menu
		if (Input.GetKeyDown ("escape")) {
			if (isPaused) {
				unpauseGame ();
			} else {
				pauseGame ();
			}
		}

		if (Scenes.getParam ("devMode") != "false") {
			if (Input.GetKeyDown ("t")) {
				spawnEnemyRand ();
			} else if (Input.GetKeyDown ("u")) {
				//Unlock all doors in dev mode
				GameObject[] transitions = GameObject.FindGameObjectsWithTag("Transition");
				foreach (GameObject temp in transitions) {
					Transition transition = temp.GetComponent<Transition> ();
					if (transition.isLocked) {
						transition.isLocked = false;
					}
				}
			} else if (Input.GetKeyDown ("y")) {
				spawnBoss (spawnZones[0]);
			}
		}
	}

	public void pauseGame() {
		pauseMenu.SetActive (true);
		Time.timeScale = 0.0f;
		isPaused = true;

		soundCon.pauseAll ();
	}

	public void unpauseGame() {
		pauseMenu.SetActive (false);
		Time.timeScale = 1.0f;
		isPaused = false;

		soundCon.playAll ();
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
		spawnEnemy (spawnZones[2]);
	}

	private void checkForEnemySpawns() {
		if (timer >= nextSpawn.spawnTime) {
			if (!nextSpawn.isBoss) {
				for (int i = 0; i < nextSpawn.spawnCount; i++) {
					Invoke ("spawnEnemyRand", i * 0.3f);
				}
				nextSpawn = SpawnMap.getNextSpawn ();
			} else {
				spawnBoss (spawnZones[0]);
				nextSpawn = SpawnMap.getNextSpawn ();
			}
			if (nextSpawn == null) {
				noMoreEnemySpawns = true;
			}
		}
	}

	private void checkForCinematics() {
		if (nextCinematic != null && phase == nextCinematic.phase && timer >= nextCinematic.playTime) {
			if (nextCinematic.dialog != null) {
				if (nextCinematic.dialog.Length == 1) {
					showDialog (nextCinematic.dialog[0]);
				} else if (nextCinematic.dialog.Length > 1) {
					StartCoroutine ("playConversation", nextCinematic.dialog);
				}
			}
			if (nextCinematic.clipIndex != -1) {
				source.clip = cinematicSounds[nextCinematic.clipIndex];
				source.Play ();
			}

			nextCinematic = CinematicMap.getNextCinematic ();
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
		EnemySpawn spawnZone = randomizer[UnityEngine.Random.Range(0, randomizer.Count)];

		spawnEnemy (spawnZone);
	}

	void spawnEnemy(EnemySpawn spawnZone) {
		float spawnLocX = spawnZone.transform.position.x;
		spawnLocX += UnityEngine.Random.Range (spawnZone.maxOffset * -1, spawnZone.maxOffset + 1);

		//Create and position the enemy
		Enemy newEnemy = Instantiate (enemy, new Vector3(spawnLocX, enemy.transform.position.y, 0), Quaternion.identity);
		enemies.Add (newEnemy);
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

	void spawnBoss(EnemySpawn spawnZone) {
		float spawnLocX = spawnZone.transform.position.x;
		spawnLocX += UnityEngine.Random.Range (spawnZone.maxOffset * -1, spawnZone.maxOffset + 1);

		//Create and position the boss
		BossGramps newBoss = Instantiate (boss, new Vector3(spawnLocX, enemy.transform.position.y, 0), Quaternion.identity);
		enemies.Add (newBoss);
		//Invulnerable while they are spawning in
		newBoss.isInvunlerable = true;

		Area spawnArea = spawnZone.GetComponentInParent<Area>();
		newBoss.setCurrentArea (spawnArea);

		//Set unique boss sounds
		int vocalType = UnityEngine.Random.Range(0,5);
		newBoss.setProwlSound(prowlingSounds[vocalType]);
		newBoss.addAttackSound (attackSoundMaster[vocalType]);
		newBoss.setWalkSound(walkSounds[UnityEngine.Random.Range(0,5)]);

		//Fade the enemy sprite in from black
		StartCoroutine("spawnFade", newBoss);

		//TODO Play boss arrival dialog

	}

	public void removeEnemy(Enemy enemy) {
		if (enemy is BossGramps) {
			bossKilled = true;
		}
		enemies.Remove (enemy);
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

	private void spawnGarageKey() {
		//Pick a random spawn location for the garage key
		keySpawns [UnityEngine.Random.Range (0, keySpawns.Length)].spawnKey();
	}

	private void buildPathingMap() {
		pathfindingMap = new Dictionary<string, Dictionary<string, string>>();
		Dictionary<string, string> livingroomMap = new Dictionary<string, string> ();
		livingroomMap.Add ("Bathroom", "TrLivingroomBathroom");
		livingroomMap.Add ("Hallway", "TrLivingroomHallway");
		livingroomMap.Add ("Kitchen", "TrLivingroomKitchen");
		livingroomMap.Add ("Bedroom", "TrLivingroomHallway");
		livingroomMap.Add ("Yard", "TrLivingroomKitchen");
		livingroomMap.Add ("Garage", "TrLivingroomKitchen");
		pathfindingMap.Add ("Livingroom", livingroomMap);

		Dictionary<string, string> bathroomMap = new Dictionary<string, string> ();
		bathroomMap.Add ("Livingroom", "TrBathroomLivingroom");
		bathroomMap.Add ("Hallway", "TrBathroomLivingroom");
		bathroomMap.Add ("Kitchen", "TrBathroomLivingroom");
		bathroomMap.Add ("Bedroom", "TrBathroomLivingroom");
		bathroomMap.Add ("Yard", "TrBathroomLivingroom");
		bathroomMap.Add ("Garage", "TrBathroomLivingroom");
		pathfindingMap.Add ("Bathroom", bathroomMap);

		Dictionary<string, string> hallwayMap = new Dictionary<string, string> ();
		hallwayMap.Add ("Livingroom", "TrHallwayLivingroom");
		hallwayMap.Add ("Bathroom", "TrHallwayLivingroom");
		hallwayMap.Add ("Kitchen", "TrHallwayKitchen");
		hallwayMap.Add ("Bedroom", "TrHallwayBedroom");
		hallwayMap.Add ("Yard", "TrHallwayKitchen");
		hallwayMap.Add ("Garage", "TrHallwayKitchen");
		pathfindingMap.Add ("Hallway", hallwayMap);

		Dictionary<string, string> bedroomMap = new Dictionary<string, string> ();
		bedroomMap.Add ("Livingroom", "TrBedroomHallway");
		bedroomMap.Add ("Bathroom", "TrBedroomHallway");
		bedroomMap.Add ("Kitchen", "TrBedroomHallway");
		bedroomMap.Add ("Hallway", "TrBedroomHallway");
		bedroomMap.Add ("Yard", "TrBedroomHallway");
		bedroomMap.Add ("Garage", "TrBedroomHallway");
		pathfindingMap.Add ("Bedroom", bedroomMap);

		Dictionary<string, string> kitchenMap = new Dictionary<string, string> ();
		kitchenMap.Add ("Livingroom", "TrKitchenLivingroom");
		kitchenMap.Add ("Bathroom", "TrKitchenLivingroom");
		kitchenMap.Add ("Bedroom", "TrKitchenHallway");
		kitchenMap.Add ("Hallway", "TrKitchenHallway");
		kitchenMap.Add ("Yard", "TrKitchenYard");
		kitchenMap.Add ("Garage","TrKitchenYard");
		pathfindingMap.Add ("Kitchen", kitchenMap);

		Dictionary<string, string> yardMap = new Dictionary<string, string> ();
		yardMap.Add ("Livingroom", "TrYardKitchen");
		yardMap.Add ("Bathroom", "TrYardKitchen");
		yardMap.Add ("Bedroom","TrYardKitchen");
		yardMap.Add ("Hallway", "TrYardKitchen");
		yardMap.Add ("Kitchen", "TrYardKitchen");
		yardMap.Add ("Garage","TrYardGarage");
		pathfindingMap.Add ("Yard", yardMap);

		Dictionary<string, string> garageMap = new Dictionary<string, string> ();
		garageMap.Add ("Livingroom", "TrGarageYard");
		garageMap.Add ("Bathroom", "TrGarageYard");
		garageMap.Add ("Bedroom","TrGarageYard");
		garageMap.Add ("Hallway", "TrGarageYard");
		garageMap.Add ("Yard", "TrGarageYard");
		garageMap.Add ("Kitchen","TrGarageYard");
		pathfindingMap.Add ("Garage", garageMap);
	}

	public Transition findRouteToPlayer(Area currentArea) {
		return searchForPlayer(currentArea);
	}

	private Transition searchForPlayer(Area startingArea) {
		//Before we go mapping out a route, check if player is in an adjacent area (saves a lot of time if they are)
		foreach (Transition transition in startingArea.transitions) {
			if (playerCon.currentArea == transition.sibling.transform.parent.GetComponent<Area> ()) {
				//Hooray!
				return transition;
			}
		}

		//Dammit. Ok, let's go find the best route to get to them using our hackish spawn map
		string transitionName = pathfindingMap[startingArea.name][playerCon.currentArea.name];
		Transition target = null;
		foreach(Transition transition in startingArea.transitions) {
			if (transition.name == transitionName) {
				target = transition;
			}
		}

		return target;
	}

	public void shakeCamera(float duration, float intensity) {
		mainCamera.GetComponent<CameraShake> ().startShaking(duration, intensity);
	}

	public void showDialog(Dialog dialog) {
		if (descriptionPanel.gameObject.activeSelf) {
			hideDescription ();
		}
		//Use Fiona's sprite if none is provided
		if (dialog.character == "boss") {
			dialog.sprites = new Sprite[] { dialogSprites [2], dialogSprites [3] };
		} else {
			dialog.sprites = new Sprite[] { dialogSprites [0], dialogSprites [1] };
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

		skipText.gameObject.SetActive (false);
		playerCon.itemSlot1.gameObject.SetActive(true);
		playerCon.itemSlot2.gameObject.SetActive(true);
		playerCon.enableCinematicControl (false);
		currentCinematic = null;
	}

	public void startTimer() {
		timerRunning = true;
	}

	public void pauseTimer() {
		timerRunning = false;
	}

	public void stopTimer() {
		timer = 0.0f;
		timerRunning = false;
	}


	/*** Stat tracking ***/
	public void countEnemyKill(int attackType) {
		killTotals [attackType]++;
	}

	public void countItemCraft() {
		itemsCrafted++;
	}


	public void updateVolume() {
		float masterVolume = PlayerPrefs.GetFloat ("masterVolume");
		float musicVolume = PlayerPrefs.GetFloat ("musicVolume");
		float effectsVolume = PlayerPrefs.GetFloat ("effectsVolume");

		musicPlayer.updateVolume(masterVolume * musicVolume);

		soundCon.updateVolume(masterVolume * effectsVolume);
		playerCon.updateVolume (masterVolume * effectsVolume);
		source.volume = masterVolume * effectsVolume;
	}

	private void endGameCalculations() {
		isGameOver = true;
		pauseTimer ();

		int killTotal = 0;
		int favoredType = 0;
		int mostTypeKills = 0;
		for(int i = 0; i < killTotals.Length; i++) {
			killTotal += killTotals[i];
			if (killTotals[i] > mostTypeKills) {
				favoredType = i;
				mostTypeKills = killTotals [i];
			}
		}

		//Convert favored weapon type into a useful string
		string typeName = "";
		switch(favoredType) {
		case Constants.ATTACK_TYPE_BLUNT:
			typeName = "blunt weapons.";
			break;
		case Constants.ATTACK_TYPE_PIERCE:
			typeName = "sharp weapons.";
			break;
		case Constants.ATTACK_TYPE_PROJECTILE:
			typeName = "ranged weapons.";
			break;
		case Constants.ATTACK_TYPE_TRAP:
			typeName = "devious traps.";
			break;
		case Constants.ATTACK_TYPE_FIRE:
			typeName = "BURNING THEM ALL!";
			break;
		default:
			typeName = "pure ingenuity.";
			break;
		}

		Scenes.setParam ("resultsTime", timer.ToString());
		Scenes.setParam ("resultsKills", killTotal.ToString());
		Scenes.setParam ("resultsType", typeName);
	}

	public void gameOver() {
		isGameOver = true;
		pauseTimer ();
		endGameCalculations ();
		Scenes.setParam ("result", "death");

		deathFade ();
	}

	private void deathFade() {
		//Set any existing enemies to idle
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		foreach (GameObject enemy in enemies) {
			enemy.GetComponent<Enemy> ().deactivate ();
		}

		StartCoroutine ("DeathFade");
	}

	private void victoryFade() {
		//Should we make player invulnerable?
		isGameOver = true;
		pauseTimer ();
		endGameCalculations ();

		StopCoroutine ("CheckVictory");
		StartCoroutine ("VictoryFade");
	}

	public void returnToMenu() {
		Scenes.clearParams ();
		SceneManager.LoadScene ("MainMenu");
	}

	IEnumerator playConversation(Dialog[] conversation) {
		foreach(Dialog dialog in conversation) {
			yield return new WaitForSeconds (dialog.delay);
			showDialog(dialog);
		}
	}

	IEnumerator playIntro() {
		currentCinematic = "playIntro";

		source.PlayOneShot (cinematicSounds[0]);
		playerCon.moveInput = -1.0f;

		yield return new WaitForSeconds (0.7f);

		playerCon.moveInput = 0.0f;

		yield return new WaitForSeconds (1.0f);

		showDialog (new Dialog("Fiona:\nThat was way too close...", null, 4.0f));

		yield return new WaitForSeconds (5.0f);

		playerCon.flipPlayer ();

		yield return new WaitForSeconds (1.0f);

		showDialog (new Dialog("\nWhat the hell were those things?", null, 4.0f));

		yield return new WaitForSeconds (5.0f);

		playerCon.flipPlayer ();

		yield return new WaitForSeconds (1.0f);

		showDialog (new Dialog("\nThey were right behind me, they'll be here any minute. I need to get ready for them.", null, 5.0f));

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

	IEnumerator DeathFade() {
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

		SceneManager.LoadScene("ResultsScreen");
	}

	IEnumerator VictoryFade() {
		StopCoroutine ("CheckVictory");
		SpriteRenderer sprite = blackFade.GetComponent<SpriteRenderer> ();
		Color startColor = sprite.color;
		startColor.a = 0.0f;
		sprite.color = startColor;
		blackFade.SetActive (true);
		gameOverText.text = "You Survived";
		for (float f = 0.0f; f < 1.0f; f += 0.005f) {
			Color spriteColor = sprite.color;
			Color textColor = gameOverText.color;

			spriteColor.a = f;
			textColor.a = f;

			sprite.color = spriteColor;
			gameOverText.color = textColor;

			yield return null;
		}

		yield return new WaitForSeconds (2.0f);

		Scenes.Load ("ResultsScreen", "result", "victory");
	}

	IEnumerator CheckVictory() {
		if (noMoreEnemySpawns && bossKilled && !playerCon.isDead && enemies.Count == 0) {
			victoryFade ();
			yield break;
		}

		yield return new WaitForSeconds (3.0f);

		StartCoroutine ("CheckVictory");
	}
}
