using UnityEngine;
using System;

namespace SpaceGame {

	public class NetVars {
			
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