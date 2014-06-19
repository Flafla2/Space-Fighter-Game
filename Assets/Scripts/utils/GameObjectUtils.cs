using UnityEngine;
using System.Collections;

namespace SpaceGame {
	public class GameObjectUtils {
	
		public static void SetLayerRecursively(GameObject obj, int newLayer)
		{
			if (null == obj)
			{
				return;
			}
			
			obj.layer = newLayer;
			
			foreach (Transform child in obj.transform)
			{
				if (null == child)
				{
					continue;
				}
				SetLayerRecursively(child.gameObject, newLayer);
			}
		}
		
	}
}