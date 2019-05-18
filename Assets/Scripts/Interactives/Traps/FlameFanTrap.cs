using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameFanTrap : FanTrap {

	[Header("Projectile Attributes")]
	[SerializeField]
	private FireGlob fireGlob;
	[SerializeField]
	private ParticleSystem flamePS;
	[SerializeField]
	private ParticleSystem[] burningPSs;
	[SerializeField]
	private GameObject explosion;
	[SerializeField]
	private Animator rotationAnimator;
	[SerializeField]
	private float projectileInterval;
	private float projectileTimer = 0.0f;
	[SerializeField]
	private float degredationInterval;
	private float degredationTimer = 0.0f;
	[SerializeField]
	private float projectileSpeed;
	[SerializeField]
	private int projectileDamage;

	protected override void Start() {
		burningPSs = GetComponentsInChildren<ParticleSystem> ();

		base.Start ();
	}

	// Update is called once per frame
	protected override void Update () {
		if (isDeployed && durability > 0) {
			projectileTimer -= Time.deltaTime;
			if (projectileTimer <= 0.0f) {
				fireProjectile ();
				projectileTimer = projectileInterval;
			}

			degredationTimer -= Time.deltaTime;
			if (degredationTimer <= 0.0f) {
				reduceDurability (false);
				degredationTimer = degredationInterval;
			}
		}

		base.Update ();
	}

	public override void deploy() {
		rotationAnimator.enabled = true;
		base.deploy ();
	}

	void fireProjectile() {
		float speed = projectileSpeed + Random.Range(-1, 2);
		float angle = Random.Range (120.0f, 240.0f) + Random.Range(0, 2) * 180.0f;
		float radAngle = angle * Mathf.Deg2Rad;
		Vector2 direction = new Vector2 (Mathf.Cos (radAngle), Mathf.Sin (radAngle));

		FireGlob newGlob = Instantiate (fireGlob, transform.position, Quaternion.identity);
		newGlob.friendlyFire = false;
		newGlob.lifetime = 0.0f;
		newGlob.activeDamage = projectileDamage;

		newGlob.GetComponent<Rigidbody2D> ().velocity = direction * speed;
		newGlob.transform.eulerAngles = new Vector3 (0.0f, 0.0f, angle - 90.0f);
	}

	private void explode() {
		Instantiate (explosion, transform.position, Quaternion.identity);

		Destroy (gameObject);
	}

	protected override void reduceDurability(bool enemyHit=true) {
		foreach (ParticleSystem ps in burningPSs) {
			if (durability < maxDurability * 0.25f) {
				ps.Play ();
			}
		}

		base.reduceDurability (enemyHit);
	}

	public override bool onBreak() {
		explode ();

		return false;
	}
}