using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Item {

	public int attackDamage;
	public int capacity;
	public int knockback;
	public int multiHit = 1;

	private int ammunition;

	override public void use() {
		fire ();
	}

	public virtual void fire() {

	}
}
