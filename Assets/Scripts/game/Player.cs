using UnityEngine;
using System.Collections;

namespace SpaceGame {
	public class Player {
	
		public const string DEFAULT_NICK = "N00B";
		
		public NetworkPlayer UnityPlayer {
			get {
				return unityPlayer;
			}
		}
		
		private NetworkPlayer unityPlayer;
		
		public string nickname;
		
		public Player(NetworkPlayer player, string nickname = DEFAULT_NICK) {
			this.unityPlayer = player;
			this.nickname = nickname;
		}
		
		public bool Equals(Player p) {
			return unityPlayer.Equals(p.UnityPlayer);
		}
				
	}
}
