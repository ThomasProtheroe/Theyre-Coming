﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBottle : Weapon {

	public GameObject shards;

	void Start() {
		xOffset = -0.03f;
		yOffset = -0.05f;
		zRotation = 25.0f;
		restingHeight = -2.96f;
		restingRotation = 90;

		player = GameObject.FindGameObjectWithTag("Player");
		source = gameObject.GetComponent<AudioSource> ();
		description = description.Replace ("\\n", "\n");
	}

	public override bool onBreak() {
		GameObject newShards = Instantiate (shards, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
		newShards.layer = 11;

		Item item = newShards.GetComponent<Item> ();
		item.isBouncing = true;
		item.pickupCollider.enabled = false;
		item.hitCollider.enabled = true;

		Rigidbody2D rb = newShards.GetComponent<Rigidbody2D> ();
		rb.bodyType = RigidbodyType2D.Dynamic;
		rb.velocity = new Vector2 (0f,0.5f);

		return true;
	}

	public override void onTerrainImpact() {
		breakItem ();
	}
}