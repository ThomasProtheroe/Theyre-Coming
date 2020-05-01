using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreParentRotation : MonoBehaviour {
	
	[SerializeField]
	private GameObject parent;

	[SerializeField]
	private float xOffset;
	[SerializeField]
	private float yOffset;

	void Start() {
		gameObject.transform.parent = null;
		Vector3 scale = gameObject.transform.localScale;
		if (scale.x < 0) {
			scale.x *= -1;
			gameObject.transform.localScale = scale;
		}
	}

	// Update is called once per frame
	void LateUpdate () {
		if (parent == null) {
			Destroy(gameObject);
			return;
		}

		transform.rotation = Quaternion.identity;

		Vector3 pos = new Vector3(parent.transform.position.x + xOffset, parent.transform.position.y + yOffset, parent.transform.position.z);
		transform.position = pos;
	}
}
