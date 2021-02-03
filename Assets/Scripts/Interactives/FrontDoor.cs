using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrontDoor : Interactive {

	public Sprite siegeSprite;
	[SerializeField]
	private Text textBox;
	[SerializeField]
	public BoxCollider2D warningTrigger;
	private string warningString;
	[SerializeField]
	private Sprite playerPortrait;
	private GameController gameCon;

	private bool warningPlayed;

	[Header("Door Shake Parameters")]
	[SerializeField]
	private float shakeDuration;
	[SerializeField]
	private float shakeAmount;
	private Vector3 originalPosition;
	private float shakeTimer;

	void Start() {
		originalPosition = transform.localPosition;
		gameCon = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		warningString = "The streets outside are crawling with those things. I won't last 2 minutes out there...";
		Invoke ("enableWarning", 5.0f);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (!warningPlayed && other.tag == "Player") {
			Dialog warningDialog = new Dialog (warningString, null, 5.0f);
			gameCon.showDialog (warningDialog);
			warningPlayed = true;
		}
	}

	public void interact() {
		OpenScavenge ();
	}

	private void OpenScavenge() {
	}
	override public void updateHighlightColor() {
		string phase = gameCon.getPhase();
		if (phase == "downtime") {
			GetComponent<SpriteOutline> ().color = positiveColor;
		} else {
			GetComponent<SpriteOutline> ().color = negativeColor;
		}
	}
	public void shakeDoor() {
		StartCoroutine ("ShakeDoor");
	}

	public void setSiegeMode() {
		GetComponent<SpriteRenderer> ().sprite = siegeSprite;
		transform.position = new Vector3 (94.586f, -2.16f);
	}

	private void enableWarning() {
		warningTrigger.enabled = true;
	}

	IEnumerator ShakeDoor() {
		shakeTimer = shakeDuration;
		while (shakeTimer > 0f)
		{
			transform.localPosition = originalPosition + Random.insideUnitSphere * shakeAmount;

			shakeTimer -= Time.deltaTime;

			if (shakeTimer <= 0f) {
				shakeTimer = 0f;
				transform.localPosition = originalPosition;
			}
			yield return null;
		}
	}
}
