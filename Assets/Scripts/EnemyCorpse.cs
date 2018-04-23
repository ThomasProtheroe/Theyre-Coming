using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCorpse : MonoBehaviour {

	public float yPos;
	public float yPosVariance;

	[SerializeField]
	private SpriteRenderer corpseSprite;
	[SerializeField]
	private Sprite burntSprite;

	public void positionOnGround(float xPos) {
		float yOffset = UnityEngine.Random.Range (yPosVariance * -1, yPosVariance);
		float newYPos = yPos + yOffset;
		transform.position = new Vector3 (xPos, newYPos, 1.0f);

		if (newYPos > yPos) {
			corpseSprite.sortingOrder = 2;
		} else {
			corpseSprite.sortingOrder = 7;
		}
	}

	public void setBurntSprite() {
		corpseSprite.sprite = burntSprite;
	}
}
