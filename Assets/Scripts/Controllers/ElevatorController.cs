using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour {
	private GameObject player;
	private GameObject mainCamera;
	[SerializeField]
	private GameController gameCon;

	public List<GameObject> elevatorSegments = new List<GameObject> ();
	public GameObject elevatorSegmentPrefab;
	public Transition destination;

	public bool elevatorRunning;
	public float elevatorSpeed;
	private float elevatorTimer = 0.0f;
	public float travelTime;
	private bool isFading;

	void Start() {
		player = GameObject.FindGameObjectWithTag("Player");
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

	}
	
	// Update is called once per frame
	void Update () {
		if (elevatorRunning) {
			float speed = elevatorSpeed * Time.deltaTime;
			for(int i = 0; i < elevatorSegments.Count ; i++) {
				GameObject segment = elevatorSegments [i];
				if (segment == null) {
					continue;
				}
				float newY = segment.transform.position.y + speed;
				Vector3 pos = new Vector3 (segment.transform.position.x, newY, segment.transform.position.z);
				segment.transform.position = pos;

				//Wrap around so we can descend FOREVER!!!
				if (segment.transform.position.y > 5.0f) {
					GameObject newSeg = Instantiate (elevatorSegmentPrefab, new Vector3(segment.transform.position.x, -10.82f, 0), Quaternion.identity);
					elevatorSegments.Add(newSeg);
					elevatorSegments.Remove (segment);

					Destroy (segment);
				}
			}

			elevatorTimer += Time.deltaTime;
			if (elevatorTimer >= travelTime - 2.0f && !isFading) {
				gameCon.miscFadeOut ();
				isFading = true;
			}

			if (elevatorTimer >= travelTime) {
				exitElevator ();
			}
		}
	}

	private void exitElevator() {
		gameCon.miscFadeIn ();

		Vector2 destinationLoc = new Vector2(destination.transform.position.x, player.transform.position.y);
		player.transform.position = destinationLoc;
		mainCamera.transform.position = new Vector3 (player.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z);

		destination.isLocked = true;
		elevatorRunning = false;
		isFading = false;

		returnPlayerControl ();
	}

	public void returnPlayerControl() {
		PlayerController playerCon = player.GetComponent<PlayerController> ();
		playerCon.enableCinematicControl (false);

		playerCon.itemSlot1.gameObject.SetActive(true);
		playerCon.itemSlot2.gameObject.SetActive(true);
	}
}
