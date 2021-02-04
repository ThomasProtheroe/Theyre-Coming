using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : Interactive {

	private PlayerController playerCon;
	private GameController gameCon;
	private SoundController soundCon;

	// Use this for initialization
	void Start () {
		playerCon = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
		gameCon = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		soundCon = GameObject.FindGameObjectWithTag ("SoundController").GetComponent<SoundController> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void interact() {
		OpenInternet ();
	}

	private void OpenInternet() {
	}

	override public void updateHighlightColor() {
		string phase = gameCon.getPhase();
		if (phase == "downtime") {
			GetComponent<SpriteOutline> ().color = positiveColor;
		} else {
			GetComponent<SpriteOutline> ().color = negativeColor;
		}
	}
}
