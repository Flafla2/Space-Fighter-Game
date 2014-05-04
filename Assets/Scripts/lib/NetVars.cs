using UnityEngine;

namespace SpaceGame {

	public class NetVars {
		
		public static bool SinglePlayer() {
			return Network.peerType == NetworkPeerType.Disconnected;
		}
		
	}
	
}