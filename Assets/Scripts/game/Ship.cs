using UnityEngine;
using System.Collections;
using SpaceGame;

public class Ship : MonoBehaviour {

	public float maxSpeed;
	public float minSpeed;
	public float startSpeed;
	public float rollSpeed;
	public float yawSpeed;
	public float pitchSpeed;
	public float throttleRate;
	public float descelerateRate;
	public float tilt;
	public float speedDeadZone;
	
	public float health = 1;
	
	public Transform cam;
	public Transform ship;
	public Transform reticule;
	public Transform deathExplosion;

	public float aimReticuleSpeed; // In Degrees
	public float aimRadius;

	public Transform laser;
	public int fireRate;
	public float laserVelocity;

	public GuiManager3D guiManager;
	public string debug_msg = "";

	private Vector3 camPos;
	private float speed;
	private float lastFireTime;
	private bool alive = true;
	private float respawnTime = -1;
	private Vector3 rawRot = Vector3.zero; // Used for network approximation
	
	public Player player;

	// Use this for initialization
	void Start () {
		speed = startSpeed;
		camPos = cam.localPosition;
		Screen.lockCursor = true;
		player = NetVars.getPlayer(networkView.owner);

		foreach(Renderer o in reticule.gameObject.GetComponentsInChildren<Renderer>())
		{
			o.material.renderQueue = 4000; //4000+ is overlay
		}
		
		if(!IsMine ())
		{
			Destroy (reticule.gameObject);
			Destroy (cam.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(!alive || !IsMine())
		{
			if(alive)
			{
				transform.Rotate(Vector3.up,rawRot.y,Space.Self);
				transform.Rotate(Vector3.right,rawRot.x,Space.Self);
				transform.Rotate(Vector3.forward,rawRot.z,Space.Self);
				
				transform.Translate(Vector3.forward*trueSpeed()*Time.deltaTime,Space.Self); // Approximation for high ping
			}
			
			return;
		}
		
		if(health <= 0) {
			Kill();
			return;
		}
		
		float pitchRaw = Input.GetAxis("Pitch");
		float yawRaw = Input.GetAxis("Yaw");
		float rollRaw = Input.GetAxis("Roll");
		float pitch	= pitchRaw	*pitchSpeed	*Time.deltaTime;
		float yaw	= yawRaw	*yawSpeed	*Time.deltaTime;
		float roll	= rollRaw	*rollSpeed	*Time.deltaTime;
		
		rawRot = new Vector3(pitch,yaw,roll);

		transform.Rotate(Vector3.up,yaw,Space.Self);
		transform.Rotate(Vector3.right,pitch,Space.Self);
		transform.Rotate(Vector3.forward,roll,Space.Self);

		float throttle = Input.GetAxis("Throttle")*Time.deltaTime;
		if(throttle > 0) throttle *= throttleRate;
		else if(throttle < 0) throttle *= descelerateRate;
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
		
		if(Input.GetButton("Fire1") && Time.time-lastFireTime > ((float)fireRate)/1000f) {
			lastFireTime = Time.time;
			if(NetVars.Authority())
				Shoot (reticule.position);
			else
				networkView.RPC("Shoot",RPCMode.Server,reticule.position);
		}
			

		cam.localPosition = camPos;
		Vector3 retDir = Vector3.Normalize(new Vector3(reticule.localPosition.x,reticule.localPosition.y,3))*tilt;
		cam.localPosition += retDir;
	}

	void OnGUI() {
		if(!IsMine ())
			return;
			
		if(!alive) {
			if(Screen.lockCursor) Screen.lockCursor = false;
			if(Time.time < respawnTime) {
				GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
				labelStyle.alignment = TextAnchor.MiddleCenter;
				GUI.Label(new Rect(0,0,Screen.width,Screen.height),("Respawn in "+(int)(respawnTime-Time.time)),labelStyle);
			} else if(GUI.Button(new Rect(Screen.width/2-100,Screen.height/2-50,200,100),"RESPAWN")) {
				if(NetVars.SinglePlayer())
					Destroy(gameObject);
				else
					Network.Destroy(gameObject);
				Spawner.Respawn(player);
			}
			return;
		} else if(!Screen.lockCursor) Screen.lockCursor = true;

		GUI.Label(new Rect(0,0,100,50),("Speed: "+trueSpeed()));
		GUI.Label(new Rect(0,50,100,50),("Health: "+(health*100)));
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
		
		GUI.Label(new Rect(Screen.width-100,0,100,500),debug_msg);
	}

	void OnTriggerEnter(Collider other) {
		if(!NetVars.Authority())
			return;
		if(other.gameObject.CompareTag("Obstacle"))
		{
			if(IsMine())
				Kill();
			else
				networkView.RPC("Kill",networkView.owner);
		}
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if(stream.isWriting) {
			bool r_alive = alive;
			stream.Serialize(ref r_alive);
			
			float r_speed = speed;
			stream.Serialize(ref r_speed);
			
			Vector3 r_rot = rawRot;
			stream.Serialize(ref r_rot);
			
			NetworkPlayer r_player = player.UnityPlayer;
			stream.Serialize(ref r_player);
		} else if(stream.isReading) {
			bool r_alive = false;
			stream.Serialize(ref r_alive);
			alive = r_alive;
			
			float r_speed = 0;
			stream.Serialize(ref r_speed);
			speed = r_speed;
			
			Vector3 r_rot = Vector3.zero;
			stream.Serialize(ref r_rot);
			rawRot = r_rot;
			
			NetworkPlayer r_player = new NetworkPlayer();
			stream.Serialize(ref r_player);
			if(!player.UnityPlayer.Equals(r_player))
				player = new Player(r_player,player.nickname);
		}
	}

	float trueSpeed()
	{
		if(speed < speedDeadZone && speed > -speedDeadZone)
			return 0;
		return speed;
	}
	
	public bool IsMine() {
		return networkView.isMine || NetVars.SinglePlayer();
	}
	
	[RPC]
	void DisplayMessage(string message) {
		guiManager.displayMessage(message);
	}
	
	[RPC]
	void Kill() {
		alive = false;
		respawnTime = Time.time+5;
			
		Transform explosion;
		if(NetVars.SinglePlayer()) {
			explosion = Instantiate(deathExplosion,ship.position,Quaternion.identity) as Transform;
			Destroy(ship.gameObject);
			Destroy(reticule.gameObject);
		}
		else {
			Vector3 shipPos = ship.position;
			explosion = Network.Instantiate(deathExplosion,shipPos,Quaternion.identity,0) as Transform;
			Network.Destroy(ship.gameObject);
			Destroy(reticule.gameObject);
		}
		explosion.position = ship.position;
		
		foreach(Collider c in GetComponents<Collider>())
			Destroy(c);
		
		if(IsMine ())
			DisplayMessage("You Died");
		else
			networkView.RPC("DisplayMessage",networkView.owner,"You Died");
	}
	
	[RPC]
	void Shoot(Vector3 aim) {
		Transform proj;
		if(NetVars.SinglePlayer())
			proj = Instantiate(laser,ship.position,Quaternion.identity) as Transform;
		else
			proj = Network.Instantiate(laser, ship.position, Quaternion.identity, 0) as Transform;
		
		proj.LookAt(aim);
		proj.gameObject.GetComponent<Laser>().friendlyPlayer = player;
		proj.gameObject.GetComponent<Laser>().velocity = laserVelocity;
		debug_msg += "bam!\n";
	}
	
	[RPC]
	public void Damage(float percent) {
		health -= percent;
	}
}