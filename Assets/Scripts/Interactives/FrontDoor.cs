using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrontDoor : MonoBehaviour {

	public Sprite siegeSprite;

	[SerializeField]
	private Text textBox;
	[SerializeField]
	BoxCollider2D warningTrigger;
	[SerializeField]
	string warningText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setSiegeMode() {
		GetComponent<SpriteRenderer> ().sprite = siegeSprite;
		transform.position = new Vector3 (94.586f, -2.16f);
	}
}
