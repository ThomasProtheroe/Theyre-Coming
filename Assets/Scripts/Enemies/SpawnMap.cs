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
		map = new Queue<SpawnInstance> ();
		map.Enqueue (new SpawnInstance(5.0f, 2));
		map.Enqueue (new SpawnInstance(30.0f, 2));
		map.Enqueue (new SpawnInstance(70.0f, 4));
		map.Enqueue (new SpawnInstance(120.0f, 5));
		map.Enqueue (new SpawnInstance(125.0f, 3));
		map.Enqueue (new SpawnInstance(160.0f, 2));
		map.Enqueue (new SpawnInstance(175.0f, 1));
		map.Enqueue (new SpawnInstance(210.0f, 10));
		map.Enqueue (new SpawnInstance(255.0f, 7));
		map.Enqueue (new SpawnInstance(280.0f, 9));
		map.Enqueue (new SpawnInstance(355.0f, 8));
		map.Enqueue (new SpawnInstance(411.0f, 3));
		map.Enqueue (new SpawnInstance(412.0f, 5));
		map.Enqueue (new SpawnInstance(413.0f, 4));
		map.Enqueue (new SpawnInstance(414.0f, 3));
		map.Enqueue (new SpawnInstance(415.0f, 4));
		map.Enqueue (new SpawnInstance(475.0f, 18));
		map.Enqueue (new SpawnInstance(540.0f, 1, true));
		map.Enqueue (new SpawnInstance(550.0f, 2));
		map.Enqueue (new SpawnInstance(550.0f, 1));
		map.Enqueue (new SpawnInstance(557.0f, 2));
		map.Enqueue (new SpawnInstance(570.0f, 1));
		map.Enqueue (new SpawnInstance(580.0f, 1));
		map.Enqueue (new SpawnInstance(590.0f, 2));
		map.Enqueue (new SpawnInstance(680.0f, 55));
	}
}

public class SpawnInstance {
	public float spawnTime { get; set; }
	public int spawnCount { get; set; }
	public bool isBoss { get; set; }

	public SpawnInstance(float time, int count, bool boss=false) {
		spawnTime = time;
		spawnCount = count;
		isBoss = boss;
	}
}
