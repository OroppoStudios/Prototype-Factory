using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    

    private void OnCollisionEnter(Collision collision)
    { 
       if(collision.body.TryGetComponent(out NewDash DashController))
       {
            //if(DashController.)
       }
       if (collision.body.TryGetComponent(out HealthSystem collisionHealth))
       {

       }

    }

    private void OnCollisionExit(Collision collision)
    {
        //Destroy Enemy
        //Animation
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
