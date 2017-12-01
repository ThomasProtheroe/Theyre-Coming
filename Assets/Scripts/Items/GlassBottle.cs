using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBottle : Weapon {

	void Start() {
		xOffset = 0.08f;
		yOffset = -0.2f;
		zRotation = 20.0f;
		restingHeight = -2.9f;
		restingRotation = 90;
	}

	public override void onBreak() {
		Debug.Log ("create shards");
	}

	public override void onTerrainImpact() {
		breakItem ();
	}
}
