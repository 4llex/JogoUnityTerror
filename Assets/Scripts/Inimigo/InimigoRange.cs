using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class InimigoRange : MonoBehaviour
{
    public NavMeshAgent navMesh;
    public GameObject player;
    public float distanciaDoAtaque;
    public float distanciaDoPlayer;
    public float velocidade = 5;
    Animator anim;
    public int hp = 100;
    public bool estaMorto;
    public Rigidbody rigid;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        navMesh = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        anim = GetComponent<Animator>();
        estaMorto = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!estaMorto)
        {
            distanciaDoPlayer = Vector3.Distance(transform.position, player.transform.position);

            VaiAtrasJogador();
            OlhaParaPlayer();

            if(hp <= 0)
            {
                estaMorto = true;
                navMesh.isStopped = true;
                navMesh.enabled = true;
                CorrigeRigEntra();
            }
        }
    }

    void OlhaParaPlayer()
    {
        Vector3 direcaoOlha = player.transform.position - transform.position;
        Quaternion rotacao = Quaternion.LookRotation(direcaoOlha);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotacao, Time.deltaTime * 300);
    }

    void VaiAtrasJogador()
    {
        navMesh.speed = velocidade;
        if (distanciaDoPlayer < distanciaDoAtaque)
        {
            navMesh.isStopped = true;
            anim.SetBool("joga", true);
            CorrigeRigEntra();
        }
        else
        {
            anim.SetBool("joga", false);
            navMesh.isStopped = false;
            navMesh.SetDestination(player.transform.position);
            CorrigeRigEntra();
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
        rigid.isKinematic = true;
        rigid.velocity = Vector3.zero;
    }

    void CorrigeRigSai()
    {
        rigid.isKinematic = false;
    }
}
