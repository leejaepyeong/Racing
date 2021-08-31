using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;



public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float baseSpeed;

    [Header("Player")]
    public Car player;
    public int lap;
    public bool check;

    [Header("GameObj")]
    public Car[] car;
    public Transform[] target;
    public Controller controllPad;
    public Transform cam;
    

    [Header("Menu")]
    public GameObject startMenu;
    public GameObject selectMenu;
    public GameObject ui;
    public GameObject finishMenu;

    [Header("Text")]
    public TextMeshProUGUI bestLapTimeText;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI curTimeText;
    public TextMeshProUGUI curSpeedText;
    public TextMeshProUGUI[] lapTimeText;
    public TextMeshProUGUI itemText;
    public TextMeshProUGUI bulletText;


    public bool hasbullet;


    float bestLapTime;
    float curTime;
    private void Awake()
    {
        if (instance == null)
            instance = this;

        SpeedSet();
        BestLapTimeSet();
    }

    public void GameStart()
    {
        if(player != null)
        StartCoroutine("StartCount");
    }

    private void Update()
    {
        if(!hasbullet)
        {
            bulletText.text = "Off";
        }
    }


    void BestLapTimeSet()
    {
        bestLapTime = PlayerPrefs.GetFloat("BestLap");
        bestLapTimeText.text = string.Format("Best {0:00}:{1:00:00}", (int)(bestLapTime / 60 % 60), bestLapTime % 60);

        if (bestLapTime == 0)
            bestLapTimeText.text = "Best  -";
    }

    public void LapTime()
    {
        if (lap == 2)
        {
            SE_Manager.instance.PlayerSound(SE_Manager.instance.goal);
            cam.parent = null;
            StopCoroutine("Timer");
            finishMenu.SetActive(true);

            player.player = false;
            player.StartAI();
            controllPad.gameObject.SetActive(false);
            player.transform.GetChild(3).gameObject.SetActive(false);

            if (curTime < bestLapTime | bestLapTime == 0)
            {
                bestLapTimeText.gameObject.SetActive(false);
                bestLapTimeText.text = string.Format("Best {0:00}:{1:00:00}", (int)(curTime / 60 % 60), curTime % 60);
                bestLapTimeText.gameObject.SetActive(true);

                PlayerPrefs.SetFloat("BestLap", curTime);

            }
        }

        lapTimeText[lap - 1].gameObject.SetActive(false);
        lapTimeText[lap - 1].text = string.Format("{0:00}:{1:00:00}", (int)(curTime / 60 % 60), curTime % 60);
        lapTimeText[lap - 1].gameObject.SetActive(true);
    }

    IEnumerator StartCount()
    {
        selectMenu.SetActive(false);

        ui.SetActive(true);

        SE_Manager.instance.PlayerSound(SE_Manager.instance.count[3]);
        countText.text = "3";
        countText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1);

        SE_Manager.instance.PlayerSound(SE_Manager.instance.count[2]);
        countText.gameObject.SetActive(false);
        countText.text = "2";
        countText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1);

        SE_Manager.instance.PlayerSound(SE_Manager.instance.count[1]);
        countText.gameObject.SetActive(false);
        countText.text = "1";
        countText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1);

        SE_Manager.instance.PlayerSound(SE_Manager.instance.count[0]);
        countText.gameObject.SetActive(false);
        countText.text = "GO!";
        countText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        countText.gameObject.SetActive(false);

        controllPad.gameObject.SetActive(true);
        player.player = true;


        controllPad.StartController();
        for (int i = 0; i < car.Length; i++)
            car[i].StartAI();

        StartCoroutine("Timer");
    }

    IEnumerator Timer()
    {
        while (true)
        {
            curTime += Time.deltaTime;
            curTimeText.text = string.Format("{0:00}:{1:00.00}", (int)(curTime / 60 % 60), curTime % 60);
            yield return null;
        }
    }

    void SpeedSet()
    {
        for (int i = 0; i < car.Length; i++)
        {
            car[i].carSpeed = Random.Range(baseSpeed, baseSpeed + 0.5f);
        }
    }

    public void StartBtn()
    {
        SE_Manager.instance.PlayerSound(SE_Manager.instance.btn);

        startMenu.SetActive(false);
        selectMenu.SetActive(true);
    }

 
    public void ItemEffect(Car cart)
    {
        int rand = Random.Range(0, 7);

        switch (rand)
        {
            case 0:
            case 1:
                StartCoroutine(IT_SpeedUp(cart));
                break;
            case 2:
                StartCoroutine(IT_SpeedDown(cart));
                break;
            case 3:
                StartCoroutine(IT_SpeedUp2(cart));
                break;
            case 4:
                StartCoroutine(IT_PlayerStop(cart));
                break;
            case 5:
            case 6:
            case 7:
                Bullet();
                break;

        }
    }

    IEnumerator IT_SpeedUp(Car c)
    {
        c.playerSpeed = 14f;
        c.itemEffect[0].SetActive(true);
        itemText.gameObject.SetActive(true);
        itemText.text = "Speed Up";
        yield return new WaitForSeconds(5);
        EffectOff(c);
        StopCoroutine("IT_SpeedUp");
    }

    IEnumerator IT_SpeedDown(Car c)
    {
        c.playerSpeed = 7f;
        c.itemEffect[1].SetActive(true);
        itemText.gameObject.SetActive(true);
        itemText.text = "Speed Down";
        yield return new WaitForSeconds(5);
        EffectOff(c);
        StopCoroutine("IT_SpeedDown");

    }

    IEnumerator IT_SpeedUp2(Car c)
    {
        c.playerSpeed = 18f;
        c.itemEffect[0].SetActive(true);
        itemText.gameObject.SetActive(true);
        itemText.text = "Speed Up2";
        yield return new WaitForSeconds(3);
        EffectOff(c);
        StopCoroutine("IT_SpeedUp2");

    }

    IEnumerator IT_PlayerStop(Car c)
    {
        c.playerSpeed = 1f;
        c.itemEffect[1].SetActive(true);
        itemText.gameObject.SetActive(true);
        itemText.text = "Speed Down2";
        yield return new WaitForSeconds(3);
        EffectOff(c);
        StopCoroutine("IT_PlayerStop");

    }
     void EffectOff(Car c)
    {
        c.itemEffect[0].SetActive(false);
        c.itemEffect[1].SetActive(false);
        itemText.gameObject.SetActive(false);
        c.playerSpeed = 10f;
    }

    void Bullet()
    {
        hasbullet = true;
        bulletText.text = "On";
    }

}