using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

	public int friendlyPlayer = -1;

	void OnTriggerEnter(Collider other) {
		if(Network.isServer)
		{
			Ship ship = other.GetComponent<Ship>();
			if(ship != null && ship.player == friendlyPlayer)
				return;
			Network.RemoveRPCs(networkView.viewID);
			Network.Destroy(gameObject);
		}
	}

}
