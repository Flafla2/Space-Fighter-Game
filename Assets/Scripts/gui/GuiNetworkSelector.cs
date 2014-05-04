using UnityEngine;
using System;

public class GuiNetworkSelector : MonoBehaviour {

	private Rect windowArea		= new Rect(20,20,500,500);
	private Rect chatArea		= new Rect(100,100,300,300);
	private bool chatEnabled	= false;
	private Vector2 logScroll	= new Vector2(0,0);
	private Vector2 chatScroll 	= new Vector2(0,0);
	private string log			= "Space Game V0.1 Alpha - NETWORK LOG\n\n";
	private string chat			= "";
	private string chatBox		= "";
	private string hostPort		= "25565";
	private string clientIP		= "localhost";
	private string clientPort	= "25565";

	void OnGUI() {
		windowArea = GUI.Window(0,windowArea,WindowFunction,"Find a Game");

		if(chatEnabled)
			chatArea = GUI.Window(1,chatArea,ChatFunction,"Chat");
	}

	void ChatFunction(int windowID) {
		Vector2 labelSize = GUI.skin.label.CalcSize(new GUIContent(chat));
		chatScroll = GUI.BeginScrollView(new Rect(10,10,180,chatArea.height-40),chatScroll,new Rect(0,0,labelSize.x,labelSize.y));
		GUI.Label(new Rect(0,0,labelSize.x,labelSize.y),chat); // Read-only
		GUI.EndScrollView();

		GUI.SetNextControlName("chatbox");
		chatBox = GUI.TextField(new Rect(10,chatArea.height-30,180,20),chatBox);
		if(GUI.GetNameOfFocusedControl().CompareTo("chatbox") == 0 && (Event.current.isKey && Event.current.keyCode == KeyCode.Return))
		{
			NetworkViewID viewID = Network.AllocateViewID();
			if(Network.isClient)
				networkView.RPC("RequestMessage", RPCMode.Server, viewID, chatBox);
			else if(Network.isServer)
				RequestMessage(networkView.viewID,chatBox);
			chatBox = "";
		}

		GUI.DragWindow(new Rect(0,0,100000,20));
	}

	void WindowFunction(int windowID) {
		GUI.Label(new Rect(10,20,windowArea.width-20,20),"Host Server");
		GUI.Label(new Rect(10,50,windowArea.width-220,20),"Port: ");
		hostPort = GUI.TextField(new Rect(windowArea.width-210,50,200,20),hostPort);

		string hostText = "Host";
		switch(Network.peerType) {
		case NetworkPeerType.Client:
			hostText = "Disconnect to Host";
			break;
		case NetworkPeerType.Server:
			hostText = "Stop Server";
			break;
		case NetworkPeerType.Connecting:
			hostText = "Connecting...";
			break;
		}

		if(GUI.Button(new Rect(10,80,windowArea.width-20,20),hostText))
		{
			if(Network.peerType == NetworkPeerType.Disconnected) {
				log += "Starting server at port "+hostPort+"...";
				int port = -1;
				try {
					port = int.Parse(hostPort);
				} catch(FormatException) {
					log += "ERROR: Port is in incorrect format\n";
					port = -1;
				}
				if(port != -1) {
					NetworkConnectionError e = Network.InitializeServer(32,port,false);
					if(e == NetworkConnectionError.NoError)
					{
						chatEnabled = true;
						log += "Done.\n";
					}
					else
						log += "ERROR: "+e.ToString()+"\n";
				}
			} else if(Network.peerType == NetworkPeerType.Server) {
				Network.Disconnect();
				chat = "";
				chatBox = "";
				chatEnabled = false;
				chatScroll = Vector2.zero;
				log += "Server has been shut down.\n";
			}
		}

		GUI.Label(new Rect(10,120,windowArea.width-20,20),"Connect to Server");
		GUI.Label(new Rect(10,150,windowArea.width-220,20),"IP Address: ");
		clientIP = GUI.TextField(new Rect(windowArea.width-210,150,200,20),clientIP);
		GUI.Label(new Rect(10,170,windowArea.width-220,20),"Port: ");
		clientPort = GUI.TextField(new Rect(windowArea.width-210,170,200,20),clientPort);

		string connectText = "Connect";
		switch(Network.peerType) {
		case NetworkPeerType.Client:
			connectText = "Disconnect";
			break;
		case NetworkPeerType.Server:
			connectText = "Stop server to Connect";
			break;
		case NetworkPeerType.Connecting:
			connectText = "Connecting...";
			break;
		}

		if(GUI.Button(new Rect(10,200,windowArea.width-20,20),connectText))
		{
			if(Network.peerType == NetworkPeerType.Disconnected) {
				log += "Connecting to server "+clientIP+" at port "+clientPort+"...";
				int port = -1;
				try {
					port = int.Parse(clientPort);
				} catch(FormatException) {
					log += "ERROR: Port is in incorrect format\n";
					port = -1;
				}
				if(port != -1) {
					Network.Connect (clientIP,port);
				}
			} else if(Network.peerType == NetworkPeerType.Client) {
				Network.Disconnect();
				log += "Client has disconnected.\n";
			}
		}

		Vector2 labelSize = GUI.skin.label.CalcSize(new GUIContent(log));
		logScroll = GUI.BeginScrollView(new Rect(10,240,windowArea.width-20,windowArea.height-260),logScroll,new Rect(0,0,labelSize.x,labelSize.y));
		GUI.Label(new Rect(0,0,labelSize.x,labelSize.y),log); // Read-only
		GUI.EndScrollView();

		GUI.Label(new Rect(10,windowArea.height-20,windowArea.width-20,20),("Peer Type: "+Network.peerType.ToString()));
		if(Network.isServer && GUI.Button(new Rect(windowArea.width-100,windowArea.height-20,100,20),"Start Game"))
			networkView.RPC("LoadLevel", RPCMode.All, "dev_scene", 1);

		GUI.DragWindow(new Rect(0,0,100000,20));
	}

	void OnPlayerConnected(NetworkPlayer player) {
		log += "New player connected from "+player.ipAddress+" : "+player.port+"\n";
	}

	void OnPlayerDisconnect(NetworkPlayer player) {
		log += "Player from "+player.ipAddress+" disconnected.\n";
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
		chatBox = "";
		chatScroll = Vector2.zero;
		chatEnabled = false;
	}

	[RPC]
	void OnSendChatMessage(string nick, string msg) {
		chat += "["+nick+"] "+msg+"\n";
	}

	[RPC]
	void RequestMessage(NetworkViewID view, string msg) {
		if(Network.isServer)
			networkView.RPC("OnSendChatMessage", RPCMode.AllBuffered, getPlayerID(view.owner).ToString(), msg);
	}

	[RPC]
	void LoadLevel(string level, int levelID) {
		Network.SetSendingEnabled(0,false);
		Network.isMessageQueueRunning = false;

		Network.SetLevelPrefix(levelID);
		Application.LoadLevel(level);

		Network.isMessageQueueRunning = true;
		Network.SetSendingEnabled(0,true);
	}

	int getPlayerID(NetworkPlayer p) {
		for(int x=0;x<Network.connections.Length;x++) {
			if(Network.connections[x].Equals(p))
				return x;
		}
		return -1;
	}
	
}
