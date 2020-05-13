using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsController : MonoBehaviour {

	private Animator anim;
	private PlayerController player;
	[SerializeField]
	private GameObject frontHand;

	private float readyTimer;
	[SerializeField]
	private float readyTime;

	private void Start() {
		anim = GetComponent<Animator> ();
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
	}

	private void Update() {
		if (readyTimer > 0.0f) {
			readyTimer -= Time.deltaTime;
			if (readyTimer <= 0.0f) {
				handsToIdle();
			}
		}
	}

	private void handsToIdle() {
		anim.SetBool ("Ready", false);
	}

	public void startHitWindow() {
		frontHand.GetComponent<Fist> ().startHitWindow();
	}

	public void endHitWindow() {
		frontHand.GetComponent<Fist> ().endHitWindow();
	}

	public void endPunch() {
		anim.SetBool ("Ready", true);
		readyTimer = readyTime;

		player.endPunch();
	}
}
