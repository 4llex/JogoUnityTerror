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

    public ParticleSystem rastroBala;
    public AudioSource audioArma;
    public AudioClip[] sonsArma;

    public int carregador = 3;
    public int municao = 17;

    UIManager uiScript;
    public GameObject posUI;

    public bool automatico;


    // Start is called before the first frame update
    void Start()
    {
        automatico = false;
        estaAtirando = false;
        anim = GetComponent<Animator>();
        audioArma = GetComponent<AudioSource>();
        uiScript = GameObject.FindWithTag("uiManager").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        uiScript.municao.transform.position = Camera.main.WorldToScreenPoint(posUI.transform.position);
        uiScript.municao.text = municao.ToString() + "/" + carregador.ToString();

        if (anim.GetBool("ocorreAcao"))
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            audioArma.clip = sonsArma[1];
            audioArma.Play(2);
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

        if (Input.GetButtonDown("Fire1") || automatico?Input.GetButton("Fire1"):false)
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
            else if(!estaAtirando && municao == 0 && carregador > 0)
            {
                anim.Play("Recarrega");
                carregador--;
                municao = 17;
            }
            else if(municao == 0 && carregador == 0)
            {
                audioArma.clip = sonsArma[3];
                audioArma.Play();

            }
        }

        if (Input.GetKeyDown(KeyCode.R) && carregador > 0 && municao < 17)
        {
            anim.Play("Recarrega");
            carregador--;
            municao = 17;
        }

        if (Input.GetButton("Fire2"))
        {
            anim.SetBool("mira", true);
            posUI.transform.localPosition = new Vector3(0f, 0.1f, -0.2f);
        }
        else
        {
            anim.SetBool("mira", false);
            posUI.transform.localPosition = new Vector3(-0.02f, 0.1f, -0.2f);
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

        if (Physics.Raycast(new Vector3(ray.origin.x + Random.Range(-0.05f, 0.05f), ray.origin.y + Random.Range(-0.05f, 0.05f), ray.origin.z), 
            Camera.main.transform.forward, out hit))
        {
            InstanciaEfeitos();

            if(hit.transform.tag == "objArrasta")
            {
                Vector3 direcaoBala = ray.direction;

                if(hit.rigidbody != null)
                {
                    hit.rigidbody.AddForceAtPosition(direcaoBala * 500, hit.point);
                }
            }
        }

        yield return new WaitForSeconds(0.3f);
        estaAtirando = false;
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
}
