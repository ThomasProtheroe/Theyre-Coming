using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog {

	public string text;
	public string character;
	public Sprite[] sprites;
	public float duration;
	public float delay;

	public Dialog(string text, string character="fiona", float duration=4.0f, float delay=0.0f) {
		this.character = character;
		this.text = text;
		this.duration = duration;
		this.delay = delay;
	}
}
