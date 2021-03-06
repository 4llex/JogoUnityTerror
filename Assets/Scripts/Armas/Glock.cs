﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BASA;

public class Glock : MonoBehaviour
{

    Animator anim;
    bool estaAtirando;
    RaycastHit hit;

    public GameObject faisca;
    public GameObject buraco;
    public GameObject fumaca;
    public GameObject efeitoTiro;
    public GameObject posEfeitoTiro;
    public GameObject particulaSangue;

    public ParticleSystem rastroBala;
    public AudioSource audioArma;
    public AudioClip[] sonsArma;

    public int carregador = 3;
    public int municao = 17;

    UIManager uiScript;
    public GameObject posUI;
    MovimentaArma movimentaArmaScript;

    public bool automatico;
    public float numeroAleatorioMira;

    public float valorMira;

    // Start is called before the first frame update
    void Start()
    {
        automatico = false;
        estaAtirando = false;
        anim = GetComponent<Animator>();
        audioArma = GetComponent<AudioSource>();
        uiScript = GameObject.FindWithTag("uiManager").GetComponent<UIManager>();
        movimentaArmaScript = GetComponentInParent<MovimentaArma>();
        valorMira = 300;
    }

    // Update is called once per frame
    void Update()
    {
        uiScript.municao.transform.position = Camera.main.WorldToScreenPoint(posUI.transform.position);
        uiScript.municao.text = municao.ToString() + "/" + carregador.ToString();

        ModificaMIra();

        if (anim.GetBool("ocorreAcao"))
        {
            return;
        }

        Automatico();
        Atira();
        Recarrega();
        Mira();
     
    }

    void ModificaMIra()
    {
        if (estaAtirando)
        {
            valorMira = Mathf.Lerp(valorMira, 450, Time.deltaTime * 20);
            uiScript.mira.sizeDelta = new Vector2(valorMira, valorMira);
        }
        else
        {
            valorMira = Mathf.Lerp(valorMira, 300, Time.deltaTime * 20);
            uiScript.mira.sizeDelta = new Vector2(valorMira, valorMira);
        }
    }

    void Automatico()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            audioArma.clip = sonsArma[1];
            audioArma.Play();
            automatico = !automatico;

            if (automatico)
            {
                uiScript.imageModoTiro.sprite = uiScript.spriteModoTiro[1];
            }
            else
            {
                uiScript.imageModoTiro.sprite = uiScript.spriteModoTiro[0];
            }
        }
    }

    void Atira()
    {
        if (Input.GetButtonDown("Fire1") || automatico ? Input.GetButton("Fire1") : false)
        {
            if (!estaAtirando && municao > 0)
            {
                municao--;
                audioArma.clip = sonsArma[0];
                audioArma.Play();
                rastroBala.Play();
                estaAtirando = true;
                StartCoroutine(Atirando());
                //audioArma.Stop();
            }
            else if (!estaAtirando && municao == 0 && carregador > 0)
            {
                anim.Play("Recarrega");
                carregador--;
                municao = 17;
            }
            else if (municao == 0 && carregador == 0)
            {
                audioArma.clip = sonsArma[3];
                audioArma.Play();

            }
        }
    }

    void Recarrega()
    {
        if (Input.GetKeyDown(KeyCode.R) && carregador > 0 && municao < 17)
        {
            anim.Play("Recarrega");
            carregador--;
            municao = 17;
        }
    }

    void Mira()
    {
        if (Input.GetButton("Fire2"))
        {
            anim.SetBool("mira", true);
            posUI.transform.localPosition = new Vector3(0f, 0.1f, -0.2f);
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 45, Time.deltaTime * 10);
            uiScript.mira.gameObject.SetActive(false);
            movimentaArmaScript.valor = 0.01f;
            numeroAleatorioMira = 0f;
        }
        else
        {
            anim.SetBool("mira", false);
            posUI.transform.localPosition = new Vector3(-0.02f, 0.1f, -0.2f);
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60, Time.deltaTime * 10);
            uiScript.mira.gameObject.SetActive(true);
            movimentaArmaScript.valor = 0.05f;
            numeroAleatorioMira = 0.05f;
        }
    }

    IEnumerator Atirando()
    {
        float screenX = Screen.width / 2;
        float screenY = Screen.height / 2;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(screenX, screenY));
        anim.Play("Atirar");

        GameObject efeitoTiroObj = Instantiate(efeitoTiro, posEfeitoTiro.transform.position, posEfeitoTiro.transform.rotation);
        efeitoTiroObj.transform.parent = posEfeitoTiro.transform;

        if (Physics.Raycast(new Vector3(ray.origin.x + Random.Range(-numeroAleatorioMira, numeroAleatorioMira), ray.origin.y + Random.Range(-numeroAleatorioMira, numeroAleatorioMira), ray.origin.z), 
            Camera.main.transform.forward, out hit))
        {
            if(hit.transform.tag == "inimigo")
            {

                if(hit.transform.GetComponent<InimigoDente>()  || hit.transform.GetComponent<InimigoRange>())
                {
                    inimigoVerificadorDano();
                }
                else if(hit.rigidbody != null && hit.transform.GetComponentInParent<InimigoDente>())
                {
                    AdicionaForca(ray, 900);
                }
            
                GameObject particulaCriada = Instantiate(particulaSangue, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                particulaCriada.transform.parent = hit.transform;
            }
            else
            {
                InstanciaEfeitos();

                if(hit.rigidbody != null)
                {
                    AdicionaForca(ray, 400);
                }
            }
          
        }

        yield return new WaitForSeconds(0.3f);
        estaAtirando = false;
    }

    void inimigoVerificadorDano()
    {
        if (hit.transform.GetComponent<InimigoDente>())
        {
            hit.transform.GetComponent<InimigoDente>().LevouDano(25);
        }
        else if(hit.transform.GetComponent<InimigoRange>())
        {
            hit.transform.GetComponent<InimigoRange>().LevouDano(25);
        }
    }

    void InstanciaEfeitos()
    {
        Instantiate(faisca, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
        Instantiate(fumaca, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));

        GameObject buracoObj = Instantiate(buraco, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
        buracoObj.transform.parent = hit.transform;
    }

    void sonMagazine()
    {
        audioArma.clip = sonsArma[1];
        audioArma.Play();
    }

    void SomUp()
    {
        audioArma.clip = sonsArma[2];
        audioArma.Play();
    }

    void AdicionaForca(Ray ray, float forca)
    {
        Vector3 direcaoBala = ray.direction;
        hit.rigidbody.AddForceAtPosition(direcaoBala * forca, hit.point);
    }
}
