using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFanTrap : FanTrap {

	[Header("Projectile Attributes")]
	[SerializeField]
	private BaseProjectile projectile;
	[SerializeField]
	private float projectileInterval;
	private float projectileTimer = 0.0f;
	[SerializeField]
	private float degredationInterval;
	private float degredationTimer = 0.0f;
	[SerializeField]
	private float projectileSpeed;
	
	// Update is called once per frame
	protected override void Update () {
		if (isDeployed) {
			projectileTimer -= Time.deltaTime;
			if (projectileTimer <= 0.0f) {
				fireProjectile ();
				projectileTimer = projectileInterval;
			}

			degredationTimer -= Time.deltaTime;
			if (degredationTimer <= 0.0f) {
				reduceDurability ();
				degredationTimer = degredationInterval;
			}
		}
			
		base.Update ();
	}

	void fireProjectile() {
		float speed = projectileSpeed;

		float angle = Random.Range (130.0f, 230.0f) + Random.Range(0, 2) * 180.0f;
		float radAngle = angle * Mathf.Deg2Rad;
		Vector2 direction = new Vector2 (Mathf.Cos (radAngle), Mathf.Sin (radAngle));

		BaseProjectile newProjectile = Instantiate (projectile, transform.position, Quaternion.identity);
		newProjectile.GetComponent<Rigidbody2D> ().velocity = direction * speed;
		newProjectile.transform.eulerAngles = new Vector3 (0.0f, 0.0f, angle - 90.0f);

		newProjectile.fire ();
	}
}
