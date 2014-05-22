using UnityEngine;
using System;
using SpaceGame;

public class NetworkHandler : MonoBehaviour {

	public string log = "Space Game V0.1 Alpha - NETWORK LOG\n\n";
	public string chat = "";
	public bool chatEnabled	= false;
	
	public void StartServer(string hostPort) {
		int port = -1;
		try {
			port = int.Parse(hostPort);
		} catch(FormatException) {
			log += "ERROR: Server port is in incorrect format\n";
			port = -1;
		}
		if(port != -1)
			StartServer(port);
	}
	
	public void StartServer(int port) {
		log += "Starting server at port "+port+"...";
		NetworkConnectionError e = Network.InitializeServer(32,port,false);
		if(e == NetworkConnectionError.NoError)
		{
			chatEnabled = true;
			log += "Done.\n";
		}
		else
			log += "ERROR: "+e.ToString()+"\n";
		
		networkView.RPC("RegisterPlayer",RPCMode.AllBuffered,Network.player);
	}
	
	public void Disconnect() {
		bool server = Network.isServer;
		Network.Disconnect();
		chat = "";
		chatEnabled = false;
		if(server)
			log += "Server has been shut down.\n";
		else
			log += "Client has disconnected.\n";
		
		NetVars.players.Clear();
	}
	
	public void Connect(string clientIP, string clientPort) {
		int port = -1;
		try {
			port = int.Parse(clientPort);
		} catch(FormatException) {
			log += "ERROR: Client port is in incorrect format\n";
			port = -1;
		}
		if(port != -1)
			Connect (clientIP,port);
	}
	
	public void Connect(string clientIP, int clientPort) {
		log += "Connecting to server "+clientIP+" at port "+clientPort+"...";
		Network.Connect (clientIP,clientPort);
	}
	
	void OnPlayerConnected(NetworkPlayer player) {
		log += "New player connected from "+player.ipAddress+" : "+player.port+"\n";
		
		networkView.RPC("RegisterPlayer",RPCMode.AllBuffered,player);
	}
	
	void OnPlayerDisconnect(NetworkPlayer player) {
		log += "Player from "+player.ipAddress+" disconnected.\n";
		
		
		networkView.RPC("UnRegisterPlayer",RPCMode.AllBuffered,player);
	}
	
	void OnFailedToConnect(NetworkConnectionError error) {
		log += "ERROR: "+error.ToString()+"\n";
	}
	
	void OnConnectedToServer() {
		log += "Done.\n";
		chatEnabled = true;
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info) {
		log += "Disconnected from server: "+info.ToString()+"\n";
		chat = "";
		chatEnabled = false;
	}
	
	[RPC]
	public void RegisterPlayer(NetworkPlayer player) {
		NetVars.players.Add(new Player(player));
	}
	
	[RPC]
	public void UnRegisterPlayer(NetworkPlayer player) {
		for(int x=0;x<NetVars.players.Count;x++) {
			if(NetVars.players[x].UnityPlayer.Equals(player))
				NetVars.players.RemoveAt(x);
		}
	}
	
	[RPC]
	public void OnSendChatMessage(string nick, string msg) {
		chat += "["+nick+"] "+msg+"\n";
	}
	
	[RPC]
	public void RequestMessage(NetworkViewID view, string msg) {
		if(Network.isServer)
			networkView.RPC("OnSendChatMessage", RPCMode.All, NetVars.getPlayer(networkView.owner).nickname, msg);
	}
	
	[RPC]
	public void LoadLevel(string level, int levelID) {
		Debug.Log ("Loading level ("+levelID+"): "+level);
		Network.SetSendingEnabled(0,false);
		Network.isMessageQueueRunning = false;
		
		Network.SetLevelPrefix(levelID);
		Application.LoadLevel(level);
		
		Network.isMessageQueueRunning = true;
		Network.SetSendingEnabled(0,true);
	}
	
	[RPC]
	void ChangeNick(NetworkPlayer player, string nick) {
		Player p = NetVars.getPlayer(player);
		chat += "<GAME> Player "+p.nickname+" changed name to "+nick+"\n";
		p.nickname = nick;
	}
}
