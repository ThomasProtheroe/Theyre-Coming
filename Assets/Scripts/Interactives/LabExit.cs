using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabExit : Transition {

	[Header("Lab Specific Settings")]
	[SerializeField]
	private GameController gameCon;
	[SerializeField]
	private ElevatorController elevatorCon;
	public MusicController musicCon;

	public override void onPlayerArrival() {
		gameCon.deactivateAllEnemies ();
		gameCon.pauseTimer ();

		musicCon.changeMusic ("lab");

		elevatorCon.elevatorRunning = true;
	}
}
