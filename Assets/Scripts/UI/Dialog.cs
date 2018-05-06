using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog {

	public string text;
	public Sprite[] sprites;
	public float duration;
	public float delay;

	public Dialog(string text, Sprite[] sprites=null, float duration=4.0f, float delay=0.0f) {
		this.sprites = sprites;
		this.text = text;
		this.duration = duration;
		this.delay = delay;
	}
}
