using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public Transform enemy;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("t")) {
			spawnEnemy (6.0f);
		}
	}

	void spawnEnemy(float xPos) {
		Instantiate (enemy, new Vector3(xPos, enemy.transform.position.y, 0), Quaternion.identity);
	}
}
