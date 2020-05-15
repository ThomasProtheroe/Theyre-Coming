using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HighlightOnSelect : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		gameObject.GetComponentsInChildren <Selectable> ();
	}
}
