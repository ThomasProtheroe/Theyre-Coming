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
		map.Enqueue (new SpawnInstance(5.0f, 1));
		map.Enqueue (new SpawnInstance(20.0f, 2));
		map.Enqueue (new SpawnInstance(60.0f, 4));
		map.Enqueue (new SpawnInstance(100.0f, 5));
		map.Enqueue (new SpawnInstance(105.0f, 3));
		map.Enqueue (new SpawnInstance(150.0f, 12));
		map.Enqueue (new SpawnInstance(190.0f, 9));
		map.Enqueue (new SpawnInstance(195.0f, 9));
		map.Enqueue (new SpawnInstance(240.0f, 4));
		map.Enqueue (new SpawnInstance(270.0f, 5));
		map.Enqueue (new SpawnInstance(301.0f, 3));
		map.Enqueue (new SpawnInstance(302.0f, 5));
		map.Enqueue (new SpawnInstance(303.0f, 4));
		map.Enqueue (new SpawnInstance(304.0f, 3));
		map.Enqueue (new SpawnInstance(350.0f, 18));
		map.Enqueue (new SpawnInstance(360.0f, 5));
		map.Enqueue (new SpawnInstance(415.0f, 25));
		map.Enqueue (new SpawnInstance(480.0f, 40));
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
