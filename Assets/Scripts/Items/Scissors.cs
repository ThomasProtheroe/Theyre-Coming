using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scissors : Weapon {

	void Start() {
		xOffset = 0.3f;
		yOffset = 0.0f;
		zRotation = 0.0f;
		restingHeight = -2.95f;
		restingRotation = 0.0f;
	}
}
