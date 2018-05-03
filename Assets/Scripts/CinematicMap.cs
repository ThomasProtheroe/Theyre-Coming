using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CinematicMap {
	private static Queue<Cinematic> map;

	static CinematicMap() {
		map = new Queue<Cinematic> ();

		buildMap ();
	}

	public static void rebuildMap() {
		buildMap();
	}

	public static Cinematic getNextCinematic() {
		Cinematic nextCinematic;
		if (map.Count > 0) {
			nextCinematic = map.Dequeue ();
		} else {
			nextCinematic = null;
		}
		return nextCinematic;
	}


	private static void buildMap() {
		map = new Queue<Cinematic> ();
		map.Enqueue (new Cinematic("prep", 10.0f, new Dialog[] { new Dialog("Doesn't look like Grandpa is home. I hope he's OK...", null, 6.0f) }));
	}
}

public class Cinematic {
	public float playTime { get; set; }
	public Dialog[] dialog { get; set;}
	public string phase { get; set;}
	string cinematicCoroutine { get; set;}

	public Cinematic(string startPhase, float time, Dialog[] dialogArray, string functionName=null) {
		phase = startPhase;
		playTime = time;
		dialog = dialogArray;
		cinematicCoroutine = functionName;
	}
}
