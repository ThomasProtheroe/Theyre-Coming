using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrontDoor : MonoBehaviour {

	public Sprite siegeSprite;
	[SerializeField]
	private Text textBox;
	[SerializeField]
	public BoxCollider2D warningTrigger;
	private string warningString;
	[SerializeField]
	private Sprite playerPortrait;
	private GameController gc;

	private bool warningPlayed;

	void Start() {
		gc = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		warningString = "\nThe streets outside are crawling with those things. I won't last 2 minutes out there...";
		Invoke ("enableWarning", 5.0f);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (!warningPlayed && other.tag == "Player") {
			Dialog warningDialog = new Dialog (warningString, playerPortrait, 5.0f);
			gc.showDialog (warningDialog);
			warningPlayed = true;
		}
	}

	public void setSiegeMode() {
		GetComponent<SpriteRenderer> ().sprite = siegeSprite;
		transform.position = new Vector3 (94.586f, -2.16f);
	}

	private void enableWarning() {
		warningTrigger.enabled = true;
	}
}
