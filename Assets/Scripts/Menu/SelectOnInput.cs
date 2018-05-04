using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectOnInput : MonoBehaviour {

	public EventSystem eventSystem;
	public GameObject selectedObject;

	private bool buttonSelected;

	// Update is called once per frame
	void Update () {
		if (Input.GetAxisRaw ("Vertical") != 0 && buttonSelected == false) {
			eventSystem.SetSelectedGameObject (selectedObject);
			buttonSelected = true;
		}
	}

	public void OnDisable() {
		buttonSelected = false;
	}

	public void disableNavigation() {
		Button[] buttons = GetComponentsInChildren<Button> ();
		foreach (Button button in buttons) {
			button.interactable = false;
		}
	}
}
