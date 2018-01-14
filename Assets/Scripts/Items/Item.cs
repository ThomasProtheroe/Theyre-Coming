using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	public bool isHeld = false;
	public bool isThrown = false;
	public bool isBouncing = false;
	public bool isAttacking = false;

	public float xOffset;
	public float yOffset;
	public float zRotation;
	public float restingHeight;
	public float restingRotation;
	public bool flipped = false;

	public int thrownDamage;

	public string type;

	public BoxCollider2D pickupCollider;
	public Collider2D hitCollider;

	public GameObject frontHand;
	public GameObject backHand;

	protected AudioSource source;
	public AudioClip throwSound;
	public AudioClip throwImpact;
	public AudioClip craftSound;
	public AudioClip pickupSound;
	public AudioClip swapSound;

	public GameObject player;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		source = gameObject.GetComponent<AudioSource> ();
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

				pickupCollider.enabled = true;
				hitCollider.enabled = false;
				gameObject.layer = 13;
			}
		}
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (isThrown) {
			if (other.gameObject.tag == "Enemy") {
				if (throwImpact) {
					source.PlayOneShot (throwImpact);
				}
				other.gameObject.GetComponent<Enemy> ().takeDamage (thrownDamage);
			}

			isThrown = false;
			isBouncing = true;
			gameObject.layer = 11;
		}
	}

	public void flipItem() {
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
		flipped = !flipped;
	}

	public void pickupItem(bool playerFlipX) {
		if (pickupSound && source) {
			source.PlayOneShot (pickupSound);
		}
			
		if (playerFlipX != flipped)
		{
			flipItem ();
		}
			
		gameObject.layer = 16;

		//disable pickup trigger, enable hit trigger
		pickupCollider.enabled = false;
		hitCollider.enabled = true;

		frontHand.SetActive (true);
		backHand.SetActive (true);

		isHeld = true;
	}

	public void dropItem() {
		transform.parent = null;
		pickupCollider.enabled = true;
		hitCollider.enabled = false;
		isHeld = false;

		frontHand.SetActive (false);
		backHand.SetActive (false);

		moveToResting ();
	}

	public void moveToResting() {
		transform.position = new Vector2 (transform.position.x, restingHeight);
		transform.eulerAngles = new Vector3 (0, 0, restingRotation);
	}

	public void playCraftingSound() {
		//Since this object was just created, the source variable is not initialized yet
		//so we need to get the AudioSource directly
		if (craftSound) {
			gameObject.GetComponent<AudioSource>().PlayOneShot (craftSound);
		}
	}

	public void playSwappingSound() {
		if (craftSound) {
			source.PlayOneShot (swapSound);
		}
	}

	public void playThrowSound() {
		if (throwSound) {
			source.PlayOneShot (throwSound);
		}
	}

	public virtual void use() {

	}
}
