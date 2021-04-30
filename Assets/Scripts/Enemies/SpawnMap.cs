using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpawnMap {
	private static Queue<SpawnInstance> map;
	
	private static string mode;
	private static int difficulty;
	private static int night;
	private static int difficultyCurve;

	public static void setMode(string inMode) {
		mode = inMode;
	}

	public static void setDifficultyCurve(int inCurve) {
		difficultyCurve = inCurve;
	}

	public static SpawnInstance getNextSpawn() {
		SpawnInstance nextSpawn = null;

		if (map == null || map.Count == 0) {
			return null;
		} 
		nextSpawn = map.Dequeue ();
		
		return nextSpawn;
	}

	public static void startEndlessMode() {
		map = new Queue<SpawnInstance> ();

		night = 1;
		difficulty = 8;
		difficultyCurve = 2;
	}

	public static void startStoryMode() {
		map = new Queue<SpawnInstance> ();

		night = 1;
		difficulty = 10;
		difficultyCurve = 2;
	}

	public static void buildWave() {
		Debug.Log("Building Wave " + night.ToString());
		map = new Queue<SpawnInstance> ();

		int enemyCount = (int)((float)difficulty * 2.5f);
		int variance = Mathf.RoundToInt(enemyCount * 0.2f);
		enemyCount += Random.Range(variance * -1, variance + 1);

		//Add specials
		float runnerChance = 0f;
		if (night > 2) {
			runnerChance = 0.15f;
		} else if (night > 5) {
			runnerChance = 0.22f;
		}

		float nexttWaveStartTime = 10f;

		int spawnGroupMax = Mathf.RoundToInt(enemyCount / 4);
		int spawnGroupMin = Mathf.RoundToInt(enemyCount / 10);
		if (spawnGroupMin < 2) {
			spawnGroupMin = 2;
		}
		int spawnGroupCount = Random.Range(spawnGroupMin, spawnGroupMax + 1);
		int enemiesPerGroup = Mathf.RoundToInt(enemyCount / spawnGroupCount);


		Debug.Log("Start Time: " + nexttWaveStartTime.ToString());
		//Debug.Log("Difficulty: " + difficulty.ToString());
		Debug.Log("Number of Enemies: " + enemyCount.ToString());
		Debug.Log("Number of Groups: " + spawnGroupCount.ToString());
		//Debug.Log("Enemies per Group: " + enemiesPerGroup.ToString());


		//Generate spawn instances based on above stats and add them to the map
		for(int i = 0; i < spawnGroupCount; i++) {
			int groupSize = Random.Range(Mathf.RoundToInt(enemiesPerGroup * 0.9f), Mathf.RoundToInt(enemiesPerGroup * 1.1f) + 1);
			if (enemyCount < groupSize || (i == spawnGroupCount - 1)) {
				groupSize = enemyCount;
			}
			
			//Generate runners
			int runnerCount = 0;
			if (runnerChance > 0f) {
				for(int n = 0; n < groupSize; n++) {
					if (Random.Range(0f, 1f) < runnerChance) {
						runnerCount ++;
						groupSize --;
					}
				}
			}
			
			map.Enqueue (new SpawnInstance (nexttWaveStartTime, groupSize, Constants.ENEMY_TYPE_NORMAL));
			if (runnerCount > 0) {
				map.Enqueue (new SpawnInstance (nexttWaveStartTime, runnerCount, Constants.ENEMY_TYPE_RUNNER));
			}
			

			nexttWaveStartTime += Random.Range(15f, 22f);
			enemyCount -= groupSize;
			enemyCount -= runnerCount;

			/*
			Debug.Log("Creating group " + i.ToString());
			Debug.Log("Group Size: " + groupSize.ToString());
			Debug.Log("Enemies left in wave: " + enemyCount.ToString());
			*/
			if (enemyCount <= 0) {
				break;
			}
		}

		//Increase difficulty
		difficulty += difficultyCurve;
		night++;
	}

/*
	private static void buildMap() {
		int mapType = Random.Range (0, 5);
		map = new Queue<SpawnInstance> ();
		if (mapType == 0) {
			map.Enqueue (new SpawnInstance (5.0f, 3));
			map.Enqueue (new SpawnInstance (30.0f, 5));
			map.Enqueue (new SpawnInstance (70.0f, 6));
			map.Enqueue (new SpawnInstance (71.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance (119.0f, 2, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance (120.0f, 5));
			map.Enqueue (new SpawnInstance (125.0f, 3));
			map.Enqueue (new SpawnInstance (160.0f, 2));
			map.Enqueue (new SpawnInstance (161.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance (175.0f, 1));
			map.Enqueue (new SpawnInstance (210.0f, 10));
			map.Enqueue (new SpawnInstance (255.0f, 7));
			map.Enqueue (new SpawnInstance (256.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance (280.0f, 9));
			map.Enqueue (new SpawnInstance (355.0f, 8));
			map.Enqueue (new SpawnInstance (356.0f, 3, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance (410.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance (411.0f, 3));
			map.Enqueue (new SpawnInstance (412.0f, 5));
			map.Enqueue (new SpawnInstance (412.5f, 2, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance (413.0f, 4));
			map.Enqueue (new SpawnInstance (414.0f, 3));
			map.Enqueue (new SpawnInstance (415.0f, 4));
			map.Enqueue (new SpawnInstance (416.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance (475.0f, 18));
			map.Enqueue (new SpawnInstance (540.0f, 1, Constants.ENEMY_TYPE_BOSS));
			map.Enqueue (new SpawnInstance (550.0f, 2));
			map.Enqueue (new SpawnInstance (550.0f, 1));
			map.Enqueue (new SpawnInstance (557.0f, 2));
			map.Enqueue (new SpawnInstance (570.0f, 1));
			map.Enqueue (new SpawnInstance (575.0f, 3, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance (580.0f, 1));
			map.Enqueue (new SpawnInstance (590.0f, 2));
			map.Enqueue (new SpawnInstance (680.0f, 55));
			map.Enqueue (new SpawnInstance (681.0f, 6, Constants.ENEMY_TYPE_RUNNER));
		} else if (mapType == 1) {
			map.Enqueue (new SpawnInstance(5.0f, 3));
			map.Enqueue (new SpawnInstance(30.0f, 5));
			map.Enqueue (new SpawnInstance(70.0f, 6));
			map.Enqueue (new SpawnInstance(71.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(119.0f, 2, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(120.0f, 5));
			map.Enqueue (new SpawnInstance(125.0f, 3));
			map.Enqueue (new SpawnInstance(160.0f, 2));
			map.Enqueue (new SpawnInstance(161.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(175.0f, 1));
			map.Enqueue (new SpawnInstance(210.0f, 10));
			map.Enqueue (new SpawnInstance(255.0f, 7));
			map.Enqueue (new SpawnInstance(256.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(280.0f, 9));
			map.Enqueue (new SpawnInstance(355.0f, 8));
			map.Enqueue (new SpawnInstance(356.0f, 3, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(410.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(411.0f, 3));
			map.Enqueue (new SpawnInstance(412.0f, 5));
			map.Enqueue (new SpawnInstance(412.5f, 2, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(413.0f, 4));
			map.Enqueue (new SpawnInstance(414.0f, 3));
			map.Enqueue (new SpawnInstance(415.0f, 4));
			map.Enqueue (new SpawnInstance(416.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(475.0f, 18));
			map.Enqueue (new SpawnInstance(540.0f, 1, Constants.ENEMY_TYPE_BOSS));
			map.Enqueue (new SpawnInstance(550.0f, 2));
			map.Enqueue (new SpawnInstance(550.0f, 1));
			map.Enqueue (new SpawnInstance(557.0f, 2));
			map.Enqueue (new SpawnInstance(570.0f, 1));
			map.Enqueue (new SpawnInstance(575.0f, 3, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(580.0f, 1));
			map.Enqueue (new SpawnInstance(590.0f, 2));
			map.Enqueue (new SpawnInstance(680.0f, 62));
			map.Enqueue (new SpawnInstance(680.0f, 8, Constants.ENEMY_TYPE_RUNNER));
		} else if (mapType == 2) {
			map.Enqueue (new SpawnInstance(5.0f, 3));
			map.Enqueue (new SpawnInstance(30.0f, 7));
			map.Enqueue (new SpawnInstance(70.0f, 8));
			map.Enqueue (new SpawnInstance(71.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(119.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(120.0f, 6));
			map.Enqueue (new SpawnInstance(125.0f, 4));
			map.Enqueue (new SpawnInstance(160.0f, 5));
			map.Enqueue (new SpawnInstance(175.0f, 2));
			map.Enqueue (new SpawnInstance(210.0f, 11));
			map.Enqueue (new SpawnInstance(255.0f, 7));
			map.Enqueue (new SpawnInstance(256.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(280.0f, 10));
			map.Enqueue (new SpawnInstance(355.0f, 9));
			map.Enqueue (new SpawnInstance(356.0f, 2, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(411.0f, 4));
			map.Enqueue (new SpawnInstance(412.0f, 6));
			map.Enqueue (new SpawnInstance(412.5f, 2, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(413.0f, 4));
			map.Enqueue (new SpawnInstance(414.0f, 3));
			map.Enqueue (new SpawnInstance(415.0f, 4));
			map.Enqueue (new SpawnInstance(416.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(475.0f, 18));
			map.Enqueue (new SpawnInstance(540.0f, 1, Constants.ENEMY_TYPE_BOSS));
			map.Enqueue (new SpawnInstance(550.0f, 2));
			map.Enqueue (new SpawnInstance(550.0f, 1));
			map.Enqueue (new SpawnInstance(557.0f, 2));
			map.Enqueue (new SpawnInstance(570.0f, 1));
			map.Enqueue (new SpawnInstance(575.0f, 2, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(580.0f, 1));
			map.Enqueue (new SpawnInstance(590.0f, 2));
			map.Enqueue (new SpawnInstance(680.0f, 62));
			map.Enqueue (new SpawnInstance(680.0f, 4, Constants.ENEMY_TYPE_RUNNER));
		} else if (mapType == 3) {
			map.Enqueue (new SpawnInstance(5.0f, 3));
			map.Enqueue (new SpawnInstance(30.0f, 5));
			map.Enqueue (new SpawnInstance(70.0f, 6));
			map.Enqueue (new SpawnInstance(71.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(119.0f, 2, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(120.0f, 5));
			map.Enqueue (new SpawnInstance(125.0f, 3));
			map.Enqueue (new SpawnInstance(160.0f, 2));
			map.Enqueue (new SpawnInstance(161.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(175.0f, 1));
			map.Enqueue (new SpawnInstance(210.0f, 10));
			map.Enqueue (new SpawnInstance(240.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(255.0f, 7));
			map.Enqueue (new SpawnInstance(256.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(280.0f, 9));
			map.Enqueue (new SpawnInstance(340.0f, 2, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(355.0f, 8));
			map.Enqueue (new SpawnInstance(356.0f, 3, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(410.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(411.0f, 3));
			map.Enqueue (new SpawnInstance(412.0f, 5));
			map.Enqueue (new SpawnInstance(412.5f, 2, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(413.0f, 4));
			map.Enqueue (new SpawnInstance(414.0f, 3));
			map.Enqueue (new SpawnInstance(415.0f, 4));
			map.Enqueue (new SpawnInstance(416.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(475.0f, 18));
			map.Enqueue (new SpawnInstance(540.0f, 1, Constants.ENEMY_TYPE_BOSS));
			map.Enqueue (new SpawnInstance(550.0f, 2));
			map.Enqueue (new SpawnInstance(550.0f, 1));
			map.Enqueue (new SpawnInstance(556.0f, 2, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(557.0f, 2));
			map.Enqueue (new SpawnInstance(570.0f, 1));
			map.Enqueue (new SpawnInstance(575.0f, 3, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(580.0f, 1));
			map.Enqueue (new SpawnInstance(590.0f, 2));
			map.Enqueue (new SpawnInstance(680.0f, 55));
			map.Enqueue (new SpawnInstance(680.0f, 6, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(690.0f, 2, Constants.ENEMY_TYPE_RUNNER));
		} else if (mapType == 4) {
			map.Enqueue (new SpawnInstance(5.0f, 3));
			map.Enqueue (new SpawnInstance(30.0f, 5));
			map.Enqueue (new SpawnInstance(70.0f, 6));
			map.Enqueue (new SpawnInstance(71.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(119.0f, 2, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(120.0f, 5));
			map.Enqueue (new SpawnInstance(125.0f, 3));
			map.Enqueue (new SpawnInstance(160.0f, 2));
			map.Enqueue (new SpawnInstance(161.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(175.0f, 1));
			map.Enqueue (new SpawnInstance(210.0f, 10));
			map.Enqueue (new SpawnInstance(255.0f, 7));
			map.Enqueue (new SpawnInstance(256.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(280.0f, 9));
			map.Enqueue (new SpawnInstance(355.0f, 8));
			map.Enqueue (new SpawnInstance(356.0f, 3, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(410.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(411.0f, 3));
			map.Enqueue (new SpawnInstance(412.0f, 5));
			map.Enqueue (new SpawnInstance(412.5f, 2, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(413.0f, 4));
			map.Enqueue (new SpawnInstance(414.0f, 3));
			map.Enqueue (new SpawnInstance(415.0f, 4));
			map.Enqueue (new SpawnInstance(416.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(475.0f, 18));
			map.Enqueue (new SpawnInstance(540.0f, 1, Constants.ENEMY_TYPE_BOSS));
			map.Enqueue (new SpawnInstance(550.0f, 2));
			map.Enqueue (new SpawnInstance(550.0f, 1));
			map.Enqueue (new SpawnInstance(557.0f, 2));
			map.Enqueue (new SpawnInstance(570.0f, 1));
			map.Enqueue (new SpawnInstance(575.0f, 3, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(580.0f, 1));
			map.Enqueue (new SpawnInstance(590.0f, 2));
			map.Enqueue (new SpawnInstance(680.0f, 62));
			map.Enqueue (new SpawnInstance(680.0f, 8, Constants.ENEMY_TYPE_RUNNER));
		}
	}
	*/
}

public class SpawnInstance {
	public float spawnTime { get; set; }
	public int spawnCount { get; set; }
	public int enemyType { get; set; }

	public SpawnInstance(float time, int count, int type=Constants.ENEMY_TYPE_NORMAL) {
		spawnTime = time;
		spawnCount = count;
		enemyType = type;
	}
}
