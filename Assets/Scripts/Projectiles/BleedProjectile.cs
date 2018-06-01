using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedProjectile : BaseProjectile {

	public int damage;
	public float lifetime;
	private List<Enemy> enemiesHit;

	public override void Start() {
		enemiesHit = new List<Enemy> ();

		base.Start ();
	}

	// Update is called once per frame
	void Update () {
		if (!isActive) {
			return;
		}
		
		lifetime -= Time.deltaTime;
		if (lifetime <= 0) {
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (!isActive) {
			return;
		}

		if (other.gameObject.tag == "Enemy") {
			Enemy enemy = other.gameObject.GetComponent<Enemy> ();
			if (enemiesHit.Contains (enemy)) {
				return;
			}

			if (!enemy.isInvunlerable && !enemy.getIsDead ()) {
				float direction = player.transform.position.x - enemy.transform.position.x;
				enemy.takeHit (damage, knockback, direction, false, Constants.ATTACK_TYPE_PROJECTILE);
				Debug.Log ("enemy hit");
				enemy.setBleeding ();
				enemiesHit.Add (enemy);
			}
		} else if (other.gameObject.tag == "AreaWall") {
			Destroy (gameObject);
		}
	}
}
