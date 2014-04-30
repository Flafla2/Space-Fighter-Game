using UnityEngine;
using System.Collections;

namespace SpaceGame {
	public class SpMath : MonoBehaviour {

		/// <summary>
		/// Cosine interpolation between two variables.
		/// </summary>
		/// <param name="y1">Number to interpolate from</param>
		/// <param name="y2">Number to interpolate to</param>
		/// <param name="mu">Alpha between y1 and y2 (0.0-1.0)</param>
		public static float cerp(float y1, float y2, float mu) {
			float mu2;
			
			mu2 = (1-Mathf.Cos(mu*Mathf.PI))/2;
			return(y1*(1-mu2)+y2*mu2);
		}

		/// <summary>
		/// Cosine interpolation between two vectors.
		/// </summary>
		/// <param name="y1">Vector to interpolate from</param>
		/// <param name="y2">Vector to interpolate to</param>
		/// <param name="mu">Alpha between y1 and y2 (0.0-1.0)</param>
		public static Vector3 cerp(Vector3 y1, Vector3 y2, float mu) {
			return new Vector3(
				cerp (y1.x,y2.x,mu),
			    cerp (y1.y,y2.y,mu),
			    cerp (y1.z,y2.z,mu)
			);
		}

	}
}