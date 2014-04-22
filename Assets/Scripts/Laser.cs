using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

	public int friendlyPlayer = -1;

	void OnTriggerEnter(Collider other) {
		Ship ship = other.GetComponent<Ship>();
		if(ship != null && ship.player == friendlyPlayer)
			return;
		Destroy (gameObject);
	}

}
