using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

	[SerializeField]
	private AudioClip explosionSound;
	[SerializeField]
	private AudioClip meatyExplosionSound;
	private PlayerController player;

	// Use this for initialization
	void Start () {
		//Raycast to get all enemies/players, deal damage to them
		var list = new List<RaycastHit2D> ();
		list.AddRange (Physics2D.RaycastAll(transform.position, Vector2.right, 10f, 1 << LayerMask.NameToLayer("Enemy")));
		list.AddRange (Physics2D.RaycastAll(transform.position, Vector2.left, 10f, 1 << LayerMask.NameToLayer("Enemy")));

		RaycastHit2D[] enemies = list.ToArray ();

		foreach(RaycastHit2D collision in enemies) {
			float direction = transform.position.x - collision.transform.position.x;

			int damageDone = Mathf.RoundToInt (15f - collision.distance);
			if (damageDone > 11) {
				collision.transform.gameObject.GetComponent<Enemy> ().GetComponent<Enemy> ().setGibOnDeath (true);
			}

			collision.transform.gameObject.GetComponent<Enemy> ().takeHit (damageDone, 3, direction, false);

		}

		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
			
		if (Mathf.Abs(player.transform.position.x - transform.position.x) < 2) {
			player.takeHit (2);
		}

		player.gameCon.shakeCamera (0.5f, 0.2f);

		if (enemies.Length > 0) {
			GameObject.FindGameObjectWithTag ("SoundController").GetComponent<SoundController> ().playPriorityOneShot (meatyExplosionSound);
		} else {
			GameObject.FindGameObjectWithTag ("SoundController").GetComponent<SoundController> ().playPriorityOneShot (explosionSound);
		}
	}
	
	protected void destroyOnFinish() {
		Destroy (gameObject);
	}
}
