using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giblet : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Physics2D.IgnoreCollision (GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>(), GetComponent<PolygonCollider2D> ());
	}
	
	public void startBloodTrail() {
		GetComponent<ParticleSystem> ().Play ();
	}
}
