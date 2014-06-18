using UnityEngine;
using System.Collections;

namespace SpaceGame {
	public class GuiRenderer3D {
	
		public static void drawLine(Vector3 vec1, Vector3 vec2, float w, Material mat, Color color1, Color color2, int layer = 0) {
			Mesh mesh = new Mesh();
			float d = Mathf.Sqrt(Mathf.Pow(vec2.x-vec1.x,2)+Mathf.Pow(vec2.y-vec1.y,2));
			mesh.vertices = new Vector3[8];
			mesh.vertices[0] = new Vector3(0,0,0);
			mesh.vertices[1] = new Vector3(w,0,0);
			mesh.vertices[2] = new Vector3(w,w,0);
			mesh.vertices[3] = new Vector3(0,w,0);
			mesh.vertices[4] = new Vector3(0,0,d);
			mesh.vertices[5] = new Vector3(w,0,d);
			mesh.vertices[6] = new Vector3(w,w,d);
			mesh.vertices[7] = new Vector3(0,w,d);
			
			mesh.triangles = new int[]{
								0,1,2,
								0,2,3,
								4,5,6,
								4,6,7,
								3,2,6,
								3,7,2,
								0,1,5,
								0,4,5,
								1,5,6,
								1,2,6,
								0,4,7,
								0,3,7
								};
			
			mesh.colors = new Color[]{
								color1,
								color1,
								color1,
								color1,
								color2,
								color2,
								color2,
								color2
								};
			
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			Graphics.DrawMesh(mesh,vec1,Quaternion.LookRotation(vec2-vec1),mat,layer);
		}
		
		public static void drawLine(Vector3 vec1, Vector3 vec2, float w, Material mat, int layer = 0) {
			drawLine (vec1, vec2, w, mat, Color.white, Color.white, layer);
		}
		
		public static void drawRect(Vector3 pos, Vector3 dim, Material mat, int layer = 0) {
			Mesh mesh = new Mesh();
			mesh.vertices = new Vector3[8];
			Vector3 vec1 = pos;
			Vector3 vec2 = pos+dim;
			mesh.vertices[0] = new Vector3(vec1.x,vec1.y,vec1.z);
			mesh.vertices[1] = new Vector3(vec2.x,vec1.y,vec1.z);
			mesh.vertices[2] = new Vector3(vec2.x,vec2.y,vec1.z);
			mesh.vertices[3] = new Vector3(vec2.x,vec2.y,vec2.z);
			mesh.vertices[4] = new Vector3(vec2.x,vec1.y,vec2.z);
			mesh.vertices[5] = new Vector3(vec1.x,vec2.y,vec1.z);
			mesh.vertices[6] = new Vector3(vec1.x,vec2.y,vec2.z);
			mesh.vertices[7] = new Vector3(vec1.x,vec1.y,vec2.z);
			
			mesh.triangles = new int[]{
								0,1,5,
								1,2,5,
								1,2,4,
								2,3,4,
								5,2,3,
								5,6,3,
								0,7,1,
								7,4,1,
								0,5,6,
								0,2,6,
								6,7,4,
								6,3,4
								};
								
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			Graphics.DrawMesh(mesh,Vector3.zero,Quaternion.identity,mat,layer);
		}
		
	}
}