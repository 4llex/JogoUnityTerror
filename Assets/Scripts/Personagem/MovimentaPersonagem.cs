using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BASA { 

    public class MovimentaPersonagem : MonoBehaviour
    {
        [Header("Config. Personagem")]
        public CharacterController controle;
        [Tooltip("velocidaded de movimentação")]
        public float velocidade = 6f;
        [Tooltip("altura do pulo")]
        public float alturaPulo = 3f;
        [Tooltip("gravidade")]
        public float gravidade = -20f;
        public bool estaCorrendo;
        public AudioClip[] audiosPulo;
        AudioSource audioPulo;
        bool noAr;

        [Header("Verifica Chao")]
        public Transform checaChao;
        public float raioEsfera = 0.4f;
        public LayerMask chaoMask;
        public bool estaNoChao;
        Vector3 velocidadeCai;

        [Header("Verifica Abaixado")]
        public Transform cameraTransform;
        public bool estaAbaixado;
        public bool levantarBloqueado;
        public float alturaLevantado, alturaAbaixado, posicaoCameraEmPe, posicaoCameraAbaixado;
        RaycastHit hit;
        float velocidadeCorrente = 1f;

        [Header("Status Personagem")]
        public float hp = 100;
        public float stamina = 100;
        public bool cansado;
        public Respiracao scriptResp;


        // Start is called before the first frame update
        void Start()
        {
            
            estaCorrendo = false;
            cansado = false;

            // controle obtem caractercontroller do personagem
            controle = GetComponent<CharacterController>();

            // personagem começa em pe
            estaAbaixado = false;

            //anexxa camera ao  Camera Trasnform
            cameraTransform = Camera.main.transform;

            audioPulo = GetComponent<AudioSource>();
            noAr = false;
        }

        // Update is called once per frame
        void Update()
        {
            Verificacoes();
            MovimentoAbaixa();
            Inputs();
            CondicaoPlayer();
            SomPulo();

        }

        /// <summary>
        /// metodo de verificacoes
        /// </summary>
        void Verificacoes()
        {
            // para saber se personagen esta tocando no chao e assim poder pular
            estaNoChao = Physics.CheckSphere(checaChao.position, raioEsfera, chaoMask);

            // suaviza a velocidade de queda apos o pulo
            if (estaNoChao && velocidadeCai.y < 0)
            {
                velocidadeCai.y = -2f;
            }

            //fazendo o personagem andar
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            // vetor para mover o personagem
            Vector3 move = (transform.right * x + transform.forward * z).normalized;
            controle.Move(move * velocidade * Time.deltaTime);


            velocidadeCai.y += gravidade * Time.deltaTime;

            controle.Move(velocidadeCai * Time.deltaTime);           

        }

        void Inputs()
        {
            //fazer o personagem correr
            if(Input.GetKey(KeyCode.LeftShift) && estaNoChao && !estaAbaixado && !cansado)
            {
                estaCorrendo = true;
                velocidade = 9;
                stamina -= 0.3f;
                stamina = Mathf.Clamp(stamina, 0, 100);
            }
            else
            {
                estaCorrendo = false;
                stamina += 0.1f;
                stamina = Mathf.Clamp(stamina, 0, 100);
            }

            //chama metodo para abaixar
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                Abaixa();
            }

            //fazer pular
            if (Input.GetButtonDown("Jump") && estaNoChao)
            {
                velocidadeCai.y = Mathf.Sqrt(alturaPulo * -2f * gravidade);
                audioPulo.clip = audiosPulo[0];
                audioPulo.Play();
            }
        }

        void MovimentoAbaixa()
        {
            controle.center = Vector3.down * (alturaLevantado - controle.height) / 2f;

            if (estaAbaixado)
            {
                controle.height = Mathf.Lerp(controle.height, alturaAbaixado, Time.deltaTime * 3);
                float novoY = Mathf.SmoothDamp(cameraTransform.localPosition.y, posicaoCameraAbaixado, ref velocidadeCorrente, Time.deltaTime * 3);
                cameraTransform.localPosition = new Vector3(0, novoY, 0);
                velocidade = 3f;
                //verifica se o personagem esta abaixado
                ChecaBloqueioAbaixado();
            }
            else
            {
                controle.height = Mathf.Lerp(controle.height, alturaLevantado, Time.deltaTime * 3);
                float novoY = Mathf.SmoothDamp(cameraTransform.localPosition.y, posicaoCameraEmPe, ref velocidadeCorrente, Time.deltaTime * 3);
                cameraTransform.localPosition = new Vector3(0, novoY, 0);
                velocidade = 6f;
                //verifica se o personagem esta abaixado
                ChecaBloqueioAbaixado();
            }
        }

        //faz a esfera aparecer
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(checaChao.position, raioEsfera);
        }

        //metodo para abaixar
        void Abaixa()
        {

            if (levantarBloqueado || estaNoChao == false)
            {
                return;
            }

            estaAbaixado = !estaAbaixado;
            
        }

        //metodo que verifica se tem algo em cima e pode levantar
        void ChecaBloqueioAbaixado()
        {
            //Comando para mostrar o vetor Raycast 
            Debug.DrawRay(cameraTransform.position, Vector3.up * 1.1f, Color.red);

            if (Physics.Raycast(cameraTransform.position, Vector3.up, out hit, 1.1f))
            {
                levantarBloqueado = true;
            }
            else
            {
                levantarBloqueado = false;
            }
        }

        void CondicaoPlayer()
        {
            if(stamina == 0)
            {
                cansado = true;
                scriptResp.forcaResp = 5;
            }

            if(stamina > 20)
            {
                cansado = false;
            }
        }

        void SomPulo()
        {
            if (!estaNoChao)
            {
                noAr = true;
            }

            if(estaNoChao && noAr)
            {
                noAr = false;
                audioPulo.clip = audiosPulo[1];
                audioPulo.Play();
            }
        }

        void OntriggerStay(Collider col)
        {
            if (col.gameObject.CompareTag("cabecaDesliza"))
            {
                controle.SimpleMove(transform.forward * 1000 * Time.deltaTime);
            }
        }

    }
}