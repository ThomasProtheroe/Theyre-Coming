using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpawn : MonoBehaviour {

	public GameObject garageKey;

	public void spawnKey(Transform position) {
		garageKey.transform.position = position.position;
	}
}
