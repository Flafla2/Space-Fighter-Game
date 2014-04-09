using UnityEngine;
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

	private Vector3 camPos;
	private Vector3 retPos;
	private Quaternion camRot;
	private Quaternion retRot;
	private float speed;
	private Vector2 aimAngle;

	// Use this for initialization
	void Start () {
		speed = startSpeed;
		camPos = cam.localPosition;
		camRot = cam.localRotation;
		retPos = reticule.localPosition;
		retRot = reticule.localRotation;

		Screen.lockCursor = true;
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

		cam.localPosition = camPos;
		cam.localRotation = camRot;
		cam.RotateAround(ship.localPosition,transform.up,-yawRaw*tilt);
		cam.RotateAround(ship.localPosition,transform.right,-pitchRaw*tilt);

		float mousex = Input.GetAxis("Mouse X");
		float mousey = Input.GetAxis("Mouse Y");

		aimAngle.x += mousex*aimReticuleSpeed;
		aimAngle.x = Mathf.Max(Mathf.Min(aimAngle.x,45),-45);
		aimAngle.y += mousey*aimReticuleSpeed;
		aimAngle.y = Mathf.Max(Mathf.Min(aimAngle.y,45),-45);

		reticule.localPosition = retPos;
		reticule.localRotation = retRot;
		reticule.RotateAround(ship.localPosition,transform.up,aimAngle.x);
		reticule.RotateAround(ship.localPosition,transform.right,-aimAngle.y);
		reticule.LookAt(cam,transform.up);
		reticule.Rotate(reticule.right,180);
	}

	void OnGUI() {
		GUI.Label(new Rect(0,0,100,50),("Speed: "+trueSpeed()));
	}

	float trueSpeed()
	{
		if(speed < speedDeadZone && speed > -speedDeadZone)
			return 0;
		return speed;
	}
}
