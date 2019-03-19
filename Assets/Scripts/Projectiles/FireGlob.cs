using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireGlob : MonoBehaviour {

	[SerializeField]
	private int passiveDamage;
	public int activeDamage;
	public float lifetime;
	public bool friendlyFire;

	private bool isAirborn = true;


	void FixedUpdate () {
		if (isAirborn && transform.position.y < -2.9f) {
			freezePosition ();
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (isAirborn) {
			if (other.gameObject.tag == "Enemy") {
				other.gameObject.GetComponent<Enemy> ().takeFireHit (activeDamage);
			} else if (other.gameObject.tag == "AreaWall") {
				if (UnityEngine.Random.Range (0, 2) == 0) {
					freezePosition ();
				} else {
					Rigidbody2D rb = GetComponent<Rigidbody2D> ();
					rb.velocity = new Vector2 (rb.velocity.x * -0.5f, 0.0f);
				}
			}
		}
	}

	void OnTriggerStay2D (Collider2D other) {
		if (!isAirborn) {
			if (other.gameObject.tag == "Enemy") {
				other.gameObject.GetComponent<Enemy> ().takeFireHit (passiveDamage);
			} else if (other.gameObject.tag == "Player" && friendlyFire) {
				other.gameObject.GetComponent<PlayerController> ().takeFireHit ();
			}
		}
	}

	public void enableCollisions() {
		foreach (CircleCollider2D collider in GetComponents <CircleCollider2D> ()) {
			if (!collider.isTrigger) {
				collider.enabled = true;
			}
		}
	}

	private void freezePosition () {
		Rigidbody2D rb = GetComponent<Rigidbody2D> ();
		rb.bodyType = RigidbodyType2D.Kinematic;
		rb.velocity = Vector2.zero;
		isAirborn = false;

		StartCoroutine ("ExtinguishAfterTime");
	}

	IEnumerator ExtinguishAfterTime() {
		yield return new WaitForSeconds(lifetime);

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
