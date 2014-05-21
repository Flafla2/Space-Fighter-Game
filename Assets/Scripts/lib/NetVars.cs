using UnityEngine;
using System;
using System.Collections.Generic;

namespace SpaceGame {

	public class NetVars {
	
		public static List<Player> players = new List<Player>();
		
		public static Player getPlayer(NetworkPlayer netplayer) {
			foreach(Player p in players) {
				if(p.UnityPlayer.Equals(netplayer))
					return p;
			}
			return null;
		}
		
		public static bool SinglePlayer() {
			return Network.peerType == NetworkPeerType.Disconnected;
		}
		
		public static bool Authority() {
			return Network.isServer || SinglePlayer();
		}
		
		public static bool IsMine(NetworkView view) {
			return view.isMine || SinglePlayer();
		}
				
	}
	
}