using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{

    List<Rigidbody> ragdollRigids = new List<Rigidbody>();
    public Rigidbody rigid;
    List<Collider> ragdollColliders = new List<Collider>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void IniciaRagdoll()
    {
        Rigidbody[] rigs = GetComponentsInChildren<Rigidbody>();

        for (int i =0; i < rigs.Length; i++)
        {

            if(rigs[i] == rigid)
            {
                continue;
            }

            ragdollRigids.Add(rigs[i]);
            rigs[i].isKinematic = true;

            Collider col = rigs[i].gameObject.GetComponent<Collider>();
            col.isTrigger = true;
            ragdollColliders.Add(col);
        }
    }
}
