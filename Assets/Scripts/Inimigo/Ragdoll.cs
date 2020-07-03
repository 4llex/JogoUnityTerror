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
        rigid = GetComponent<Rigidbody>();
    }

    public void DesativaRagdoll()
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
            col.enabled = false;
            ragdollColliders.Add(col);
        }
    }

    public void AtivaRagdoll()
    {

        for(int i=0; i < ragdollRigids.Count; i++)
        {
            ragdollRigids[i].isKinematic = false;
            ragdollColliders[i].enabled = true;
            ragdollRigids[i].transform.gameObject.layer = 10;
        }

        rigid.isKinematic = true;
        GetComponent<CapsuleCollider>().enabled = false;
        StartCoroutine("FinalizaAnimacao");

    }

    IEnumerator FinalizaAnimacao()
    {
        yield return new WaitForEndOfFrame();
        GetComponent<Animator>().enabled = false;
        this.enabled = false;
    }

    public IEnumerator SomeMorto()
    {
        yield return new WaitForSeconds(10);
        rigid.isKinematic = false;
        DesativaRagdoll();
        yield return new WaitForSeconds(4);
        Destroy(this.gameObject);
    }
}
