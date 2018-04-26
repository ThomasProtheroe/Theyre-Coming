using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public float moveThreshold;
	public float cameraSpeed;
	public float backgroundSpeedDivisor = 10.0f;
	private float xDiff;
	private GameObject player;
	private Vector3 targetPos;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		transform.position = new Vector3 (player.transform.position.x, transform.position.y, transform.position.z);
	}
	
	// Update is called once per frame
	void LateUpdate () {
		float backgroundSpeed = (cameraSpeed * Time.deltaTime) / backgroundSpeedDivisor;

		if (player.transform.position.x > gameObject.transform.position.x) {
			xDiff = player.transform.position.x - gameObject.transform.position.x;
			backgroundSpeed *= -1;
		} else {
			xDiff = gameObject.transform.position.x - player.transform.position.x;
		}

		if (xDiff >= moveThreshold) {
			targetPos = player.transform.position;
			targetPos.y = gameObject.transform.position.y;
			targetPos.z = gameObject.transform.position.z;
			gameObject.transform.position = Vector3.MoveTowards (gameObject.transform.position, targetPos, cameraSpeed * Time.deltaTime);

			//Move parallax background

			SpriteRenderer backgroundSprite = gameObject.GetComponentInChildren<SpriteRenderer> ();
			backgroundSprite.transform.localPosition = new Vector3 (backgroundSprite.transform.localPosition.x + backgroundSpeed, backgroundSprite.transform.localPosition.y, backgroundSprite.transform.localPosition.z);
		}
	}
}
