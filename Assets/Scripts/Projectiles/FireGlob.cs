using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireGlob : MonoBehaviour {

	[SerializeField]
	private int damage;
	[SerializeField]
	private float lifetime;

	// Use this for initialization
	void Start () {
		
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Enemy") {
			other.gameObject.GetComponent<Enemy> ().takeFireHit(damage);
		} else if (other.gameObject.tag == "Terrain") {
			Rigidbody2D rb = GetComponent<Rigidbody2D> ();
			rb.bodyType = RigidbodyType2D.Kinematic;
			rb.velocity = Vector2.zero;
		}
	}
}
