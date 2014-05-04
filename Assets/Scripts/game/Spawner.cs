using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {
	
	public int player = 0;
	public Transform shipPrefab;
	public GuiManager3D guiManager;
	
	// Use this for initialization
	void Start () {
		if(Network.isServer) {
			if(UnityNetworkConnection() >= 0) {
				for(int x=0;x<Network.connections.Length;x++) {
					if(Network.connections[x] == networkView.owner) {
						networkView.RPC ("SpawnShip",Network.connections[x]);
					}
				}
			} else
				SpawnShip();
		}
		
		if(Network.peerType == NetworkPeerType.Disconnected) SpawnShip();
	}

	int UnityNetworkConnection() {
		return player-1;
	}
	
	[RPC]
	void SpawnShip() {
		Ship ship;
		if(Network.peerType == NetworkPeerType.Disconnected)
			ship = (Instantiate(shipPrefab,transform.position,Quaternion.identity) as Transform).GetComponent<Ship>();
		else
			ship = (Network.Instantiate(shipPrefab,transform.position,Quaternion.identity,0) as Transform).GetComponent<Ship>();
		ship.guiManager = guiManager;
	}
}
