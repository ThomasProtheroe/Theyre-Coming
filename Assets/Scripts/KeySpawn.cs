using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpawn : MonoBehaviour {

	public GameObject garageKey;

	public void spawnKey() {
		garageKey.transform.position = transform.position;
		garageKey.transform.parent = transform.parent;
		transform.parent.gameObject.GetComponent<Item> ().hiddenItem = garageKey;
	}
}
