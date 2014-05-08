using UnityEngine;
using System.Collections;
using SpaceGame;

public class Spawner : MonoBehaviour {
	
	public int player = 0;
	public Transform shipPrefab;
	public GuiManager3D guiManager;
	
	// Use this for initialization
	void Start () {
		if(Network.isServer) {
			if(UnityNetworkConnection() >= 0) {
				if(UnityNetworkConnection() < Network.connections.Length)
					networkView.RPC ("SpawnShip",Network.connections[UnityNetworkConnection()]);
			} else
				SpawnShip();
		}
		
		if(NetVars.SinglePlayer() && player == 0) SpawnShip();
	}

	int UnityNetworkConnection() {
		return player-1;
	}
	
	[RPC]
	void SpawnShip() {
		Debug.Log("Spawning Ship");
		Ship ship;
		if(NetVars.SinglePlayer())
			ship = (Instantiate(shipPrefab,transform.position,Quaternion.identity) as Transform).GetComponent<Ship>();
		else
			ship = (Network.Instantiate(shipPrefab,transform.position,Quaternion.identity,0) as Transform).GetComponent<Ship>();
		ship.guiManager = guiManager;
		ship.player = player;
	}
}
