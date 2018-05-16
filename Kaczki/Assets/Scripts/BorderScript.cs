using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderScript : MonoBehaviour {
    /*skrypt odpowiadajacy za odpowiednie kierowanie kaczką przy krawędziach. Oblicza punkt wzdłuż sciany aby kaczka w nią nie wpływała*/

	public int side;
	// Use this for initialization
	public Vector3 CalculatePoint(Vector3 mousePoint,Vector3 playerTransform)
	{
		if(side == 1)//boki
			mousePoint = new Vector3 (playerTransform.x, playerTransform.y, mousePoint.z);
			else
			if(side == 2)//gora lub dol
			mousePoint = new Vector3 (mousePoint.x, playerTransform.y, playerTransform.z);
		Debug.DrawLine (playerTransform, mousePoint,Color.red);
		return mousePoint;
	}

}
