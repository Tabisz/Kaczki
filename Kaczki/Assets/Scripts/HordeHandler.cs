using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HordeHandler : MonoBehaviour {
	public GameObject childDuck;
	private GameObject oldest;
	private GameObject youngest;
	private int childCount;
	public int maxChildCount;


	void Start()
	{
		childCount = 0;
	}


	public bool LocalAddDuck()
	{
        if (childCount >= maxChildCount)
            return false;
        else
        {
            AddDuck();
            return true;
        }

	}
    public void AddDuck()
    {
        Vector3 duckPlant = new Vector3(transform.position.x, transform.position.y + transform.localScale.y, transform.position.z);
        GameObject babyD = Instantiate(childDuck, duckPlant, transform.rotation);
        if (childCount == 0)
        {
            youngest = oldest = babyD;
            babyD.GetComponent<DuckHandler>().Next = gameObject;

        }
        else {
            babyD.GetComponent<DuckHandler>().Next = youngest;
            youngest.GetComponent<DuckHandler>().Prev = babyD;
            youngest = babyD;
        }
        childCount++;
    }

    public bool LocalDelDuck(Vector3 dst)
    {
        if (childCount <= 0)
            return false;
        if (childCount == 1)
        {

            FireDuck(dst, oldest);
        }
        else {
            oldest = oldest.GetComponent<DuckHandler>().Prev;
            FireDuck(dst, oldest.GetComponent<DuckHandler>().Next);
            oldest.GetComponent<DuckHandler>().Next = gameObject;
        }
        childCount--;
        return true;
    }

    public void DelDuck(float x, float z)
    {
        Vector3 dst = new Vector3(x, 0.4f, z);

        if (childCount == 1)
        {

            FireDuck(dst, oldest);
        }
        else {
            oldest = oldest.GetComponent<DuckHandler>().Prev;
            FireDuck(dst, oldest.GetComponent<DuckHandler>().Next);
            oldest.GetComponent<DuckHandler>().Next = gameObject;
        }
        childCount--;
    }







    void FireDuck(Vector3 dst, GameObject duck)
    {
        Vector3 duckPlant = new Vector3(transform.position.x, transform.position.y + transform.localScale.y, transform.position.z);
        duck.layer = 13;

        duck.GetComponent<DuckHandler>().follow = false;
        duck.GetComponent<DuckHandler>().bullet = true;
        duck.transform.position = duckPlant;
        duck.GetComponent<Rigidbody>().AddForce((dst - transform.position).normalized * 100);

    }





}
