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
		map.Enqueue (new SpawnInstance(140.0f, 2));
		map.Enqueue (new SpawnInstance(155.0f, 2));
		map.Enqueue (new SpawnInstance(180.0f, 12));
		map.Enqueue (new SpawnInstance(225.0f, 7));
		map.Enqueue (new SpawnInstance(230.0f, 9));
		map.Enqueue (new SpawnInstance(275.0f, 4));
		map.Enqueue (new SpawnInstance(305.0f, 6));
		map.Enqueue (new SpawnInstance(341.0f, 3));
		map.Enqueue (new SpawnInstance(342.0f, 5));
		map.Enqueue (new SpawnInstance(343.0f, 4));
		map.Enqueue (new SpawnInstance(344.0f, 3));
		map.Enqueue (new SpawnInstance(345.0f, 4));
		map.Enqueue (new SpawnInstance(405.0f, 18));
		map.Enqueue (new SpawnInstance(440.0f, 5));
		map.Enqueue (new SpawnInstance(470.0f, 25));
		map.Enqueue (new SpawnInstance(540.0f, 40));
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
