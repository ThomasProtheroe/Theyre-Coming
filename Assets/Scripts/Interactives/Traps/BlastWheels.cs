using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastWheels : RemoteCarTrap {

	[SerializeField]
	private ParticleSystem fusePS;
	[SerializeField]
	private ParticleSystem explosionPS;
	[SerializeField]
	private GameObject explosion;

	protected new void OnTriggerEnter2D(Collider2D other) {
		if (isDeployed) {
			if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "AreaWall") {
				explode ();
			}
		}
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

			collision.transform.gameObject.GetComponent<Enemy> ().takeHit (Mathf.RoundToInt(15f - collision.distance) , 3, direction, true);

		}

		playerCon.gameCon.shakeCamera (0.5f, 0.2f);

		source.Stop ();
		breakItem ();
	}

	protected override void onDeploy() {
		var sh = fusePS.shape;
		var main = fusePS.main;
		var em = fusePS.emission;
		main.startLifetime = 1f;
		em.rateOverTimeMultiplier = 2500.0f;
		sh.position = new Vector3 (0.11f, sh.position.y, sh.position.z);
	}

	override public void onTravel() {
		fusePS.Stop ();
	}

	override public void onArrival() {
		fusePS.Play ();
	}

	public override bool onBreak() {
		fusePS.Stop ();
		source.Stop ();

		//De-parent the Particle System so the particles dont disappear when the item is destroyed
		fusePS.transform.parent = null;
		fusePS.GetComponent<DestroyAfterTime> ().StartCoroutine ("destroyAfterTime", 1.0f);

		return true;
	}
}
