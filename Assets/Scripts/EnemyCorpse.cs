using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCorpse : MonoBehaviour {

	public float yPos;
	public float yPosVariance;

	[SerializeField]
	private SpriteRenderer sprite;
	[SerializeField]
	private Sprite[] genericSprites;
	[SerializeField]
	private Sprite[] burntSprites;

	public void positionOnGround(float xPos) {
		float yOffset = UnityEngine.Random.Range (yPosVariance * -1, yPosVariance);
		float newYPos = yPos + yOffset;
		transform.position = new Vector3 (xPos, newYPos, 1.0f);

		if (newYPos > yPos) {
			sprite.sortingOrder = 1;
		} else {
			sprite.sortingOrder = 10;
		}
	}

	public void setBurntSprite() {
		sprite.sprite = genericSprites[UnityEngine.Random.Range(0, genericSprites.Length)];
	}

	public void setGenericSprite() {
		sprite.sprite = genericSprites[UnityEngine.Random.Range(0, burntSprites.Length)];
	}
}
