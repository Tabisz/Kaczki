using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour {
    private Vector3 pos;
    private float dis = 1;
    private float startTime;
    public float speed = 0.02f;
    public float rotSpeed =10;
  

    void Update()
    {

            transform.position = Vector3.Lerp(transform.position, pos, dis * speed);

        UpdateRot();
    }


    public void UpdatePos(Vector3 _pos)
    {
        dis = Vector3.Distance(transform.position, _pos);
        pos = _pos;



    }
    public void UpdateRot()
    {
        Quaternion lookRotation = Quaternion.LookRotation((pos - transform.position).normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotSpeed);

    }



}
