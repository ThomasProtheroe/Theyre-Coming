using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : MonoBehaviour {

	public int knockback;
	public bool isActive;
	protected PlayerController player;

	public virtual void Start() {
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
	}

	public virtual void fire() {
		isActive = true;
		transform.parent = null;
	}

	public void breakProjectile() {
		Rigidbody2D body = GetComponent<Rigidbody2D>();
		body.velocity = Vector2.zero;

		gameObject.layer = 11;

		//int xBreakForce = Random.Range(-100, 100);
		//int yBreakForce = Random.Range(60, 100);

		body.bodyType = RigidbodyType2D.Dynamic;
		//body.AddForce (new Vector2 (xBreakForce, yBreakForce));
		body.AddTorque (25.0f);

		StartCoroutine ("beginSpriteFlash");
		StartCoroutine ("destroyAfterTime", 1.0f);
	}

	/**** Coroutines ****/ 
	IEnumerator destroyAfterTime(float time) {
		yield return new WaitForSeconds(time);

		Destroy (gameObject);
	}

	IEnumerator beginSpriteFlash() {
		while (true) {
			SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer> ();
			if (sprite.enabled) {
				sprite.enabled = false;
			} else {
				sprite.enabled = true;
			}

			yield return new WaitForSeconds (0.05f);
		}
	}
}
