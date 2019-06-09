using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorativeSprite : MonoBehaviour {
	SpriteRenderer sprite;

	void Start() {
		sprite = GetComponent<SpriteRenderer> ();
	}

	public void breakSprite() {
		transform.parent = null;
		gameObject.layer = 11;

		Rigidbody2D body = GetComponent<Rigidbody2D>();
		body.bodyType = RigidbodyType2D.Dynamic;

		StartCoroutine ("beginSpriteFlash");
		GetComponent<DestroyAfterTime> ().StartCoroutine ("destroyAfterTime", 1.0f);
	}

	IEnumerator beginSpriteFlash() {
		while (true) {
			if (sprite.enabled) {
				sprite.enabled = false;
			} else {
				sprite.enabled = true;
			}

			yield return new WaitForSeconds (0.05f);
		}
	}
}
