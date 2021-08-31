using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Controller : MonoBehaviour, IPointerUpHandler, IDragHandler
{


    public RectTransform pad;
    public RectTransform stick;
    Vector3 playerRotate;

    Car player;
    Animator playerAni;
    bool onMove;
    bool onRMove;
    float playerSpeed;

    [Header("MiniMap")]
    public Transform minimapCam;
    public GameObject minimap;


    //입력키
    bool bDown;
    bool rDown;
    bool mDown;


   

    public void StartController()
    {
        player = GameManager.instance.player;
        playerAni = player.GetComponent<Animator>();
        StartCoroutine("PlayerMove");
    }
    public void OnMove()
    {
        StartCoroutine("Acceleration");
        onMove = true;
        onRMove = false;
    }

    public void OffMove()
    {
        if(!onRMove)
        {
            StartCoroutine("Braking");
        }
        
    }
    IEnumerator PlayerMove()
    {
        minimap.SetActive(true);


        while (true)
        {
            GameManager.instance.curSpeedText.text = string.Format("{0:000.00}", playerSpeed * 10);
            if (onMove || onRMove)
            {
                if(onMove)
                {
                    player.transform.Translate(Vector3.forward * playerSpeed * Time.deltaTime);

                    if (Mathf.Abs(stick.localPosition.x) > pad.rect.width * 0.2f)
                        player.transform.Rotate(playerRotate * 30 * Time.deltaTime);

                    if (Mathf.Abs(stick.localPosition.x) <= pad.rect.width * 0.2f)
                        playerAni.Play("Ani_Forward");
                    if (stick.localPosition.x > pad.rect.width * 0.2f)
                        playerAni.Play("Ani_Right");
                    if (stick.localPosition.x < pad.rect.width * -0.2f)
                        playerAni.Play("Ani_Left");
                }
                else if(onRMove)
                {
                    player.transform.Translate(Vector3.forward * -1 * playerSpeed * Time.deltaTime);

                    if (Mathf.Abs(stick.localPosition.x) > pad.rect.width * 0.2f)
                        player.transform.Rotate(playerRotate * 30 * Time.deltaTime);

                    if (Mathf.Abs(stick.localPosition.x) <= pad.rect.width * 0.2f)
                        playerAni.Play("Ani_Forward");
                    
                }
                    

                player.transform.GetChild(3).gameObject.SetActive(true);
                player.transform.GetChild(4).gameObject.SetActive(false);
            }

            if (!onMove && !onRMove)
            {
                playerAni.Play("Ani_Idle");
                player.transform.GetChild(3).gameObject.SetActive(false);
                player.transform.GetChild(4).gameObject.SetActive(true);
            }

            minimapCam.position = player.transform.position + new Vector3(0, 80, 0);
            yield return null;
        }
    }

    IEnumerator Acceleration()
    {
        StopCoroutine("Braking");

        while (true)
        {

            playerSpeed += 7 * Time.deltaTime;
            if (playerSpeed >= player.playerSpeed)
                playerSpeed -= 15 * Time.deltaTime;

            yield return null;
        }
    }

    IEnumerator Braking()
    {
        StopCoroutine("Acceleration");

        while (true)
        {
            playerSpeed -= 7 * Time.deltaTime;

            if (playerSpeed <= 0)
            {
                playerSpeed = 0;
                onMove = false;
                StopCoroutine("Braking");
            }
            yield return null;

        }
    }




    public void OnPointerUp(PointerEventData eventData)
    {
        stick.localPosition = Vector3.zero;
        playerRotate = Vector3.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        stick.position = eventData.position;
        stick.localPosition = Vector3.ClampMagnitude(eventData.position -
            (Vector2)pad.position, pad.rect.width * 0.5f);

        playerRotate = new Vector3(0, stick.localPosition.x, 0).normalized;
    }




    //pc용
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && !rDown)
        {
            mDown = true;
            OnMove();
        }
            
        if (Input.GetKeyUp(KeyCode.A) && !onRMove)
        {
            mDown = false;
            OffMove();
        }
            

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            stick.localPosition = new Vector3(-pad.rect.width * 0.35f, 0, 0);

            playerRotate = new Vector3(0, stick.localPosition.x, 0).normalized;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            stick.localPosition = new Vector3(pad.rect.width * 0.35f, 0, 0);

            playerRotate = new Vector3(0, stick.localPosition.x, 0).normalized;
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            stick.localPosition = new Vector3(0, 0, 0);
            playerRotate = new Vector3(0, stick.localPosition.x, 0).normalized;
        }

        GetInput();
        Bullet();
        RMove();

    }

    void GetInput()
    {
        bDown = Input.GetButtonDown("Fire1");
        rDown = Input.GetButton("R");
    }


    void Bullet()
    {
        if(bDown && GameManager.instance.hasbullet)
        {
            GameManager.instance.player.UseBullet();
            GameManager.instance.hasbullet = false;
        }
    }


    void RMove()
    {
        if(rDown && !mDown)
        {
            StartCoroutine("rMoveUp");
        }
        else if(!rDown && !onMove )
        {
            StartCoroutine("rMoveDown");
        }
        
    }
    IEnumerator rMoveUp()
    {
        onMove = false;
        onRMove = true;
        playerSpeed += 7 * Time.deltaTime;
        if (playerSpeed > player.playerSpeed - 3)
            playerSpeed -= 10 * Time.deltaTime;

        yield return null;
    }
    IEnumerator rMoveDown()
    {
        StopCoroutine("rMoveUp");

        while (true)
        {
            
            playerSpeed -= 7 * Time.deltaTime;

            if (playerSpeed <= 0)
            {
                playerSpeed = 0;
                onRMove = false;
                StopCoroutine("rMoveDown");
            }
            yield return null;

        }
    }
}