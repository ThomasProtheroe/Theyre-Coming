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
		int mapType = Random.Range (0, 4);
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
			map.Enqueue (new SpawnInstance(5.0f, 2));
			map.Enqueue (new SpawnInstance(30.0f, 3));
			map.Enqueue (new SpawnInstance(40.0f, 15));
			map.Enqueue (new SpawnInstance(70.0f, 5));
			map.Enqueue (new SpawnInstance(90.0f, 7));
			map.Enqueue (new SpawnInstance(91.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(100.0f, 3));
			map.Enqueue (new SpawnInstance(105.0f, 2));
			map.Enqueue (new SpawnInstance(125.0f, 15, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(155.0f, 3));
			map.Enqueue (new SpawnInstance(170.0f, 8, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(190.0f, 5));
			map.Enqueue (new SpawnInstance(200.0f, 4, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(230.0f, 30));
			map.Enqueue (new SpawnInstance(260.0f, 5, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(290.0f, 15));
			map.Enqueue (new SpawnInstance(310.0f, 3));
			map.Enqueue (new SpawnInstance(340.0f, 25, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(375.0f, 18));
			map.Enqueue (new SpawnInstance(400.0f, 1));
			map.Enqueue (new SpawnInstance(430.5f, 15, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(470.0f, 3));
			map.Enqueue (new SpawnInstance(480.0f, 2));
			map.Enqueue (new SpawnInstance(490.0f, 1));
			map.Enqueue (new SpawnInstance(500.0f, 40));
			map.Enqueue (new SpawnInstance(550.0f, 1, Constants.ENEMY_TYPE_BOSS));
			map.Enqueue (new SpawnInstance(565.0f, 5));
			map.Enqueue (new SpawnInstance(575.0f, 3, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(600.0f, 1));
			map.Enqueue (new SpawnInstance(640.0f, 2));
			map.Enqueue (new SpawnInstance(680.0f, 80));
			map.Enqueue (new SpawnInstance(681.0f, 5, Constants.ENEMY_TYPE_RUNNER));
		} else if (mapType == 2) {
			map.Enqueue (new SpawnInstance(5.0f, 2));
			map.Enqueue (new SpawnInstance(10.0f, 2));
			map.Enqueue (new SpawnInstance(20.0f, 1));
			map.Enqueue (new SpawnInstance(35.0f, 4));
			map.Enqueue (new SpawnInstance(45.0f, 2));
			map.Enqueue (new SpawnInstance(55.0f, 1));
			map.Enqueue (new SpawnInstance(70.0f, 6));
			map.Enqueue (new SpawnInstance(71.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(100.0f, 2, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(101.0f, 5));
			map.Enqueue (new SpawnInstance(115.0f, 3));
			map.Enqueue (new SpawnInstance(125.0f, 3));
			map.Enqueue (new SpawnInstance(160.0f, 8));
			map.Enqueue (new SpawnInstance(161.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(175.0f, 2));
			map.Enqueue (new SpawnInstance(210.0f, 10));
			map.Enqueue (new SpawnInstance(230.0f, 3));
			map.Enqueue (new SpawnInstance(240.0f, 1));
			map.Enqueue (new SpawnInstance(255.0f, 12));
			map.Enqueue (new SpawnInstance(256.0f, 3, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(280.0f, 9));
			map.Enqueue (new SpawnInstance(320.0f, 12));
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
			map.Enqueue (new SpawnInstance(680.0f, 55));
			map.Enqueue (new SpawnInstance(681.0f, 6, Constants.ENEMY_TYPE_RUNNER));
		} else if (mapType == 3) {
			map.Enqueue (new SpawnInstance(5.0f, 3));
			map.Enqueue (new SpawnInstance(30.0f, 5));
			map.Enqueue (new SpawnInstance(70.0f, 6));
			map.Enqueue (new SpawnInstance(71.0f, 1, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(100.0f, 2, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(119.0f, 5, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(120.0f, 2));
			map.Enqueue (new SpawnInstance(125.0f, 1));
			map.Enqueue (new SpawnInstance(160.0f, 1));
			map.Enqueue (new SpawnInstance(161.0f, 4, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(175.0f, 4, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(210.0f, 10));
			map.Enqueue (new SpawnInstance(255.0f, 7));
			map.Enqueue (new SpawnInstance(256.0f, 8, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(280.0f, 2));
			map.Enqueue (new SpawnInstance(355.0f, 8));
			map.Enqueue (new SpawnInstance(356.0f, 4, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(410.0f, 2, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(411.0f, 3));
			map.Enqueue (new SpawnInstance(412.0f, 5));
			map.Enqueue (new SpawnInstance(412.5f, 5, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(413.0f, 2));
			map.Enqueue (new SpawnInstance(414.0f, 1));
			map.Enqueue (new SpawnInstance(415.0f, 4));
			map.Enqueue (new SpawnInstance(416.0f, 10, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(475.0f, 8));
			map.Enqueue (new SpawnInstance(540.0f, 1, Constants.ENEMY_TYPE_BOSS));
			map.Enqueue (new SpawnInstance(550.0f, 2));
			map.Enqueue (new SpawnInstance(550.0f, 1));
			map.Enqueue (new SpawnInstance(557.0f, 2));
			map.Enqueue (new SpawnInstance(570.0f, 1));
			map.Enqueue (new SpawnInstance(575.0f, 3, Constants.ENEMY_TYPE_RUNNER));
			map.Enqueue (new SpawnInstance(580.0f, 1));
			map.Enqueue (new SpawnInstance(590.0f, 2));
			map.Enqueue (new SpawnInstance(680.0f, 40));
			map.Enqueue (new SpawnInstance(681.0f, 15, Constants.ENEMY_TYPE_RUNNER));
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
