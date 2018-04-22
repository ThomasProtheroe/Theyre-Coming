using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireGlob : MonoBehaviour {

	[SerializeField]
	private int passiveDamage;
	[SerializeField]
	private int activeDamage;
	[SerializeField]
	private float lifetime;

	private bool isAirborn = true;

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Enemy") {
			if (isAirborn) {
				other.gameObject.GetComponent<Enemy> ().takeFireHit(activeDamage);
			} else {
				other.gameObject.GetComponent<Enemy> ().takeFireHit(passiveDamage);
			}

		} else if (other.gameObject.tag == "Terrain") {
			Rigidbody2D rb = GetComponent<Rigidbody2D> ();
			rb.bodyType = RigidbodyType2D.Kinematic;
			rb.velocity = Vector2.zero;
			isAirborn = false;

			GetComponent<DestroyAfterTime> ().StartCoroutine ("destroyAfterTime", 10.0f);
		}
	}
}
