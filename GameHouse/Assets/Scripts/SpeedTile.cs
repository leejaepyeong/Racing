using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedTile : MonoBehaviour
{
   
    public enum Type {Up, Down };
    public Type type;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Car")
        {
            
            Car car = other.gameObject.GetComponent<Car>();
            ApplyEffect(car, type);
        }
    }

    void ApplyEffect(Car car, Type type)
    {
        switch(type)
        {
            case Type.Up:
                StartCoroutine("SpeedUp",car);
                break;
            case Type.Down:
                StartCoroutine("SpeedDown", car);
                break;
        }

    }

    IEnumerator SpeedUp(Car c)
    {
       
        c.playerSpeed = 16f;
        yield return new WaitForSeconds(5f);
        EffectOff(c);
        StopCoroutine("SpeedUp");
    }

    IEnumerator SpeedDown(Car c)
    {
        c.playerSpeed = 7f;
        yield return new WaitForSeconds(5f);
        EffectOff(c);
        StopCoroutine("SpeedDown");
    }

    void EffectOff(Car c)
    {
        c.playerSpeed = 10f;
    }

}
