using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Item {

	public int durability;
	protected int maxDurability;
	public bool friendlyFire;

	public float deployY;
	public float deployRotation;
	public EnhancedAudioClip deploySound;

	protected bool isDeployed;
	protected string deployedArea;

	public Collider2D triggerCollider;

	protected override void Start() {
		usable = true;
		maxDurability = durability;

		base.Start ();
	}

	override public void use() {
		deploy ();
	}

	public virtual void deploy() {
		playTrapDeploySound();
	}

	public virtual void trigger(GameObject other=null) {
		                                                                
	}

	public void playTrapDeploySound() {
		soundController.playPriorityOneShot (deploySound);
	}
}
