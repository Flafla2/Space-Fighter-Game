using UnityEngine;
using System.Collections;
using SpaceGame;

public class RandRotator : MonoBehaviour {

	public float lookMagnitude;
	public float rotateTime;

	private Vector3 currentLookPos;
	private bool started = false;
	private Vector3 nextLookPos;
	private float time1;
	private float time2;
	private Matrix4x4 mat;

	void Start () {
		currentLookPos	= new Vector3(0,0,5);
		nextLookPos 	= new Vector3(0,0,5);
		mat = transform.localToWorldMatrix;
		started = true;

		StartCoroutine ("PickRandomPos");
	}

	void Update () {
		Vector3 calcLookPos = mat.MultiplyPoint(SpMath.cerp(currentLookPos,nextLookPos,(Time.time-time1)/(time2-time1)));
		transform.LookAt(calcLookPos);
	}

	IEnumerator PickRandomPos () {
		while(true) {
			currentLookPos 	= nextLookPos;
			nextLookPos.x	= Random.Range (-lookMagnitude, lookMagnitude);
			nextLookPos.y 	= Random.Range (-lookMagnitude, lookMagnitude);

			time1 = Time.time;
			time2 = Time.time+rotateTime;

			yield return new WaitForSeconds(rotateTime);
		}
	}

	void OnDrawGizmosSelected() {
		if(!started)
			return;

		Vector3 calcLookPos = mat.MultiplyPoint(SpMath.cerp(currentLookPos,nextLookPos,(Time.time-time1)/(time2-time1)));

		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(calcLookPos, 1);
	}
}
