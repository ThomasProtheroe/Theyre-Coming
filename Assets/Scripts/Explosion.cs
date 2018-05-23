using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

	[SerializeField]
	private Animator anim;
	[SerializeField]
	private AudioClip explosionSound;

	// Use this for initialization
	void Start () {
		GameObject.FindGameObjectWithTag ("SoundController").GetComponent<SoundController> ().playPriorityOneShot(explosionSound);
	}
	
	protected void destroyOnFinish() {
		Destroy (gameObject);
	}
}
