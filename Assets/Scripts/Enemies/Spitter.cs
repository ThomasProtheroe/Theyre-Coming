using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spitter : Enemy {
	private bool isSpitting;
	private bool isRecovering;

	[Header("Spit Attack")]
	[SerializeField]
	private ParticleSystem droolPS;
	[SerializeField]
	private ParticleSystem spitPS;
	[SerializeField]
	private BileProjectile projectile;
	[SerializeField]
	private float projectileSpeed;

	//Behaviour control
	[Header("Behaviour Parameters")]
	[SerializeField]
	private float attackAggroRange;
	[SerializeField]
	private float prepRange;
	[SerializeField]
	private float prepDelay;
    
	
}
	