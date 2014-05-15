using UnityEngine;
using System.Collections;
using SpaceGame;

public class Laser : MonoBehaviour {

	public int friendlyPlayer = -1;
	public float velocity;
	
	void Update() {
		transform.Translate(Vector3.forward*velocity*Time.deltaTime,Space.Self);
	}

	void OnTriggerEnter(Collider other) {
		if(NetVars.Authority())
		{
			Ship ship = other.GetComponent<Ship>();
			if(ship != null && ship.player == friendlyPlayer)
				return;
			if(ship != null) {
				if(!ship.IsMine())
					ship.networkView.RPC("Damage",ship.networkView.owner,0.1f);
				else
					ship.Damage(0.1f);
			}
			if(NetVars.SinglePlayer())
				Destroy(gameObject);
			else {
				Network.RemoveRPCs(networkView.viewID);
				Network.Destroy(gameObject);
			}
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
