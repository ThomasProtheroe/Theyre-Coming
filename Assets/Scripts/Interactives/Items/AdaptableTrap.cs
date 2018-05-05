using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptableTrap : Item {

	[SerializeField]
	private SpikeTrap trap;

	public override void use ()
	{
		SpikeTrap newTrap = Instantiate (trap);
		newTrap.deploy ();
		Destroy (gameObject);
	}
}
