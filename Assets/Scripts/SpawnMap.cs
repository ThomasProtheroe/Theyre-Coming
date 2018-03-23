using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpawnMap {
	private static Queue<SpawnInstance> map;

	static SpawnMap() {
		map = new Queue<SpawnInstance> ();

		buildMap ();
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
		map.Enqueue (new SpawnInstance(5.0f, 1));
		map.Enqueue (new SpawnInstance(25.0f, 2));
		map.Enqueue (new SpawnInstance(45.0f, 5));
		map.Enqueue (new SpawnInstance(70.0f, 6));
		map.Enqueue (new SpawnInstance(80.0f, 2));
		map.Enqueue (new SpawnInstance(110.0f, 20));
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
