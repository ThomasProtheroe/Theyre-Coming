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
	private GameObject inventoryPanel;
	[SerializeField]
	private GameObject firstTutorialPanel;
	[SerializeField]
	private GameObject craftingTwinkle;
	[SerializeField]
	private Sprite[] dialogSprites;
	[SerializeField]
	private AudioClip[] cinematicSounds;
	public KeySpawn[] keySpawns;

	[SerializeField]
	private MysteryButton mysteryButton;

	[Header("General Settings")]
	public int prepTime;

	[Header("Audio Control")]
	public MusicController musicPlayer;
	public SoundController soundCon;

	public AudioClip[] prowlingSounds;
	public AudioClip[] walkSounds;
	private AudioClip[][] zombieAttackSoundMaster;
	public AudioClip[] zombieAttackSounds1;
	public AudioClip[] zombieAttackSounds2;
	public AudioClip[] zombieAttackSounds3;
	public AudioClip[] zombieAttackSounds4;
	public AudioClip[] zombieAttackSounds5;

	public AudioClip[] runnerProwlingSounds;
	public AudioClip[] runnerWalkSounds;
	private AudioClip[][] runnerAttackSoundMaster;
	public AudioClip[] runnerAttackSounds1;
	public AudioClip[] runnerAttackSounds2;

	//Audio multipliers
	public float masterVolume;
	public float musicVolume;
	public float effectsVolume;

	[Header("Enemies")]
	public Enemy zombie;
	public Enemy runner;
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
	private List<String> craftedTier1;

	private int tutorialPlay;
	private int tutorialStep;
	private bool isGameOver;
	private bool resultsReady;
	[HideInInspector]
	public bool isPaused;
	private bool timerRunning;
	private bool bossKilled;
	private bool noMoreEnemySpawns;
	private float timer;
	private string phase;
	private string currentCinematic;
	private string gameMode;
	[SerializeField]
	private int maxCorpseCount;
	[SerializeField]
	private int corpseCount;
	private Queue<float> doorShakeMap;

	void Awake() {
		tutorialPlay = PlayerPrefs.GetInt ("tutorialPlay");
		if (tutorialPlay == 1) {
			tutorialStep = 1;
			PlayerPrefs.SetInt("tutorialPlay", 0);
		}
	}

	// Use this for initialization
	void Start () {
		gameMode = Scenes.getParam ("gameMode");
		if (gameMode != "story" && gameMode != "endless") {
			gameMode = "dev";
		}

		player = GameObject.FindGameObjectWithTag ("Player");
		playerCon = player.GetComponent<PlayerController> ();
                            
		SpawnMap.setMode(gameMode);
		if (gameMode == "story") {
			SpawnMap.startStoryMode ();
			CinematicMap.rebuildMap ();
		} else if (gameMode == "endless") {
			SpawnMap.startEndlessMode ();
		}

		enemies = new List<Enemy> ();
		nextCinematic = CinematicMap.getNextCinematic ();

		zombieAttackSoundMaster = new AudioClip[5][];
		zombieAttackSoundMaster [0] = zombieAttackSounds1;
		zombieAttackSoundMaster [1] = zombieAttackSounds2;
		zombieAttackSoundMaster [2] = zombieAttackSounds3;
		zombieAttackSoundMaster [3] = zombieAttackSounds4;
		zombieAttackSoundMaster [4] = zombieAttackSounds5;

		runnerAttackSoundMaster = new AudioClip[2][];
		runnerAttackSoundMaster [0] = runnerAttackSounds1;
		runnerAttackSoundMaster [1] = runnerAttackSounds2;

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

		//Load external data files
		RecipeBook.loadRecipes (System.IO.Path.Combine(Application.streamingAssetsPath, "RecipeMaster.csv"));
		DialogList.loadDialogs (System.IO.Path.Combine(Application.streamingAssetsPath, "DialogMaster.csv"));

		spawnGarageKey ();

		timer = 0.0f;
		timerRunning = true;
		Time.timeScale = 1.0f;

		killTotals = new int[6];
		craftedTier1 = new List<string> ();

		//Set up the scene based on the game mode
		if (gameMode != "dev") {
			//Hide the cursor in non-dev modes (leave in dev mode for debugging)
			Cursor.lockState = CursorLockMode.Locked;
		}

		if (gameMode != "dev") {
			startIntro ();
		} else {
			if (tutorialPlay == 1) {
				firstTutorialPanel.GetComponent<TutorialPanel> ().showTutorialPanel();
			}
		}

		if (gameMode == "dev") {
			changePhase("siege");
		}

		StartCoroutine ("CheckVictory");
	}
	
	// Update is called once per frame
	void Update () {
		if (isGameOver && resultsReady) {
			checkGameOverSkip();
		}

		if (timerRunning) {
			timer += Time.deltaTime;
		}

		if (phase == "prep") {
			TimeSpan timeSpan = TimeSpan.FromSeconds ((prepTime - Mathf.Floor (timer)));
			timerText.text = string.Format ("{0:D2}:{1:d2}", timeSpan.Minutes, timeSpan.Seconds);

			//Shake the front door in time with the pounding noises
			if (doorShakeMap.Count > 0 && doorShakeMap.Peek() < timer) {
				doorShakeMap.Dequeue ();
				GameObject.FindGameObjectWithTag("FrontDoor").GetComponent<FrontDoor> ().shakeDoor();
			}

			if (Mathf.Floor (timer) >= prepTime) {
				timerText.enabled = false;
				changePhase ("siege");
			}
		} else if (phase == "siege") {
			if ((gameMode == "story" || gameMode == "endless") && !noMoreEnemySpawns) {
				checkForEnemySpawns ();
			}
		} else if (phase == "downtime") {

		}

		if (gameMode == "story") {
			checkForCinematics ();
		}
		

		//Skip cinematics when button is pressed
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

		//Dev mode specific functionality
		if (gameMode == "dev") {
			if (Input.GetKeyDown ("t")) {
				spawnEnemyRand (Constants.ENEMY_TYPE_NORMAL);
			} else if (Input.GetKeyDown ("p")) {
				spawnEnemyRand (Constants.ENEMY_TYPE_RUNNER);
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
			} else if (Input.GetKeyDown ("i")) {
				MysteryButton newMysteryButton = Instantiate (mysteryButton, player.transform.position, Quaternion.identity);
				newMysteryButton.drop ();
			}
		}
	}

	public void pauseGame() {
		pauseMenu.SetActive (true);
		Time.timeScale = 0.0f;
		isPaused = true;
		Cursor.lockState = CursorLockMode.None;

		soundCon.pauseAll ();
	}

	public void unpauseGame() {
		pauseMenu.SetActive (false);
		Time.timeScale = 1.0f;
		isPaused = false;
		Cursor.lockState = CursorLockMode.Locked;

		soundCon.playAll ();
	}

	void changePhase(string newPhase) {
		Debug.Log("Change phase: " + newPhase);
		phase = newPhase;
		musicPlayer.changeMusic (phase);
		timer = 0.0f;

		if (newPhase == "prep") {
			timerText.enabled = true;
		} else if (newPhase == "downtime") {

		} else if (newPhase == "siege") {
			SpawnMap.buildWave(timer);
			noMoreEnemySpawns = false;
			nextSpawn = SpawnMap.getNextSpawn ();
			//Change front door to broken version
			GameObject.FindGameObjectWithTag("FrontDoor").GetComponent<FrontDoor> ().setSiegeMode();
			/*
			if (gameMode == "story") {
				spawnEnemy (spawnZones[2], Constants.ENEMY_TYPE_NORMAL);
			}
			*/

			StartCoroutine ("CheckNightEnd");
		}
	}

	public void startNewNight() {
		changePhase("siege");
	}

	private void checkForEnemySpawns() {
		Debug.Log(timer);
		if (timer >= nextSpawn.spawnTime) {
			if (nextSpawn.enemyType != Constants.ENEMY_TYPE_BOSS) {
				for (int i = 0; i < nextSpawn.spawnCount; i++) {
					spawnEnemyRand (nextSpawn.enemyType);
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
				soundCon.playEnvironmentalSound (cinematicSounds[nextCinematic.clipIndex], false);
			}

			nextCinematic = CinematicMap.getNextCinematic ();
		}
	}

	private void checkGameOverSkip() {
		if (Input.anyKeyDown) {
			SceneManager.LoadScene("ResultsScreen");
		}
	}

	void spawnEnemyRand(int enemyType=Constants.ENEMY_TYPE_NORMAL) {
		//Wrapper for co-routine
		StartCoroutine ("SpawnEnemyRand", enemyType);
	}

	void spawnEnemy(EnemySpawn spawnZone, int enemyType) {
		float spawnLocX = spawnZone.transform.position.x;
		spawnLocX += UnityEngine.Random.Range (spawnZone.maxOffset * -1, spawnZone.maxOffset + 1);

		//Create and position the enemy
		Enemy newEnemy;
		if (enemyType == Constants.ENEMY_TYPE_RUNNER) {
			newEnemy = Instantiate (runner, new Vector3(spawnLocX, runner.transform.position.y, 0), Quaternion.identity);

			//Select a random walk, prowl and attack sound and assign them to the new enemy
			int vocalType = UnityEngine.Random.Range(0,4);
			newEnemy.setWalkSound(runnerWalkSounds[UnityEngine.Random.Range(0,4)]);
			newEnemy.setProwlSound(runnerProwlingSounds[vocalType]);
			//We have shared attack sounds between certain vocal types
			if (vocalType == 0 || vocalType == 1) {
				newEnemy.addAttackSound (runnerAttackSoundMaster [0]);
			} else {
				newEnemy.addAttackSound (runnerAttackSoundMaster [1]);
			}
		} else {
			newEnemy = Instantiate (zombie, new Vector3(spawnLocX, zombie.transform.position.y, 0), Quaternion.identity);

			//Select a random walk, prowl and attack sound and assign them to the new enemy
			int vocalType = UnityEngine.Random.Range(0,5);
			newEnemy.setProwlSound(prowlingSounds[vocalType]);
			newEnemy.addAttackSound (zombieAttackSoundMaster[vocalType]);
			newEnemy.setWalkSound(walkSounds[UnityEngine.Random.Range(0,5)]);
		}

		if (gameMode == "endless") {
			newEnemy.enableItemDrops(10);
		}

		enemies.Add (newEnemy);
		//Invulnerable while they are spawning in
		newEnemy.isInvulnerable = true;

		Area spawnArea = spawnZone.GetComponentInParent<Area>();
		newEnemy.setCurrentArea (spawnArea);

		//Fade the enemy sprite in from black
		StartCoroutine("spawnFade", newEnemy);
	}

	void spawnBoss(EnemySpawn spawnZone) {
		float spawnLocX = spawnZone.transform.position.x;
		spawnLocX += UnityEngine.Random.Range (spawnZone.maxOffset * -1, spawnZone.maxOffset + 1);

		//Create and position the boss
		BossGramps newBoss = Instantiate (boss, new Vector3(spawnLocX, zombie.transform.position.y, 0), Quaternion.identity);
		enemies.Add (newBoss);
		//Invulnerable while they are spawning in
		newBoss.isInvulnerable = true;

		Area spawnArea = spawnZone.GetComponentInParent<Area>();
		newBoss.setCurrentArea (spawnArea);

		//Set unique boss sounds
		int vocalType = UnityEngine.Random.Range(0,5);
		newBoss.setProwlSound(prowlingSounds[vocalType]);
		newBoss.addAttackSound (zombieAttackSoundMaster[vocalType]);
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

	public void showDescription(string itemName, string description, List<string> statusEffects, int tier=-1) {
		if (dialogPanel.gameObject.activeSelf) {
			return;
		}
		string[] text = new string[2];
		text [0] = itemName;
		text [1] = description;
		descriptionPanel.showDescription (text);

		if (tier == -1) {
			descriptionPanel.hideTierImage();
		} else {
			descriptionPanel.showTierImage (tier);
		}

		descriptionPanel.updateStatusIcons (statusEffects);
	}

	public void hideDescription() {
		descriptionPanel.hideDescription ();
	}

	private void createTwinkle(Item item) {
		Vector3 position = RandomPointInBounds (item.hitCollider.bounds);
		GameObject twinkle = Instantiate (craftingTwinkle, position, Quaternion.identity);
		twinkle.GetComponent<DestroyAfterTime> ().StartCoroutine ("destroyAfterTime", 1.0f);
	}

	public static Vector3 RandomPointInBounds(Bounds bounds) {
		return new Vector3(
			UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
			UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
			UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
		);
	}

	private void startIntro() {
		skipText.gameObject.SetActive (true);

		playerCon.enableCinematicControl (true);

		StartCoroutine ("introFade");
		StartCoroutine ("playIntro");
	}

	private void stopIntro() {
		blackFade.SetActive (false);
		changePhase("downtime");

		skipText.gameObject.SetActive (false);

		//Start the tutorial if using
		if (tutorialPlay == 1) {
			firstTutorialPanel.GetComponent<TutorialPanel> ().showTutorialPanel();
		}
			
		playerCon.enableCinematicControl (false);
		currentCinematic = null;
	}

	public void startElevatorCinematic() {
		playerCon.enableCinematicControl (true);

		StartCoroutine ("playElevatorDialog");
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

	public bool isTutorialActive() {
		if (tutorialPlay == 0) {
			return false;
		} else {
			return true;
		}
	}

	public int getTutorialStep() {
		return tutorialStep;
	}

	public string getPhase() {
		return phase;
	}

	public void setTutorialStep(int step) {
		tutorialStep = step;
	}

	public void gameOver() {
		isGameOver = true;
		pauseTimer ();
		endGameCalculations ();
		Scenes.setParam ("result", "death");
		deactivateAllEnemies ();
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

	public void startCraftingFanfare(Item item) {
		if (item.tier == 1 && item.craftingFanfare != null && !craftedTier1.Contains(item.itemName)) {
			craftedTier1.Add (item.itemName);
			StartCoroutine ("CraftingFanfare", item);
		}
	}

	public void deactivateAllEnemies() {
		//Set any existing enemies to idle
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		foreach (GameObject enemy in enemies) {
			enemy.GetComponent<Enemy> ().deactivate ();
		}
	}

	public void activateAllEnemies() {
		//Set any existing enemies to idle
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		foreach (GameObject enemy in enemies) {
			enemy.GetComponent<Enemy> ().activate ();
		}
	}

	public void returnToMenu() {
		Scenes.clearParams ();
		SceneManager.LoadScene ("MainMenu");
	}

	public void miscFadeOut(float interval=0.01f) {
		StartCoroutine ("MiscBlackFadeOut", interval);
	}

	public void miscFadeIn(float interval=0.01f) {
		StartCoroutine ("MiscBlackFadeIn", interval);
	}

	IEnumerator playConversation(Dialog[] conversation) {
		foreach(Dialog dialog in conversation) {
			yield return new WaitForSeconds (dialog.delay);
			showDialog(dialog);
		}
	}

	IEnumerator playIntro() {
		currentCinematic = "playIntro";

		soundCon.playEnvironmentalSound(cinematicSounds[0], false);
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

	IEnumerator playElevatorDialog() {
		currentCinematic = "playElevatorDialog";

		yield return new WaitForSeconds (3.0f);

		showDialog (new Dialog("\nWhat is this place?", null, 5.0f));

		yield return new WaitForSeconds (6.0f);

		playerCon.flipPlayer ();

		yield return new WaitForSeconds (2.5f);

		playerCon.flipPlayer ();

		yield return new WaitForSeconds (2.5f);

		showDialog (new Dialog("\nAt least it seems safe in here. There's no way those things are getting inside.", null, 5.0f));

		yield return new WaitForSeconds (9.0f);
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
		for (float f = 0.9f; f > 0.6f; f -= 0.0035f) {
			Color spriteColor = sprite.color;

			spriteColor.a = f;

			sprite.color = spriteColor;

			yield return null;
		}
		for (float f = 0.6f; f > 0.0f; f -= 0.007f) {
			Color spriteColor = sprite.color;

			spriteColor.a = f;

			sprite.color = spriteColor;

			yield return null;
		}
		blackFade.SetActive (false);
	}

	IEnumerator CraftingFanfare(Item item) {
		//Pause game
		Time.timeScale = 0.0f;
		isPaused = true;
		soundCon.pauseAll ();

		//Darken screen except for Item and description boxes
		Vector3 target = GameObject.FindGameObjectWithTag("CanvasCenter").transform.position;
		Vector3 restingPosition = descriptionPanel.transform.position;
		SpriteRenderer sprite = blackFade.GetComponent<SpriteRenderer> ();
		Color startColor = sprite.color;
		startColor.a = 0.0f;
		sprite.color = startColor;
		blackFade.SetActive (true);
		for (float f = 0.0f; f < 0.5f; f += 0.03f) {
			Color spriteColor = sprite.color;
			spriteColor.a = f;
			sprite.color = spriteColor;

			//Move the UI boxes towards the center of the screen
			//Description Box
			descriptionPanel.transform.position = Vector3.MoveTowards(descriptionPanel.transform.position, new Vector3(descriptionPanel.transform.position.x, target.y), 3.0f);
			inventoryPanel.transform.position = Vector3.MoveTowards(inventoryPanel.transform.position, new Vector3(inventoryPanel.transform.position.x, target.y), 3.0f);

			yield return null;
		}

		//Highlight/shine item sprite and play fanfare
		soundCon.playPriorityOneShot(item.craftingFanfare);


		float twinkleInterval = 0.12f;
		float nextTwinkle = 0.0f;
		float itemShineDuration = 0.0f;
		SpriteRenderer itemSprite = item.GetComponent<SpriteRenderer> ();
		//Turn yellow over time
		for (float f = 1f; f >= 0.2f; f -= 0.03f) {
			Color c = itemSprite.material.color;
			c.r = 0.95f;
			c.g = (1.0f - ((1.0f - f) / 3));
			c.b = f;
			itemSprite.material.color = c;

			itemShineDuration += Time.unscaledDeltaTime;
			if (nextTwinkle <= 0.0f) {
				createTwinkle (item);
				nextTwinkle = twinkleInterval;
			} else {
				nextTwinkle -= Time.unscaledDeltaTime;
			}

			yield return null;
		}

		//Turn back to original color
		nextTwinkle = 0.0f;
		for (float f = 0.2f; f <= 1; f += 0.03f) {
			Color c = itemSprite.material.color;
			c.r = 0.95f;
			c.g = (1.0f - ((1.0f - f) / 3));
			c.b = f;
			itemSprite.material.color = c;

			itemShineDuration += Time.unscaledDeltaTime;
			if (nextTwinkle <= 0.0f) {
				createTwinkle (item);
				nextTwinkle = twinkleInterval;
			} else {
				nextTwinkle -= Time.unscaledDeltaTime;
			}
			yield return null;
		}
		Color col = itemSprite.material.color;
		col.r = 1.0f;
		col.g = 1.0f;
		col.b = 1.0f;
		itemSprite.material.color = col;

		float additionalWaitTime = (item.craftingFanfare.length * 0.7f) - itemShineDuration;
		if (additionalWaitTime <= 0.0f) {
			additionalWaitTime = 0.0f;
		}
		yield return new WaitForSecondsRealtime (additionalWaitTime);

		//Unpause game and brighten screen when fanfare is complete
		for (float f = 0.5f; f > 0.0f; f -= 0.03f) {
			Color spriteColor = sprite.color;
			spriteColor.a = f;
			sprite.color = spriteColor;

			//Return UI to original position
			descriptionPanel.transform.position = Vector3.MoveTowards(descriptionPanel.transform.position, new Vector3(descriptionPanel.transform.position.x, restingPosition.y), 3.0f);
			inventoryPanel.transform.position = Vector3.MoveTowards(inventoryPanel.transform.position, new Vector3(inventoryPanel.transform.position.x, restingPosition.y), 3.0f);

			yield return null;
		}
		blackFade.SetActive (false);

		Time.timeScale = 1.0f;
		isPaused = false;
		soundCon.playAll ();
	}

	IEnumerator SpawnEnemyRand(int enemyType) {
		yield return new WaitForSeconds (0.3f);

		//Randomly select a spawn location based on spawn weighting
		List<EnemySpawn> randomizer = new List<EnemySpawn>();
		foreach (EnemySpawn spawner in spawnZones) {
			for(int i=0; i < spawner.weight; i++) {
				randomizer.Add (spawner);
			}
		}
		EnemySpawn spawnZone = randomizer[UnityEngine.Random.Range(0, randomizer.Count)];

		spawnEnemy (spawnZone, enemyType);
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

		enemy.isInvulnerable = false;
		enemy.activate ();
	}

	IEnumerator DeathFade() {
		//musicPlayer.fadeMusicOut(0.03f);
		musicPlayer.changeMusic("death");

		//Start the fade to black
		SpriteRenderer sprite = blackFade.GetComponent<SpriteRenderer> ();
		Color startColor = sprite.color;
		startColor.a = 0.0f;
		sprite.color = startColor;
		blackFade.SetActive (true);
		for (float f = 0.0f; f < 1.0f; f += 0.002f) {
			Color spriteColor = sprite.color;
			Color textColor = gameOverText.color;

			spriteColor.a = f;
			textColor.a = f;

			sprite.color = spriteColor;
			gameOverText.color = textColor;

			yield return null;
		}

		resultsReady = true;
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

	IEnumerator CheckNightEnd() {
		if (noMoreEnemySpawns && !playerCon.isDead && enemies.Count == 0) {
			yield return new WaitForSeconds (4.0f);
			changePhase ("downtime");
			yield break;
		}

		yield return new WaitForSeconds (3.0f);

		StartCoroutine ("CheckNightEnd");
	}

	IEnumerator CheckVictory() {
		if (noMoreEnemySpawns && bossKilled && !playerCon.isDead && enemies.Count == 0) {
			victoryFade ();
			yield break;
		}

		yield return new WaitForSeconds (3.0f);

		StartCoroutine ("CheckVictory");
	}

	IEnumerator MiscBlackFadeIn(float interval=0.01f) {
		SpriteRenderer sprite = blackFade.GetComponent<SpriteRenderer> ();
		Color startColor = sprite.color;
		startColor.a = 1.0f;
		blackFade.SetActive (true);
		for (float f = 1.0f; f > 0.0f; f -= interval) {
			Color spriteColor = sprite.color;
			spriteColor.a = f;
			sprite.color = spriteColor;

			yield return null;
		}
		blackFade.SetActive (false);
	}

	IEnumerator MiscBlackFadeOut(float interval=0.01f) {
		SpriteRenderer sprite = blackFade.GetComponent<SpriteRenderer> ();
		Color startColor = sprite.color;
		startColor.a = 0.0f;
		sprite.color = startColor;
		blackFade.SetActive (true);
		for (float f = 0.0f; f < 1.0f; f += interval) {
			Color spriteColor = sprite.color;
			spriteColor.a = f;
			sprite.color = spriteColor;

			yield return null;
		}
	}
}
