using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {

	public float maxSize = 5;
	public Transform shieldEdge1;
	public Transform shieldEdge2;
	public Transform shieldCore;
	public Transform healthBar;
	
	private float health = 1;
	private float shield = 1;

	void Start () {
		healthBar.localScale = new Vector3(maxSize,1,1);
		shieldCore.localScale = new Vector3(maxSize,1,1);
		shieldEdge2.localPosition = new Vector3(-maxSize,shieldEdge2.localPosition.y,shieldEdge2.localPosition.z);
		shieldEdge1.renderer.enabled = true;
		shieldEdge2.renderer.enabled = true;
		shieldCore.renderer.enabled = true;
	}
	
//	void Update() {
//		if(shield != 0)
//			setShield (shield - 0.1f * Time.deltaTime);
//		else if(health != 0)
//			setHealth (health - 0.1f * Time.deltaTime);
//	}
	
	public void setHealth(float health) {
		if(health < 0) health = 0;
		if(health > 1) health = 1;
		this.health = health;
		healthBar.localScale = new Vector3(maxSize*health,1,1);
		healthBar.renderer.enabled = health > 0;
	}
	
	public void setShield(float shield) {
		if(shield < 0) shield = 0;
		if(shield > 1) shield = 1;
		this.shield = shield;
		shieldCore.localScale = new Vector3(maxSize*shield,1,1);
		
		if(shield <= (0.05/maxSize)) shieldEdge1.localScale = new Vector3(shield*maxSize/0.05f,1,1);
		else shieldEdge1.localScale = new Vector3(1,1,1);
		
		shieldEdge2.renderer.enabled = shield >= 1;
		shieldEdge1.renderer.enabled = shield > 0;
		shieldCore.renderer.enabled = shield > 0;
	}
}
