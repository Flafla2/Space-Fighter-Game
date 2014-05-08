using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

	public int friendlyPlayer = -1;
	public float velocity;
	
	void Update() {
		transform.Translate(Vector3.forward*velocity*Time.deltaTime,Space.Self);
	}

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
