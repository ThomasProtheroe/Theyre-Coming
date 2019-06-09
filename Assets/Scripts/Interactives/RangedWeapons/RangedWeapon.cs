using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Item {

	public int attackDamage;
	public int capacity;
	public int knockback;
	public int multiHit = 1;

	protected int ammunition;
	[SerializeField]
	private GameObject emptyPrefab;
	[SerializeField]
	protected AudioClip fireSound;

	private new void Start () {
		ammunition = capacity;
		usable = true;
		base.Start ();
	}

	public override void updateDurabilityIndicator() {
		playerCon.activeSlot.setDurabilityIndicator(((float)ammunition / capacity));
	}

	public void setEmpty() {
		GameObject newWeapon = Instantiate(emptyPrefab, new Vector3(transform.position.x, transform.position.y), transform.rotation);
		transform.parent = null;
		playerCon.heldItem = newWeapon;
		newWeapon.transform.parent = playerCon.heldItemParent.transform;
		newWeapon.GetComponent<Item> ().pickupItem (playerCon.playerSprite.flipX, false);

		playerCon.positionHeldItem ();

		playerCon.activeSlot.setImage(newWeapon.GetComponent<SpriteRenderer>().sprite);

		Destroy (gameObject);
	}

	override public void use() {
		fire ();
	}

	public virtual void fire() {
		updateDurabilityIndicator();
	}

	public void stopAttacking() {
		isAttacking = false;
		Debug.Log ("Stop Attacking");
	}
}
