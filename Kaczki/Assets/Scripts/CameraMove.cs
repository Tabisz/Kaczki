using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {
	public GameObject Mother;

	void Update () {
		
		transform.position = new Vector3 (Mother.transform.position.x, transform.position.y,Mother.transform.position.z);
	}
}
