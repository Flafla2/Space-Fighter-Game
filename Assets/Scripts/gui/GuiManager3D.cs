using UnityEngine;
using System.Collections;

public class GuiManager3D : MonoBehaviour {

	public Transform[] messages;
	public string[] messageNames;
	private Transform currentMessage = null;

	void Start() {
		if(messages.Length != messageNames.Length)
			Debug.LogError("Error: Length of Message Names != Length of Messages!");
	}

	public void displayMessage(string name)
	{
		if(currentMessage != null) {
			Destroy(currentMessage);
			currentMessage = null;
		}

		int index = indexOfName(name);
		if(index >= 0)
		{
			currentMessage = Instantiate(messages[index]) as Transform;
			currentMessage.parent = transform;
			currentMessage.localPosition = Vector3.zero;
			currentMessage.localRotation = Quaternion.identity;
		}
		else
			Debug.LogError("Error: Invalid Message Name Given!");
	}

	private int indexOfName(string name) {
		for(int x=0;x<messageNames.Length;x++) {
			if(messageNames[x].CompareTo(name) == 0)
				return x;
		}
		return -1;
	}

}
