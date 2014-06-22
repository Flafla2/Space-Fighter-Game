using UnityEngine;
using System.Collections;
using SpaceGame;

public class Laser : MonoBehaviour {

	public Player friendlyPlayer;
	public float velocity;
	
	void Update() {
		transform.Translate(Vector3.forward*velocity*Time.deltaTime,Space.Self);
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log(other.gameObject.name);
		if(NetVars.Authority())
		{
			Ship ship = other.gameObject.GetComponent<Ship>();
			if((ship == null && !other.gameObject.tag.Equals("Obstacle")) || (ship != null && ship.player.Equals(friendlyPlayer)))
				return;
			
			if(ship != null) {
				if(!ship.IsMine())
					ship.networkView.RPC("Damage",ship.networkView.owner,0.1f);
				else
					ship.Damage(0.1f);
			}
			
			if(NetVars.SinglePlayer())
				Destroy(gameObject);
			else if(enabled) {
				Network.RemoveRPCs(networkView.viewID);
				Network.Destroy(gameObject);
			}
		}
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if(stream.isWriting) {
			float r_velocity = velocity;
			stream.Serialize(ref r_velocity);
			
			NetworkPlayer r_friendlyPlayer = friendlyPlayer.UnityPlayer;
			stream.Serialize(ref r_friendlyPlayer);
		} else if(stream.isReading) {
			float r_velocity = 0;
			stream.Serialize(ref r_velocity);
			velocity = r_velocity;
			
			NetworkPlayer r_friendlyPlayer = new NetworkPlayer();
			stream.Serialize(ref r_friendlyPlayer);
			if(friendlyPlayer == null || !friendlyPlayer.UnityPlayer.Equals(r_friendlyPlayer))
				friendlyPlayer = NetVars.getPlayer(r_friendlyPlayer);
//			else if(!friendlyPlayer.UnityPlayer.Equals(r_friendlyPlayer))
//				friendlyPlayer = new Player(r_friendlyPlayer,friendlyPlayer.nickname);
		}
	}

}
