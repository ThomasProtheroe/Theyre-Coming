using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseballBat : Weapon {

	void Start() {
		xOffset = 0.08f;
		yOffset = -0.2f;
		zRotation = 20.0f;
		restingHeight = -2.9f;
		restingRotation = 90;
	}

}
