using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    /* kontrolowanie kaczki gracza*/

    public float rotSpeed = 10;
    public float maxSpeed = 0.07f;
    public float minSpeed = 0.03f;
    private HordeHandler hordeHandler;
    private Client client;
    public Vector3 waypoint;
    Vector3 dst = Vector3.zero;

    void Start()
    {
		hordeHandler = GetComponent<HordeHandler> ();
        client = GameObject.Find("Client").GetComponent<Client>();
    }
    void Update () {
        dst = TrackMouse();
        float speed = maxSpeed;
        dst.y = 0.4f;

        waypoint = transform.position;
       // transform.position = Vector3.Lerp(transform.position, dst, speed);
  

        Quaternion lookRotation = Quaternion.LookRotation((dst - transform.position).normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotSpeed);
        


        
        if (Input.GetButtonDown("Fire1"))
        {
            if (hordeHandler.LocalAddDuck())
                client.AddChild();
                
        }
        if (Input.GetButtonDown("Fire2"))
        {

            if (hordeHandler.LocalDelDuck(TrackMouse()))
                client.DelChild(TrackMouse());


        }

        
        

    }
    private void FixedUpdate()
    {
        if(dst != Vector3.zero && Input.GetKey(KeyCode.Space))
        GetComponent<Rigidbody>().AddForce(dst - transform.position );
        Debug.Log(-dst * 50);
        if (transform.position.y < 0)
            GetComponent<Rigidbody>().AddForce(new Vector3(0, 20, 0));
    }

    private Vector3 TrackMouse()
    {
        Vector3 dst = transform.position;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);



        if (Physics.Raycast(ray, out hit))
            if (hit.collider != null)
            {
                dst = new Vector3(hit.point.x, 0, hit.point.z);
                dst = CheckObsticle(dst);
            }
            else
                dst = transform.position;

        return dst;

    }
   private Vector3 CheckObsticle(Vector3 _dst)
    {
        Vector3 dst = _dst - transform.position;
        GameObject border;
        RaycastHit hit;

        Ray ray = new Ray(transform.position, dst);
        Debug.DrawRay(transform.position, dst);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.layer == 8)
            {
                border = hit.collider.gameObject;
                float distance = (hit.point - transform.position).magnitude;
                if (distance < 1f)
                {

                    return border.GetComponent<BorderScript>().CalculatePoint(_dst, transform.position);


                }

            }

        }






        return _dst;
    }



  

}

