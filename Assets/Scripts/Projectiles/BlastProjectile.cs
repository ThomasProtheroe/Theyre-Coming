using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastProjectile : BaseProjectile {

	[SerializeField]
	private ParticleSystem fusePS;
	[SerializeField]
	private ParticleSystem explosionPS;
	[SerializeField]
	private GameObject explosion;

	void OnTriggerEnter2D(Collider2D other) {
		if (!isActive) {
			return;
		}

		if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "AreaWall") {
			explode ();
		}
	}

	public override void fire() {
		fusePS.Play ();

		base.fire ();
	}

	protected override bool hitTarget(Enemy target) {
		if (!target.getIsDead ()) {
			explode ();
			return false;
		}

		return true;
	}

	private void explode() {
		Instantiate (explosion, transform.position, Quaternion.identity);

		//Raycast to get all enemies/players, deal damage to them
		var list = new List<RaycastHit2D> ();
		list.AddRange (Physics2D.RaycastAll(transform.position, Vector2.right, 10f, 1 << LayerMask.NameToLayer("Enemy")));
		list.AddRange (Physics2D.RaycastAll(transform.position, Vector2.left, 10f, 1 << LayerMask.NameToLayer("Enemy")));

		RaycastHit2D[] enemies = list.ToArray ();

		foreach(RaycastHit2D collision in enemies) {
			float direction = transform.position.x - collision.transform.position.x;

			collision.transform.gameObject.GetComponent<Enemy> ().takeHit (Mathf.RoundToInt(13f - collision.distance), 3, direction, true);

		}

		if (Mathf.Abs(player.transform.position.x - transform.position.x) < 2) {
			player.takeHit (2);
		}

		player.gameCon.shakeCamera (0.5f, 0.2f);

		Destroy (gameObject);
	}
}
