﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : MonoBehaviour {

	public Transition sibling;

	private Animator anim;
	private GameObject player;
	private GameObject camera;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		camera = GameObject.FindGameObjectWithTag("MainCamera");
		anim = gameObject.GetComponent<Animator> ();
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void playerTravel() {
		StartCoroutine ("movePlayer");
	}

	IEnumerator movePlayer() {
		PlayerController playerCon = player.GetComponent<PlayerController> ();
		SpriteRenderer sprite = player.GetComponent<SpriteRenderer> ();
		SpriteRenderer itemSprite = null;
		Color originalItemColor = new Color(255f, 255f, 255f, 255f);
		if (playerCon.heldItem) {
			itemSprite = playerCon.heldItem.GetComponent<SpriteRenderer> ();
			originalItemColor = itemSprite.material.color;
		}
		anim.SetTrigger ("Open");

		Color originalPlayerColor = sprite.material.color;

		for (float f = 1f; f >= 0; f -= 0.03f) {
			Color c = sprite.material.color;
			c.a = f;
			sprite.material.color = c;

			if (playerCon.heldItem) {
				Color itemC = itemSprite.material.color;
				itemC.a = f;
				itemSprite.material.color = c;
			}

			yield return null;
		}

		yield return new WaitForSeconds (0.3f);
			
		Vector2 destination = new Vector2(sibling.transform.position.x, player.transform.position.y);
		player.transform.position = destination;

		sprite.material.color = originalPlayerColor;
		itemSprite.material.color = originalItemColor;

		camera.transform.position = new Vector3 (player.transform.position.x, camera.transform.position.y, camera.transform.position.z);

		playerCon.isBusy = false;
	}
}
