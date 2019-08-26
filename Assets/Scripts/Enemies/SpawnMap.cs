using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpawnMap {
	private static Queue<SpawnInstance> map;

	static SpawnMap() {
		map = new Queue<SpawnInstance> ();

		buildMap ();
	}

	public static void rebuildMap() {
		buildMap();
	}

	public static SpawnInstance getNextSpawn() {
		SpawnInstance nextSpawn;
		if (map.Count > 0) {
			nextSpawn = map.Dequeue ();
		} else {
			nextSpawn = null;
		}
		return nextSpawn;
	}


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
