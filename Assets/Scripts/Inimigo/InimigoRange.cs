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

    public GameObject pedraPermanente;
    public GameObject pedraInstancia;

    public bool usaCurvaAnimacao;
    public CapsuleCollider col;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        navMesh = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        anim = GetComponent<Animator>();
        estaMorto = false;
        usaCurvaAnimacao = false;
        col = GetComponent<CapsuleCollider>();
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
                anim.CrossFade("Zombie Death", 0.2f);
                transform.gameObject.layer = 10;
                anim.applyRootMotion = true;
                col.direction = 2;
                usaCurvaAnimacao = false;
            }

            if(usaCurvaAnimacao && !anim.IsInTransition(0))
            {
                col.height = anim.GetFloat("AlturaCollider");
                col.center = new Vector3(0, anim.GetFloat("CentroColliderY"), 0);
            }
            else
            {
                col.height = 2;
                col.center = new Vector3(0, 1, 0);
            }
        }
    }

    void InstanciaPedra()
    {
        pedraPermanente.SetActive(false);
        GameObject pedra = Instantiate(pedraInstancia, anim.GetBoneTransform(HumanBodyBones.RightHand).transform);
        pedra.transform.parent = null;
        pedra.transform.LookAt(player.transform.position);
        JogaPedra jogaScript = pedra.GetComponent<JogaPedra>();
        jogaScript.Joga();
    }

    public void AparecePedraPermanente()
    {
        pedraPermanente.SetActive(true);
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
            usaCurvaAnimacao = true;
        }
        else
        {
            pedraPermanente.SetActive(false);
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

    public void LevouDano(int dano)
    {
        hp -= dano;
    }
}
