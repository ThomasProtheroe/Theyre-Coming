using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamingBat : Weapon {

	public override void onEnemyImpact(GameObject enemy) {
		Debug.Log ("hit");
		enemy.GetComponent<Enemy> ().setBurning ();

		base.onEnemyImpact (enemy);
	}
}
