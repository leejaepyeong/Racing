using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Car : MonoBehaviour
{
    public float carSpeed;
    public float playerSpeed;
    public Transform target;
    int nextTarget;
    public bool player;
    public GameObject[] itemEffect;

    public Transform bulletPos;
    public GameObject bullet;

    
   public Transform[] tiers;

   
    private void Update()
    {
        if(player)
        UpdateRotate();
    }

    Vector3 CalcTerrainNormal()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Terrain");

        // fl fr bl br
        RaycastHit[] rayHits = new RaycastHit[4];
        Physics.Raycast(tiers[0].position + Vector3.up, Vector3.down, out rayHits[0], layerMask);
        Physics.Raycast(tiers[1].position + Vector3.up, Vector3.down, out rayHits[1], layerMask);
        Physics.Raycast(tiers[2].position + Vector3.up, Vector3.down, out rayHits[2], layerMask);
        Physics.Raycast(tiers[3].position + Vector3.up, Vector3.down, out rayHits[3], layerMask);

        return (rayHits[0].normal + rayHits[1].normal + rayHits[2].normal + rayHits[3].normal).normalized;
    }

    public void UpdateRotate()
    {
        var tn = CalcTerrainNormal();
        transform.rotation = Quaternion.LookRotation(Vector3.Cross(transform.right, tn), tn);
    }

    void OnDrawGizmos()
    {
        var tn = CalcTerrainNormal();
        var endPos = transform.position + (tn * 5.0f);
        Debug.DrawLine(transform.position, endPos, Color.magenta);
        Debug.DrawLine(transform.position,
            transform.position + (Vector3.Cross(transform.right, tn) * 20.0f), Color.cyan);
    }
    


    public void StartAI()
    {
        if (!player)
        {
            target = GameManager.instance.target[nextTarget];
            GetComponent<NavMeshAgent>().speed = carSpeed;
            GetComponent<NavMeshAgent>().SetDestination(target.position);
            StartCoroutine("AI_Move");
            StartCoroutine("AI_Animation");
        }
    }

    IEnumerator AI_Move()
    {
        GetComponent<NavMeshAgent>().SetDestination(target.position);

        while (true)
        {
            float dis = (target.position - transform.position).magnitude;
            

            if (dis <= 1)
            {
                nextTarget += 1;
                if (nextTarget >= GameManager.instance.target.Length)
                {
                    nextTarget = 0;
                }

                target = GameManager.instance.target[nextTarget];
                GetComponent<NavMeshAgent>().SetDestination(target.position);
            }

            yield return null;
        }

    }

    IEnumerator AI_Animation()
    {
        Vector3 lastPosition;
        while (true)
        {
            lastPosition = transform.position;
            yield return new WaitForSecondsRealtime(0.03f);

            if ((lastPosition - transform.position).magnitude > 0)
            {
                Vector3 dir = transform.InverseTransformPoint(lastPosition);
                if (dir.x >= -0.01f && dir.x <= 0.01f)
                    GetComponent<Animator>().Play("Ani_Forward");
                if (dir.x <= -0.01f)
                    GetComponent<Animator>().Play("Ani_Right");
                if (dir.x >= 0.01f)
                    GetComponent<Animator>().Play("Ani_Left");
            }
            if ((lastPosition - transform.position).magnitude <= 0)
            {
                GetComponent<Animator>().Play("Ani_Idle");
            }

            yield return null;
        }
    }


    public void UseBullet()
    {
        var tn = CalcTerrainNormal();
        transform.rotation = Quaternion.LookRotation(Vector3.Cross(transform.right, tn), tn);
        
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, gameObject.transform.rotation);
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 30;
        Destroy(intantBullet, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player)
        {
            if (other.gameObject.tag == "Finish")
            {
                if (GameManager.instance.check)
                {
                    GameManager.instance.check = false;

                    if (GameManager.instance.lap > 0)
                    {

                        SE_Manager.instance.PlayerSound(SE_Manager.instance.lap);
                        GameManager.instance.LapTime();
                    }
                    GameManager.instance.lap += 1;
                }

            }
            if (other.gameObject.tag == "CheckPoint")
            {
                GameManager.instance.check = true;
            }
        }
        if(!player && other.gameObject.tag == "Bullet")
        {
            Destroy(other.gameObject);
            GetComponent<NavMeshAgent>().speed -= 2f;
            Invoke("EffectOff",5f);
        }
    }

    void EffectOff()
    {
        GetComponent<NavMeshAgent>().speed += 2f;
    }
    


}
