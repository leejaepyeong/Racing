using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemEffect : MonoBehaviour
{
    
    private void Update()
    {
        transform.Rotate(0,25 * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider other)
    {
       if(other.gameObject.tag == "Car")
        { 
            Car cart = other.gameObject.GetComponent<Car>();

            Destroy(gameObject);
            GameManager.instance.ItemEffect(cart);
            
        }
    }

  
}
