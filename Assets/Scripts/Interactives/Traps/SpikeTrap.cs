using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : Trap {

	public int damage;
	public AudioClip hitSound;

	override public void trigger(GameObject victim) {
		victim.GetComponent<Enemy> ().takeHit (damage, 0);
		source.PlayOneShot (hitSound);
		//Apply status effect here

		durability--;
		if (durability <= 0) {
			breakItem ();
		} else {
			if ((state == 0) && (bloodySprite1 != null)) {
				GetComponent<SpriteRenderer> ().sprite = bloodySprite1;
				state++;
			} else if ((state == 1) && (bloodySprite2 != null)) {
				GetComponent<SpriteRenderer> ().sprite = bloodySprite2;
				state++;
			} else if ((state == 2) && (bloodySprite3 != null)) {
				GetComponent<SpriteRenderer> ().sprite = bloodySprite3;
				state++;
			}
		}
	}
	 
}
