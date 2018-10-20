using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptableTrap : Item {

	[SerializeField]
	private SpikeTrap trap;

	protected override void Start() {
		usable = true;

		base.Start ();
	}

	public override void use ()
	{
		SpikeTrap newTrap = Instantiate (trap);
		newTrap.manualStart ();
		newTrap.transform.position = new Vector2 (player.transform.position.x, transform.position.y);
		newTrap.deploy ();
		Destroy (gameObject);
	}
}
