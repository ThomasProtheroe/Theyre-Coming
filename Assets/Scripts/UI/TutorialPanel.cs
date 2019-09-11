using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : MonoBehaviour {

	[SerializeField]
	private bool viewed;
	[SerializeField]
	private SpriteRenderer sprite;
	[SerializeField]
	private GameObject text;
	[SerializeField]
	private float lifetime;
	public GameObject parentItem;
	private GameController gameController;

	// Use this for initialization
	void Start () {
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController> ();
		if (gameController.isTutorialActive() == false) {
			destroyTutorialPanel();
		}
	}
	
	public void showTutorialPanel () {
		//Destroy previously viewed tutorial panels
		if (parentItem.GetComponent<PlayerController> () == null) {
			foreach(GameObject tutorialObject in GameObject.FindGameObjectsWithTag("Tutorial")) {
				
				if (tutorialObject.transform.parent.gameObject == transform.parent.gameObject) {
					continue;
				}
				if (tutorialObject.GetComponent<TutorialPanel> ().viewed) {
					Destroy(tutorialObject);
				}

				if (tutorialObject.GetComponent<TutorialPanel> ().parentItem.GetComponent<PlayerController> () != null) {
					continue;
				}

				if (tutorialObject.GetComponent<TutorialPanel> ().parentItem.GetComponent<Item> ().type == parentItem.GetComponent<Item> ().type) {
					Destroy(tutorialObject);
				}
			}
		}
		
		if (lifetime > 0) {
			GetComponent<DestroyAfterTime> ().StartCoroutine ("destroyAfterTime", lifetime);
		}

		viewed = true;
		text.SetActive(true);
		sprite.enabled = true;
	}

	public void hideTutorialPanel() {
		text.SetActive(false);
		sprite.enabled = false;
	}

	public void destroyTutorialPanel() {
		Destroy(gameObject);
	}

	public void flipPanel() {
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}
}
