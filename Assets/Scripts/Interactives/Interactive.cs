using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive : MonoBehaviour
{
	protected Color negativeColor = new Color (0.825f, 0.0f, 0.0f);
	protected Color positiveColor = new Color(0.08f, 1.0f, 0.04f);

	public virtual void enableHighlight() {
		
	}

	public virtual void disableHighlight() {

	}

	public virtual void updateHighlightColor() {
		
	}
}
