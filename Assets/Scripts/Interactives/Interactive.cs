using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive : MonoBehaviour
{
	protected Color negativeColor = new Color (0.825f, 0.0f, 0.0f);
	protected Color positiveColor = new Color(0.08f, 1.0f, 0.04f);

	public void enableHighlight() {
		GetComponent<SpriteOutline> ().enabled = true;
		StartCoroutine ("highlightGlow");
	}

	public void disableHighlight() {
		StopCoroutine ("highlightGlow");
		GetComponent<SpriteOutline> ().enabled = false;
	}

	public virtual void updateHighlightColor() {
		
	}

	IEnumerator highlightGlow() {
		SpriteOutline outline = GetComponent<SpriteOutline> ();
		//Fade out the highlight
		for (float f = 1f; f >= 0.3; f -= 0.01f) {
			Color c = outline.color;
			c.a = f;
			outline.color = c;

			yield return null;
		}
		//Fade the highlight back in
		for (float f = 0.3f; f <= 1; f += 0.01f) {
			Color c = outline.color;
			c.a = f;
			outline.color = c;

			yield return null;
		}
		Color orig = outline.color;
		orig.a = 1;
		outline.color = orig;


		StartCoroutine ("highlightGlow");
	}
}
