using UnityEngine;
using System.Collections;
using SpaceGame;

[ExecuteInEditMode]
public class WireCubeRenderer : MonoBehaviour {

	public Vector3 size;
	public float lineWidth;
	public Material material;
	public Color color1;
	public Color color2;
	public int layer = 0;
	
	private Vector3[] vec1;
	private Vector3[] vec2;
	
	void Start () {
		Vector3 box1 = transform.position;
		Vector3 box2 = transform.position+size;
		vec1 = new Vector3[]{
			new Vector3(box1.x,box1.y,box1.z),
			new Vector3(box1.x,box1.y,box1.z),
			new Vector3(box1.x,box1.y,box1.z),
			new Vector3(box2.x,box2.y,box1.z),
			new Vector3(box2.x,box2.y,box1.z),
			new Vector3(box2.x,box2.y,box1.z),
			new Vector3(box1.x,box2.y,box2.z),
			new Vector3(box1.x,box2.y,box2.z),
			new Vector3(box1.x,box2.y,box2.z),
			new Vector3(box2.x,box1.y,box2.z),
			new Vector3(box2.x,box1.y,box2.z),
			new Vector3(box2.x,box1.y,box2.z)
		};
		
		vec2 = new Vector3[]{
			new Vector3(box2.x,box1.y,box1.z),
			new Vector3(box1.x,box2.y,box1.z),
			new Vector3(box1.x,box1.y,box2.z),
			new Vector3(box1.x,box2.y,box1.z),
			new Vector3(box2.x,box1.y,box1.z),
			new Vector3(box2.x,box2.y,box2.z),
			new Vector3(box2.x,box2.y,box2.z),
			new Vector3(box1.x,box1.y,box2.z),
			new Vector3(box1.x,box2.y,box1.z),
			new Vector3(box1.x,box1.y,box2.z),
			new Vector3(box2.x,box2.y,box2.z),
			new Vector3(box2.x,box1.y,box1.z)
		};
	}
		
	void OnRenderObject () {
		if(vec1 == null || vec2 == null || GUI.changed)
			Start();
		
		for(int x=0;x<vec1.Length;x++) {
			GuiRenderer3D.drawLine(vec1[x],vec2[x],lineWidth,material,layer);
		}
	}
}
