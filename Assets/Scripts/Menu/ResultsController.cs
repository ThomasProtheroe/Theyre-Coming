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

	// Use this for initialization
	void Start () {
		//Update all stats from Scene params
		float resultsTime = float.Parse(Scenes.getParam ("resultsTime"));
		TimeSpan timeSpan = TimeSpan.FromSeconds(resultsTime);
		timeText.text = "You managed to stay alive for " + timeSpan.Minutes + " minutes and " + timeSpan.Seconds + "seconds";

		killsText.text = "You killed " + int.Parse (Scenes.getParam ("resultsKills")) + " undead.";

		typeText.text = "Your favorite weapon of destruction was " + Scenes.getParam ("resultsType");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
