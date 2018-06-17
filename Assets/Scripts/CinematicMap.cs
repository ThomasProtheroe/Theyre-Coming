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

		map.Enqueue (new Cinematic("prep", 25.0f, new Dialog[] { new Dialog("\nDoesn't look like Grandpa is home. I hope he's OK...", null, 6.0f) }));
		map.Enqueue (new Cinematic("prep", 60.0f, null, 1));
		map.Enqueue (new Cinematic("siege", 190.0f, new Dialog[] { new Dialog("\nThey just keep coming... How am I supposed to get out of this alive?", null, 6.0f) }));
		map.Enqueue (new Cinematic("siege", 445.0f, new Dialog[] { new Dialog("\nThat's weird, I don't hear many more of them outside. Are they finally letting up?", null, 5.0f) }));
		map.Enqueue (new Cinematic("siege", 455.0f, new Dialog[] { new Dialog("\nFIOOOOONNNNNNAAAAAA!", "boss", 5.0f) }));
		map.Enqueue (new Cinematic("siege", 460.0f, new Dialog[] { new Dialog("\nWhat the hell was that? Sound like it came from the backyard.", null, 5.0f) }));
		map.Enqueue (new Cinematic("siege", 630.0f, new Dialog[] { new Dialog("\nI can hear more of them outside, there must be a hundred of them. Better get ready.", null, 6.0f) }));
		map.Enqueue (new Cinematic("siege", 654.0f, new Dialog[] { new Dialog("\nHere they come. Looks like this is it Fiona, last stand time!", null, 6.0f) }));
	}
}

public class Cinematic {
	public float playTime { get; set; }
	public Dialog[] dialog { get; set; }
	public int clipIndex { get; set; }
	public string phase { get; set;}
	string cinematicCoroutine { get; set;}

	public Cinematic(string startPhase, float time, Dialog[] dialogArray, int clip=-1, string functionName=null) {
		phase = startPhase;
		playTime = time;
		dialog = dialogArray;
		clipIndex = clip;
		cinematicCoroutine = functionName;
	}
}
