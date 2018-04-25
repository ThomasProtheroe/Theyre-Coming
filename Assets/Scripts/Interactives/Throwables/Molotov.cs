using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molotov : Throwable {

	[SerializeField]
	private FireGlob fireGlob;
	[SerializeField]
	private int globCount;
	[SerializeField]
	private float maxYVel;
	[SerializeField]
	private float minYVel;
	[SerializeField]
	private float maxXVel;
	[SerializeField]
	private float minXVel;
	[SerializeField]
	private float enemyXMod;
	[SerializeField]
	private float enemyYMod;

	bool enemyImpact = false;

	public override void onEnemyImpact(GameObject enemy) {
		enemyImpact = true;
		breakItem ();
	}

	public override void onTerrainImpact() {
		breakItem ();
	}
		
	public override bool onBreak() {
		if (enemyImpact) {
			maxXVel += enemyXMod;
			maxYVel -= enemyYMod;
		}

		for (int i = 0; i < globCount; i++) {
			FireGlob newGlob = Instantiate (fireGlob, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);

			Rigidbody2D rb = newGlob.GetComponent<Rigidbody2D> ();
			rb.velocity = new Vector2 (UnityEngine.Random.Range(minXVel, maxXVel) * throwDirection, UnityEngine.Random.Range(minYVel, maxYVel));
		}
			
		soundController.playPriorityOneShot (breakSound);

		return true;
	}

}
