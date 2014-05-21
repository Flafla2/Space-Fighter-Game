using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpaceGame;

public class Spawner : MonoBehaviour {

	public static List<Spawner> AllSpawners = new List<Spawner>();
	
	public Player player;
	public Transform shipPrefab;
	public GuiManager3D guiManager;
	public bool singleplayerSpawn = false;
	
	public static void Respawn(Player player) {
		foreach(Spawner s in AllSpawners) {
			if(s.player == player) {
				s.guiManager.clearMessages();
				if(NetVars.IsMine(s.networkView))
					s.SpawnShip();
				else
					s.networkView.RPC("SpawnShip",s.player.UnityPlayer);
			}
		}
	}
	
	// Use this for initialization
	void Start () {
		if(Network.isServer) {
			networkView.RPC ("SpawnShip",player.UnityPlayer);
		} else
			SpawnShip();
		
		if(NetVars.SinglePlayer() && singleplayerSpawn) SpawnShip();
		
		AllSpawners.Add(this);
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
