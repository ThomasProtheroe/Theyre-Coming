using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog {

	public string text;
	public Sprite sprite;
	public float duration;

	public Dialog(string text, Sprite sprite, float duration=4.0f) {
		this.sprite = sprite;
		this.text = text;
		this.duration = duration;
	}
}
