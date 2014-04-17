﻿using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {

	public float maxSpeed;
	public float minSpeed;
	public float startSpeed;
	public float rollSpeed;
	public float yawSpeed;
	public float pitchSpeed;
	public float throttleRate;
	public float tilt;
	public float speedDeadZone;

	public Transform cam;
	public Transform ship;
	public Transform reticule;

	public float aimReticuleSpeed; // In Degrees
	public float aimRadius;
	public Vector2 aimUnits;

	public Transform laser;
	public int fireRate;
	public float laserForce;

	private Vector3 camPos;
	private float speed;
	private float lastFireTime;

	// Use this for initialization
	void Start () {
		speed = startSpeed;
		camPos = cam.localPosition;
		Screen.lockCursor = true;

		foreach(Renderer o in reticule.gameObject.GetComponentsInChildren<Renderer>())
		{
			o.material.renderQueue = 4000; //4000+ is overlay
		}
	}
	
	// Update is called once per frame
	void Update () {
		float pitchRaw = Input.GetAxis("Pitch");
		float yawRaw = Input.GetAxis("Yaw");
		float rollRaw = Input.GetAxis("Roll");
		float pitch	= pitchRaw	*pitchSpeed	*Time.deltaTime;
		float yaw	= yawRaw	*yawSpeed	*Time.deltaTime;
		float roll	= rollRaw	*rollSpeed	*Time.deltaTime;

		transform.Rotate(Vector3.up,yaw,Space.Self);
		transform.Rotate(Vector3.right,pitch,Space.Self);
		transform.Rotate(Vector3.forward,roll,Space.Self);

		float throttle = Input.GetAxis("Throttle")*throttleRate*Time.deltaTime;
		speed = Mathf.Max(Mathf.Min(speed+throttle,maxSpeed),minSpeed);
		
		transform.Translate(Vector3.forward*trueSpeed()*Time.deltaTime,Space.Self);

		float mousex = Input.GetAxis("Mouse X");
		float mousey = Input.GetAxis("Mouse Y");

		reticule.localPosition += new Vector3(mousex,mousey,0)*aimReticuleSpeed;
		float xpos = reticule.localPosition.x;
		float ypos = reticule.localPosition.y;
		if(xpos*xpos+ypos*ypos > aimRadius*aimRadius)
		{
			float angle = Mathf.Atan2(ypos,xpos);
			xpos = Mathf.Cos(angle)*aimRadius;
			ypos = Mathf.Sin(angle)*aimRadius;
		}
		reticule.localPosition = new Vector3(xpos,ypos,reticule.localPosition.z);

		if(Input.GetButton("Fire1") && Time.time-lastFireTime > ((float)fireRate)/1000f)
		{
			lastFireTime = Time.time;

			Transform proj = Instantiate(laser) as Transform;
			proj.position = ship.position;
			proj.LookAt(reticule);
			proj.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(reticule.position-proj.position)*laserForce);
		}

		cam.localPosition = camPos;
		Vector3 retDir = Vector3.Normalize(new Vector3(reticule.localPosition.x,reticule.localPosition.y,3))*tilt;
		cam.localPosition += retDir;
	}

	void OnGUI() {
		GUI.Label(new Rect(0,0,100,50),("Speed: "+trueSpeed()));
		float sides = 32;
		Vector2? lastPoint = null;
		for(float x=0;x<=360;x+=360f/sides)
		{
			Vector3 point = transform.TransformPoint(new Vector3(aimRadius*Mathf.Cos(Mathf.Deg2Rad*x),aimRadius*Mathf.Sin(Mathf.Deg2Rad*x),reticule.localPosition.z));
			Vector2 scrPoint = Camera.main.WorldToScreenPoint(point);
			scrPoint.y = Screen.height-scrPoint.y;
			if(lastPoint == null)
			{
				lastPoint = scrPoint;
				continue;
			}
			Drawing.DrawLine(lastPoint.Value,scrPoint,new Color(1,1,1,0.5f),2,false);
			lastPoint = scrPoint;
		}
	}

	float trueSpeed()
	{
		if(speed < speedDeadZone && speed > -speedDeadZone)
			return 0;
		return speed;
	}
}
