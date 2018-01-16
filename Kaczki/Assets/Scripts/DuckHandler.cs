using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckHandler : MonoBehaviour {

	public GameObject Next;
	public GameObject Prev;
	public float acceleration = 1f;

	public bool follow = true;
	public bool bullet = false;
	bool flag = true;
	public float rotSpeed = 10;

	private AudioSource saw;


	void Start ()
	{
		saw = gameObject.GetComponent<AudioSource> ();
	}

	void Update () {
		if (follow) {
			Vector3 dst;
			dst = Next.transform.position;
			float speed = Vector3.Distance (transform.position, dst) / acceleration;
			transform.position = Vector3.Lerp (transform.position, dst, speed);

			Quaternion lookRotation = Quaternion.LookRotation ((dst - transform.position).normalized);
			transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * rotSpeed);



		} else{
			if(transform.position.y < -5)
			Destroy (gameObject);

		}

	}


		
	void OnCollisionEnter(Collision col)
	{
		if (flag) {
			if (col.gameObject.tag == "Ground") {
				saw.Play ();
				flag = false;
				bullet = false;
			}
		}
		if (col.gameObject.tag == "Enemy") {
			Destroy (col.gameObject);
			saw.Play ();
			EnemyHandler.enemyCount--;

			gameObject.GetComponent<Rigidbody> ().AddForce (Vector3.up*100);
		}
	}

	/*


	  */






}
