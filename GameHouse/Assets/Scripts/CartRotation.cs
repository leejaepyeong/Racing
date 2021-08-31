using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartRotation : MonoBehaviour
{
    public Transform[] tiers;


    private void Update()
    {
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
}
