using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedTransition : Transition {
	public bool isLocked;
	[SerializeField]
	private Dialog lockedDialog;

	// Use this for initialization
	void Start () {
		isLocked = true;
	}

	public void unlock() {
		isLocked = false;
	}

	public void displayLockedDialog() {
		gc.showDialog (lockedDialog);
	}

	override public void updateHighlightColor() {
		if (inUse || isLocked) {
			GetComponent<SpriteOutline> ().color = negativeColor;
		} else {
			GetComponent<SpriteOutline> ().color = positiveColor;
		}
	}
}
