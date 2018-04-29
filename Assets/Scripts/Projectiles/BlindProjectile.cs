using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindProjectile : MonoBehaviour {

	public float lifetime;
	public bool destroyOnContact;
	public Color color;

	// Update is called once per frame
	void Update () {
		lifetime -= Time.deltaTime;
		if (lifetime <= 0) {
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Enemy") {
			Enemy enemy = other.gameObject.GetComponent<Enemy> ();
			if (!enemy.isInvunlerable && !enemy.getIsDead ()) {
				enemy.triggerSplash (color);
				enemy.setBlind ();
				Destroy (gameObject);
			}
		}
	}
}
