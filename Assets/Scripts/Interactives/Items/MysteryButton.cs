using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryButton : Item {

	[SerializeField]
	private LabEntrance labEntrance;

	[Header("Button Sounds")]
	[SerializeField]
	private AudioClip useSound;
	[SerializeField]
	private AudioClip successSound;
	[SerializeField]
	private AudioClip failSound;

	[Header("Sprites")]
	[SerializeField]
	private Sprite successSprite;
	[SerializeField]
	private Sprite failSpriteOff;
	[SerializeField]
	private Sprite failSpriteOn;

	[SerializeField]
	private float useCooldown;
	private float useTimer;

	protected override void Start() {
		usable = true;
		useTimer = 0.0f;

		//Find the lab entrance transition
		foreach (GameObject transitionObj in GameObject.FindGameObjectsWithTag ("Transition")) {
			LabEntrance transition = transitionObj.GetComponent<LabEntrance> ();
			if (transition != null) {
				labEntrance = transition;
			}
		}
			
		base.Start ();
	}

	protected override void Update() {
		if (useTimer > 0) {
			useTimer -= Time.deltaTime;
		}

		base.Update ();
	}

	public override void use ()
	{
		if (!usable || useTimer > 0.0f) {
			return;
		}

		useTimer = useCooldown;

		playUseSound ();
		if (playerCon.getCurrentArea () == labEntrance.gameObject.transform.parent.gameObject.GetComponent<Area> ()) {
			Invoke ("onUseSuccess", 1.0f);
			usable = false;
		} else {
			Invoke ("onUseFail", 1.0f);
		}
	}

	public void drop() {
		Rigidbody2D body = GetComponent<Rigidbody2D>();

		pickupCollider.enabled = false;
		hitCollider.enabled = true;
		body.bodyType = RigidbodyType2D.Dynamic;
		isThrown = true;
		gameObject.layer = 12;
	}

	private void onUseSuccess() {
		playSuccessSound ();
		sprite.sprite = successSprite;
		Invoke ("revealLab", 1.0f);
	}

	private void onUseFail() {
		playFailSound ();
		sprite.sprite = failSpriteOff;
		flashFailLight ();
	}

	private void revealLab() {
		labEntrance.reveal ();
	}

	private void flashFailLight() {
		showFailOn ();
		Invoke ("showFailOff", 0.4f);
	}

	private void showFailOn() {
		sprite.sprite = failSpriteOn;
	}

	private void showFailOff() {
		sprite.sprite = failSpriteOff;
	}

	private void playUseSound() {
		soundController.playPriorityOneShot (useSound);
	}

	private void playSuccessSound() {
		soundController.playPriorityOneShot (successSound);
	}

	private void playFailSound() {
		soundController.playPriorityOneShot (failSound);
	}
}
