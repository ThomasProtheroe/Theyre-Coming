using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public int prepTime;
	public Enemy enemy;
	public BossGramps boss;
	public GameObject pauseMenu;
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
	public SoundController soundCon;
	public AudioClip[] prowlingSounds;
	public AudioClip[] walkSounds;
	private AudioClip[][] attackSoundMaster;
	public AudioClip[] attackSounds1;
	public AudioClip[] attackSounds2;
	public AudioClip[] attackSounds3;
	public AudioClip[] attackSounds4;
	public AudioClip[] attackSounds5;

	public List<EnemySpawn> spawnZones = new List<EnemySpawn> ();
	public Dictionary<string, List<EnemyCorpse>> corpseDict = new Dictionary<string, List<EnemyCorpse>> ();

	//Pathfinding vars
	private List<Area> areas = new List<Area> ();
	private List<Area> visitedAreas;
	private Queue<Area> areasToSearch;

	private Dictionary<string, Dictionary<string, string>> pathfindingMap;

	private GameObject player;
	private PlayerController playerCon;
	private SpawnInstance nextSpawn;
	private Cinematic nextCinematic;

	//Audio multipliers
	public float masterVolume;
	public float musicVolume;
	public float effectsVolume;

	//Stat tracking
	public int[] killTotals;
	private int itemsCrafted;

	private bool isGameOver;
	[HideInInspector]
	public bool isPaused;
	[HideInInspector]
	private bool timerRunning;
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
		CinematicMap.rebuildMap ();

		nextSpawn = SpawnMap.getNextSpawn ();
		nextCinematic = CinematicMap.getNextCinematic ();

		attackSoundMaster = new AudioClip[5][];
		attackSoundMaster [0] = attackSounds1;
		attackSoundMaster [1] = attackSounds2;
		attackSoundMaster [2] = attackSounds3;
		attackSoundMaster [3] = attackSounds4;
		attackSoundMaster [4] = attackSounds5;

		buildPathingMap ();

		//Load settings
		updateVolume();

		devMode = Scenes.getParam ("devMode");
		if (devMode == null) {
			devMode = "false";
		}

		foreach (GameObject area in GameObject.FindGameObjectsWithTag ("Area")) {
			areas.Add(area.GetComponent<Area>());
		}

		RecipeBook.loadRecipes (System.IO.Path.Combine(Application.streamingAssetsPath, "RecipeMaster.csv"));

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
			TimeSpan timeSpan = TimeSpan.FromSeconds((prepTime - Mathf.Floor (timer)));
			timerText.text = string.Format ("{0:D2}:{1:d2}", timeSpan.Minutes, timeSpan.Seconds);
			if (Mathf.Floor (timer) >= prepTime) {
				timerText.enabled = false;
				changePhase ("siege");
				onSiegePhase ();
			}
		} else if (phase == "siege") {
			if (Scenes.getParam ("devMode") == "false") {
				checkForEnemySpawns ();
			}
		}

		checkForCinematics ();

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

		//Pause Menu
		if (Input.GetKeyDown("escape")) {
			if (isPaused) {
				unpauseGame ();
			} else {
				pauseGame ();
			}
		}

		if ((Scenes.getParam("devMode") != "false") && Input.GetKeyDown ("t")) {
			spawnEnemyRand ();
		} else if ((Scenes.getParam("devMode") != "false") && Input.GetKeyDown ("y")) {
			spawnBoss (spawnZones[0]);
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
		spawnEnemy (spawnZones[1]);
	}

	private void checkForEnemySpawns() {
		if (nextSpawn != null && timer >= nextSpawn.spawnTime) {
			if (!nextSpawn.isBoss) {
				for (int i = 0; i < nextSpawn.spawnCount; i++) {
					Invoke ("spawnEnemyRand", i * 0.3f);
				}
				nextSpawn = SpawnMap.getNextSpawn ();
			} else {
				spawnBoss (spawnZones[0]);
			}

		}
	}

	private void checkForCinematics() {
		if (nextCinematic != null && phase == nextCinematic.phase && timer >= nextCinematic.playTime) {
			if (nextCinematic.dialog != null) {
				if (nextCinematic.dialog.Length > 1) {
					showDialog (nextCinematic.dialog[0]);
				} else {
					StartCoroutine ("playConversation", nextCinematic.dialog);
				}
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

	private void buildPathingMap() {
		//Build pathing map
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
		yardMap.Add ("Yard", "TrYardKitchen");
		yardMap.Add ("Garage","TrYardGarage");
		pathfindingMap.Add ("Yard", yardMap);
	}

	public Transition findRouteToPlayer(Area currentArea) {
		//visitedAreas = new List<Area> ();
		//areasToSearch = new Queue<Area> ();
		//areasToSearch.Enqueue (currentArea);

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

	/*
	private Transition searchForPlayer2() {
		Area currentArea = areasToSearch.Peek ();
		visitedAreas.Add (currentArea);
		foreach (Transition transition in currentArea.transitions) {
			Area neighbor = transition.sibling.transform.parent.GetComponent<Area> ();
			if (neighbor == playerCon.currentArea) {
				return transition;
			}
			if (!visitedAreas.Contains (neighbor) && !areasToSearch.Contains(neighbor)) {
				areasToSearch.Enqueue (neighbor);
			}
		}

		areasToSearch.Dequeue ();

		if (areasToSearch.Count > 0) {
			return searchForPlayer2 ();
		}

		return null;
	}
	*/

	public void gameOver() {
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

		fadeToMenu ();
	}

	private void fadeToMenu() {
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
		//Use Fiona's sprite if none is provided
		if (dialog.sprites == null) {
			dialog.sprites = new Sprite[] {dialogSprites [0], dialogSprites[1]};
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

	public void returnToMenu() {
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

		SceneManager.LoadScene("ResultsScreen");
	}
}
