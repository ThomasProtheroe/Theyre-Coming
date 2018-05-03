using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog {

	public string text;
	public Sprite sprite;
	public float duration;
	public float delay;

	public Dialog(string text, Sprite sprite=null, float duration=4.0f, float delay=0.0f) {
		this.sprite = sprite;
		this.text = text;
		this.duration = duration;
		this.delay = delay;
	}
}
