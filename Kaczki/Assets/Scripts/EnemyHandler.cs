using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour {

	GameObject Mother;
	public float speed = 0.001f;
	public float rotSpeed = 10;
	public static int enemyCount;

	// Use this for initialization
	void Start () {
		Mother = GameObject.FindGameObjectWithTag ("MotherDuck");
		enemyCount++;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 dst;
		dst = Mother.transform.position;
		transform.position = Vector3.Lerp (transform.position, dst, speed*Time.deltaTime);

		Quaternion lookRotation = Quaternion.LookRotation((dst - transform.position ).normalized);
		transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime*rotSpeed);
	}
}
