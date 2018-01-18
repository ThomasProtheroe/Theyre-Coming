using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : MonoBehaviour {

	public Transition sibling;
	public AudioClip openSound;
	public AudioClip closeSound;

	private Animator anim;
	private GameObject player;
	private GameObject camera;
	private AudioSource source;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		camera = GameObject.FindGameObjectWithTag("MainCamera");
		anim = gameObject.GetComponent<Animator> ();
		source = gameObject.GetComponent<AudioSource> ();
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void playerTravel() {
		StartCoroutine ("movePlayer");
	}

	public void enemyTravel(Enemy enemy) {
		StartCoroutine ("moveEnemy", enemy);
	}

	private void playOpenSound() {
		if (openSound) {
			source.clip = openSound;
			source.Play ();
		}
	}

	private void playCloseSound() {
		if (closeSound) {
			source.clip = closeSound;
			source.Play ();
		}
	}

	IEnumerator movePlayer() {
		PlayerController playerCon = player.GetComponent<PlayerController> ();
		SpriteRenderer sprite = player.GetComponent<SpriteRenderer> ();
		SpriteRenderer itemSprite = null;
		SpriteRenderer[] handsSprites = null;
		Color originalItemColor = new Color(255f, 255f, 255f, 255f);

		playOpenSound ();

		if (playerCon.heldItem) {
			itemSprite = playerCon.heldItem.GetComponent<SpriteRenderer> ();
			originalItemColor = itemSprite.material.color;

			Item itemCon = playerCon.heldItem.GetComponent<Item> ();
			handsSprites = new SpriteRenderer[] {
				itemCon.frontHand.GetComponent<SpriteRenderer> (),
				itemCon.backHand.GetComponent<SpriteRenderer> ()
			};
		} else {
			handsSprites = playerCon.handsParent.GetComponentsInChildren<SpriteRenderer> ();
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

			Color handC = handsSprites [0].material.color;
			foreach (SpriteRenderer hand in handsSprites) {
				handC.a = f;
				hand.material.color = c;
			}

			yield return null;
		}

		yield return new WaitForSeconds (0.35f);

		playCloseSound ();
			
		Vector2 destination = new Vector2(sibling.transform.position.x, player.transform.position.y);
		player.transform.position = destination;

		playerCon.setCurrentArea (sibling.gameObject.transform.parent.gameObject.GetComponent<Area>());

		sprite.material.color = originalPlayerColor;
		foreach (SpriteRenderer hand in handsSprites) {
			hand.material.color = originalPlayerColor;
		}
		if (itemSprite) {
			itemSprite.material.color = originalItemColor;
		}

		camera.transform.position = new Vector3 (player.transform.position.x, camera.transform.position.y, camera.transform.position.z);
		GameObject parallax = camera.transform.GetChild (0).gameObject;
		parallax.transform.localPosition = new Vector3 (0.0f, -1.5f, 10.0f);

		playerCon.isBusy = false;
		playerCon.isInvulnerable = false;
	}

	IEnumerator moveEnemy(Enemy enemy) {
		enemy.deactivate ();
		SpriteRenderer sprite = enemy.gameObject.GetComponent<SpriteRenderer> ();

		anim.SetTrigger ("Open");
		sibling.GetComponent<Animator> ().SetTrigger ("Open");

		Color originalEnemyColor = sprite.material.color;

		for (float f = 1f; f >= 0; f -= 0.03f) {
			Color c = sprite.material.color;
			c.a = f;
			sprite.material.color = c;

			yield return null;
		}

		yield return new WaitForSeconds (0.35f);

		Vector2 destination = new Vector2(sibling.transform.position.x, player.transform.position.y);
		enemy.gameObject.transform.position = destination;
		enemy.setCurrentArea (sibling.transform.parent.gameObject.GetComponent<Area> ());

		for (float f = 0f; f <= 1; f += 0.03f) {
			Color c = sprite.material.color;
			c.a = f;
			sprite.material.color = c;

			yield return null;
		}

		enemy.activate ();
	}
}
