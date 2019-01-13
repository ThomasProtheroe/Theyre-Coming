using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabExit : Transition {

	[Header("Lab Specific Settings")]
	[SerializeField]
	private GameController gameCon;

	public override void onPlayerArrival() {
		gameCon.deactivateAllEnemies ();
		gameCon.pauseTimer ();
	}
}
