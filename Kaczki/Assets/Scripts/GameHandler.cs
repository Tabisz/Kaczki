using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour {
    /* nieużywany skrypt do obsługi wrogów*/
	public GameObject Enemy;
	public GameObject Mother;
	public int maxEnemy;

	// Use this for initialization
	void Start () {
		EnemyHandler.enemyCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (EnemyHandler.enemyCount <= maxEnemy) {
			Instantiate (Enemy, GetSpawn (), transform.rotation);

		}
	
		}




	/*Vector3 GetSpawn()
	{
		Vector3 spawn;
		int side = Random.Range (1, 5);
		if (side <= 2)
		{
			spawn = new Vector3 (-10, 2, Random.Range (-5, 5));
		} 
		else if (side <= 3)
		{
			
			spawn = new Vector3 (Random.Range (-5, 5), 2, 10);
		} 
		else if (side <= 4)
		{
			spawn = new Vector3 (10, 2, Random.Range (-5, 5));
		}
		else
		{
			spawn = new Vector3 (Random.Range (-5, 5), 2, -10);
		}
		Debug.Log (spawn);
		return spawn;
	}*/

	Vector3 GetSpawn()
	{
		
		Vector3 spawn = new Vector3 (Random.Range (-50, 50), 2, Random.Range (-50, 50));
		return spawn;
	}
}
