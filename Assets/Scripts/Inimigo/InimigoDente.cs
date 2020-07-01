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
    public int hp = 100;
    Ragdoll ragScript;

    public bool estaMorto;

    // Start is called before the first frame update
    void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        anim = GetComponent<Animator>();
        ragScript = GetComponent<Ragdoll>();

        estaMorto = false;
        ragScript.DesativaRagdoll();
    }

    // Update is called once per frame
    void Update()
    {
        distanciaDoPlayer = Vector3.Distance(transform.position, player.transform.position);

        VaiAtrasJogador();
        OhaParaPlayer();

        if( hp <= 0 && !estaMorto)
        {
            estaMorto = true;
            ParaDeAndar();
            ragScript.AtivaRagdoll();
            this.enabled = false;
        }
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
        ragScript.rigid.isKinematic = true;
        ragScript.rigid.velocity = Vector3.zero;
    }

    void CorrigeRigSai()
    {
        ragScript.rigid.isKinematic = false;
    }

    public void LevouDano(int dano)
    {
        hp -= dano;
    }

    void ParaDeAndar()
    {
        navMesh.isStopped = true;
        anim.SetBool("podeAndar", false);
    }
}
