using UnityEngine;
using System;
using SpaceGame;

public class GuiNetworkSelector : MonoBehaviour {

	public NetworkHandler handler;

	private Rect windowArea		= new Rect(20,20,500,500);
	private Rect chatArea		= new Rect(100,100,300,300);
	private Vector2 logScroll	= new Vector2(0,0);
	private Vector2 chatScroll 	= new Vector2(0,0);
	private string chatBox		= "";
	private string nickBox		= "";
	private string currentNick	= "";
	private string hostPort		= "25565";
	private string clientIP		= "localhost";
	private string clientPort	= "25565";

	void OnGUI() {
		windowArea = GUI.Window(0,windowArea,WindowFunction,"Find a Game");

		if(handler.chatEnabled)
			chatArea = GUI.Window(1,chatArea,ChatFunction,"Chat");
	}

	void ChatFunction(int windowID) {
		Vector2 labelSize = GUI.skin.label.CalcSize(new GUIContent(handler.chat));
		chatScroll = GUI.BeginScrollView(new Rect(10,10,180,chatArea.height-40),chatScroll,new Rect(0,0,labelSize.x,labelSize.y));
		GUI.Label(new Rect(0,0,labelSize.x,labelSize.y),handler.chat); // Read-only
		GUI.EndScrollView();

		GUI.SetNextControlName("chatbox");
		chatBox = GUI.TextField(new Rect(10,chatArea.height-30,180,20),chatBox);
		if(GUI.GetNameOfFocusedControl().CompareTo("chatbox") == 0 && (Event.current.isKey && Event.current.keyCode == KeyCode.Return))
		{
			NetworkViewID viewID = Network.AllocateViewID();
			if(Network.isClient)
				handler.networkView.RPC("RequestMessage", RPCMode.Server, viewID, chatBox);
			else if(Network.isServer)
				handler.RequestMessage(networkView.viewID,chatBox);
			chatBox = "";
		}
		
		if(currentNick.Equals(""))
			currentNick = NetVars.getPlayer(Network.player).nickname;
		
		GUI.SetNextControlName("namebox");
		labelSize = GUI.skin.label.CalcSize(new GUIContent("Change Name"));
		GUI.Label(new Rect(200,20,chatArea.width-200,20),"Change Name");
		nickBox = GUI.TextField(new Rect(200,20+labelSize.y,chatArea.width-200,20),nickBox);
		if(GUI.GetNameOfFocusedControl().CompareTo("namebox") == 0 && (Event.current.isKey && Event.current.keyCode == KeyCode.Return) && !nickBox.Trim().Equals(""))
		{
			handler.networkView.RPC ("ChangeNick", RPCMode.All, Network.player, nickBox);
			nickBox = "";
			currentNick = NetVars.getPlayer(Network.player).nickname;
		}
		GUI.Label(new Rect(200,40+labelSize.y,chatArea.width-200,20),currentNick);

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
				handler.StartServer(hostPort);
			} else if(Network.peerType == NetworkPeerType.Server) {
				handler.Disconnect();
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
				handler.Connect(clientIP,clientPort);
			} else if(Network.peerType == NetworkPeerType.Client) {
				handler.Disconnect();
			}
		}

		Vector2 labelSize = GUI.skin.label.CalcSize(new GUIContent(handler.log));
		logScroll = GUI.BeginScrollView(new Rect(10,240,windowArea.width-20,windowArea.height-260),logScroll,new Rect(0,0,labelSize.x,labelSize.y));
		GUI.Label(new Rect(0,0,labelSize.x,labelSize.y),handler.log); // Read-only
		GUI.EndScrollView();

		GUI.Label(new Rect(10,windowArea.height-20,windowArea.width-20,20),("Peer Type: "+Network.peerType.ToString()));
		if(Network.isServer && GUI.Button(new Rect(windowArea.width-100,windowArea.height-20,100,20),"Start Game"))
			handler.networkView.RPC("LoadLevel", RPCMode.All, "dev_scene", 1);

		GUI.DragWindow(new Rect(0,0,100000,20));
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info) {
		chatBox = "";
		nickBox = "";
		chatScroll = Vector2.zero;
	}
	
}
