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

	void Update () {
		if (isAirborn && transform.position.y < -2.9f) {
			Rigidbody2D rb = GetComponent<Rigidbody2D> ();
			rb.bodyType = RigidbodyType2D.Kinematic;
			rb.velocity = Vector2.zero;
			isAirborn = false;

			StartCoroutine ("ExtinguishAfterTime", 2.0f);
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (isAirborn && other.gameObject.tag == "Enemy") {
			other.gameObject.GetComponent<Enemy> ().takeFireHit(activeDamage);
		}
	}

	void OnTriggerStay2D (Collider2D other) {
		if (!isAirborn) {
			if (other.gameObject.tag == "Enemy") {
				other.gameObject.GetComponent<Enemy> ().takeFireHit (passiveDamage);
			} else if (other.gameObject.tag == "Player") {
				other.gameObject.GetComponent<PlayerController> ().takeFireHit ();
			}
		}
	}

	IEnumerator ExtinguishAfterTime(float time) {
		yield return new WaitForSeconds(time);

		StartCoroutine ("Extinguish");
	}

	IEnumerator Extinguish() {
		ParticleSystem ps = GetComponent<ParticleSystem> ();
		var main = ps.main;
		float newStartSize = main.startSize.constant;
		for (float f = 1f; f >= 0.0f; f -= 0.01f) {
			main.startSize = newStartSize * f;

			yield return null;
		}

		Destroy (gameObject);
	}
}
