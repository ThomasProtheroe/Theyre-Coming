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
		map.Enqueue (new SpawnInstance(20.0f, 2));
		map.Enqueue (new SpawnInstance(60.0f, 4));
		map.Enqueue (new SpawnInstance(100.0f, 5));
		map.Enqueue (new SpawnInstance(105.0f, 3));
		map.Enqueue (new SpawnInstance(160.0f, 12));
		map.Enqueue (new SpawnInstance(2050.0f, 7));
		map.Enqueue (new SpawnInstance(210.0f, 9));
		map.Enqueue (new SpawnInstance(255.0f, 4));
		map.Enqueue (new SpawnInstance(285.0f, 6));
		map.Enqueue (new SpawnInstance(321.0f, 3));
		map.Enqueue (new SpawnInstance(322.0f, 5));
		map.Enqueue (new SpawnInstance(323.0f, 4));
		map.Enqueue (new SpawnInstance(324.0f, 3));
		map.Enqueue (new SpawnInstance(325.0f, 4));
		map.Enqueue (new SpawnInstance(385.0f, 18));
		map.Enqueue (new SpawnInstance(410.0f, 5));
		map.Enqueue (new SpawnInstance(440.0f, 25));
		map.Enqueue (new SpawnInstance(520.0f, 40));
	}
}

public class SpawnInstance {
	public float spawnTime { get; set; }
	public int spawnCount { get; set;}

	public SpawnInstance(float time, int count) {
		spawnTime = time;
		spawnCount = count;
	}
}
