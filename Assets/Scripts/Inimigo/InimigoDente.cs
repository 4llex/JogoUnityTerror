using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class InimigoDente : MonoBehaviour
{
    public NavMeshAgent navMesh;
    public GameObject player;
    public float distanciaDoAtaque;
    public float distanciaDoPlayer;
    public float velocidade = 5;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        distanciaDoPlayer = Vector3.Distance(transform.position, player.transform.position);

        VaiAtrasJogador();
        OhaParaPlayer();
    }

    void OhaParaPlayer()
    {
        Vector3 direcaoOlha = player.transform.position - transform.position;
        Quaternion rotacao = Quaternion.LookRotation(direcaoOlha);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotacao, Time.deltaTime * 300);
    }

    void VaiAtrasJogador()
    {
        navMesh.speed = velocidade;
        if(distanciaDoPlayer < distanciaDoAtaque)
        {
            navMesh.isStopped = true;
            Debug.Log("Atacando");
            anim.SetTrigger("ataca");
            anim.SetBool("podeAndar", false);
            anim.SetBool("paraAtaque", false);
            CorrigeRigEntra();
        }
        if (distanciaDoPlayer >= 3)
        {
            anim.SetBool("paraAtaque", true);

        }
        if(anim.GetBool("podeAndar"))
        {
            navMesh.isStopped = false;
            navMesh.SetDestination(player.transform.position);
            anim.ResetTrigger("ataca");
            CorrigeRigSai();
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            CorrigeRigEntra();
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            CorrigeRigSai();
        }
    }

    void CorrigeRigEntra()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    void CorrigeRigSai()
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
