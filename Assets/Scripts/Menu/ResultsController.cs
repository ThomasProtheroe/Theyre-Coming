using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultsController : MonoBehaviour {

	[SerializeField]
	private Text timeText;
	[SerializeField]
	private Text killsText;
	[SerializeField]
	private Text typeText;

	public Camera camera0;
	public Camera camera1;

	// Use this for initialization
	void Start () {
		//Update all stats from Scene params
		if (Scenes.getParam ("result") == "victory") {
			timeText.text = "You survived the undead horde!";
		} else {
			float resultsTime = float.Parse(Scenes.getParam ("resultsTime"));
			TimeSpan timeSpan = TimeSpan.FromSeconds(resultsTime);
			timeText.text = "You managed to stay alive for " + timeSpan.Minutes + " minutes and " + timeSpan.Seconds + " seconds.";
		}

		int totalKills = int.Parse (Scenes.getParam ("resultsKills"));
		killsText.text = "You killed " + totalKills + " undead.";

		typeText.text = "Your favorite method of destruction was " + Scenes.getParam ("resultsType") + ".";

		if (Scenes.getParam ("result") == "victory") {
			//TODO Replace this with victory screen
			camera1.enabled = true;
		} else {
			if (totalKills < 50) {
				camera0.enabled = true;
			} else if (totalKills >= 50) {
				camera1.enabled = true;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown) {
			Scenes.clearParams ();
			Scenes.Load ("MainMenu");
		}
	}
}
