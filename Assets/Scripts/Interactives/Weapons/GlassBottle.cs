using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBottle : Weapon {

	public GameObject shards;

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

		soundController.playPriorityOneShot(item.breakSound);

		return true;
	}

	public override void onTerrainImpact() {
		breakItem ();
	}
}
