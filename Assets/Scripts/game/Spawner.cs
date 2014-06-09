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
			if(s.player != null && s.player.Equals(player)) {
				s.guiManager.clearMessages();
				
				s.networkView.RPC("SpawnShip",RPCMode.All,player.UnityPlayer);

			}
		}
	}
	
	// Use this for initialization
	void Start () {
		if(Network.isServer) {
			foreach(Player p in NetVars.players) { // TODO: Update this check for when new players are added
				if(player == null && !CheckForPlayer(p)) {
					if(Network.isServer) {
						networkView.RPC ("SpawnShip",RPCMode.All,p.UnityPlayer);
					}
				}
			}
		}
		
		if(NetVars.SinglePlayer() && singleplayerSpawn) SpawnShip(Network.player);
		
		AllSpawners.Add(this);
	}
	
	static bool CheckForPlayer(Player p) {
		foreach(Spawner s in AllSpawners) {
			if(s.player.Equals(p)) return true;
		}
		return false;
	}
	
	[RPC]
	void SpawnShip(NetworkPlayer p) {
		player = NetVars.getPlayer(p);
		
		if(!p.Equals(Network.player)) return;
		
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
