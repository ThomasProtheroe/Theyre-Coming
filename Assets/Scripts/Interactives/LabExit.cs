﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabExit : Transition {

	[Header("Lab Specific Settings")]
	[SerializeField]
	private GameController gameCon;
	private PlayerController playerCon;
	[SerializeField]
	private ElevatorController elevatorCon;
	public MusicController musicCon;

	void Start() {
		playerCon = GameObject.FindGameObjectWithTag ("Player").GetComponent <PlayerController> ();
	}

	public override void onPlayerArrival() {
		gameCon.deactivateAllEnemies ();
		gameCon.pauseTimer ();
		gameCon.miscFadeIn ();
		gameCon.startElevatorCinematic ();

		musicCon.changeMusic ("lab");

		elevatorCon.elevatorRunning = true;
	}
}
