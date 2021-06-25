using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BileProjectile : MonoBehaviour {

	public float lifetime;
	public int damage;

	// Update is called once per frame
	void Update () {
		lifetime -= Time.deltaTime;
		if (lifetime <= 0) {
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Player") {
			PlayerController player = other.gameObject.GetComponent<PlayerController> ();
			if (!player.isInvulnerable) {
				player.takeBileHit (damage);
			}
		}

		if (other.gameObject.tag == "Enemy") {
			other.gameObject.GetComponent<Enemy> ().stopBurning(true);

		}
	}
}
