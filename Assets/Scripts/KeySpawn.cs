using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpawn : MonoBehaviour {

	public GarageKey keyPrefab;

	public void spawnKey() {
		GarageKey newKey =  Instantiate (keyPrefab, transform.position, Quaternion.identity);
	}
}
