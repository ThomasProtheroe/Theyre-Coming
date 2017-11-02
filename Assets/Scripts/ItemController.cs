using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour {

	public bool isThrown = false;
	public bool isBouncing = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (isBouncing) {
			Rigidbody2D itemBody = gameObject.GetComponent<Rigidbody2D> ();
			float xVel = itemBody.velocity.x;
			float yVel = itemBody.velocity.y;
			//Once it has come to rest
			if (Mathf.Approximately (xVel, 0.0f) && Mathf.Approximately (yVel, 0.0f)) {
				isBouncing = false;
				//Turn physics effects off for the item
				itemBody.bodyType = RigidbodyType2D.Kinematic;
				
				BoxCollider2D[] components = gameObject.GetComponents<BoxCollider2D> ();
				foreach(BoxCollider2D collider in components) {
					if (!collider.isTrigger) {
						Destroy (collider);
					}
				}
			}
		}
	}

	void OnCollisionEnter2D() {
		if (isThrown) {
			isThrown = false;
			isBouncing = true;
		}
	}

	public void flipItem() {
		SpriteRenderer itemSprite = gameObject.GetComponent<SpriteRenderer> ();
		itemSprite.flipX = !itemSprite.flipX;
	}
}
