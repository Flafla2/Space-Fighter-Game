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
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if(stream.isWriting) {
			float r_velocity = velocity;
			stream.Serialize(ref r_velocity);
			
			int r_friendlyPlayer = friendlyPlayer;
			stream.Serialize(ref r_friendlyPlayer);
		} else if(stream.isReading) {
			float r_velocity = 0;
			stream.Serialize(ref r_velocity);
			velocity = r_velocity;
			
			int r_friendlyPlayer = 0;
			stream.Serialize(ref r_friendlyPlayer);
			friendlyPlayer = r_friendlyPlayer;
		}
	}

}
