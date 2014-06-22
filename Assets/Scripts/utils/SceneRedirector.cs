using UnityEngine;
using System.Collections;
using SpaceGame;

public class SceneRedirector : MonoBehaviour {

	public int sceneToRedirect;
	
	void Awake () {
		if(NetVars.SinglePlayer())
			Application.LoadLevel(sceneToRedirect);
	}
}
