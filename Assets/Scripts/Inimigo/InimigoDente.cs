using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BASA;

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

    public GameObject objDeslisa;
    public bool estaMorto;
    public bool bravo;
    public Renderer render;

    // Start is called before the first frame update
    void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        anim = GetComponent<Animator>();
        ragScript = GetComponent<Ragdoll>();
        render = GetComponentInChildren<Renderer>();

        estaMorto = false;
        ragScript.DesativaRagdoll();
    }

    // Update is called once per frame
    void Update()
    {
        if (!estaMorto)
        {
            distanciaDoPlayer = Vector3.Distance(transform.position, player.transform.position);

            VaiAtrasJogador();
            OhaParaPlayer();

            if(hp <= 50)
            {
                bravo = true;
                render.material.color = Color.red;
                velocidade = 8;
            }

            if (hp <= 0 && !estaMorto)
            {
                render.material.color = Color.white;
                objDeslisa.SetActive(false);
                estaMorto = true;
                ParaDeAndar();
                navMesh.enabled = false;
                ragScript.AtivaRagdoll();

            }
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
        int n;

        n = Random.Range(0, 10);

        if (n % 2 == 0 && !bravo) 
        {
            ParaDeAndar();
        }

        ParaDeAndar();
        hp -= dano;
    }

    void ParaDeAndar()
    {
        navMesh.isStopped = true;
        anim.SetTrigger("levouTiro");
        anim.SetBool("podeAndar", false);
        CorrigeRigEntra();
    }

    public void DaDanoPlayer()
    {
        player.GetComponent<MovimentaPersonagem>().hp -= 10;
    }
}
