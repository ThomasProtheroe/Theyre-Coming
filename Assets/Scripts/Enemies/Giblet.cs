using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giblet : MonoBehaviour {

	[SerializeField]
	private EnhancedAudioClip gibletImpact;

	// Use this for initialization
	void Start () {
		Physics2D.IgnoreCollision (GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>(), GetComponent<PolygonCollider2D> ());
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "AreaWall" || other.gameObject.tag == "Terrain") {
			GameObject.FindGameObjectWithTag ("SoundController").GetComponent<SoundController> ().playPriorityOneShot(gibletImpact);
		}
	}

	public void startBloodTrail() {
		GetComponent<ParticleSystem> ().Play ();
	}
}
