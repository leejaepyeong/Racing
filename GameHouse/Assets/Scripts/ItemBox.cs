using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    public GameObject pref;
    public float spawnRateMin = 3f;
    public float spawnRateMax = 6f;

    float spawnRate;
    float timeAfterSpawn;

    bool spawn = false;
    void Start()
    {
        timeAfterSpawn = 0f;
        spawnRate = Random.Range(spawnRateMin, spawnRateMax); 
    }

    void Update()
    {
        // 시간 갱신
        if (spawn == false)
        {
            timeAfterSpawn += Time.deltaTime;
        }
        

        if (timeAfterSpawn >= spawnRate)
        {
            spawn = true;
            timeAfterSpawn = 0f;    //누적 시간 리셋

            GameObject items = Instantiate(pref, transform.position, transform.rotation); 

            spawnRate = Random.Range(spawnRateMin, spawnRateMax);   //  스폰시간 랜덤
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Car")
        {
            spawn = false;
        }
    }
}

