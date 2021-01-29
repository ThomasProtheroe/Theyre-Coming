using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bed : Interactive {

	private PlayerController playerCon;
	private GameController gameCon;
	private SoundController soundCon;
	[SerializeField]
	private AudioClip sleepSound;

	private bool inUse;

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
		goToBed ();
	}

	private void goToBed() {
		string phase = gameCon.getPhase();
		if (phase == "downtime") {
			StartCoroutine("GoToBed");
		} else {
			finishUse();
		}
	}

	public void finishUse() {
		playerCon.isBusy = false;
	}

	override public void updateHighlightColor() {
		string phase = gameCon.getPhase();
		if (phase == "downtime") {
			GetComponent<SpriteOutline> ().color = positiveColor;
		} else {
			GetComponent<SpriteOutline> ().color = negativeColor;
		}
	}

	IEnumerator GoToBed() {
		gameCon.miscFadeOut(0.005f);
		yield return new WaitForSeconds (3.0f);
		soundCon.playPriorityOneShot (sleepSound);
		yield return new WaitForSeconds (1.0f);
		playerCon.goToSleep();
		gameCon.miscFadeIn(0.005f);
		yield return new WaitForSeconds (3.0f);
		gameCon.startNewNight();
		finishUse();
	}
}
