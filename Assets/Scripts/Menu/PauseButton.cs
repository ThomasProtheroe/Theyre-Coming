using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour, IPointerEnterHandler , IPointerExitHandler  {

	public Text buttonText;

	public void OnPointerEnter(PointerEventData eventData) {
		buttonText.color = Color.yellow;
	}

	public void OnPointerExit(PointerEventData eventData) {
		buttonText.color = Color.white;
	}
}
