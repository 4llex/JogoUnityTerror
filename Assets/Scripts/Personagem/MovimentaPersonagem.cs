using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BASA { 
    public class MovimentaPersonagem : MonoBehaviour
    {
        public CharacterController controle;
        //velocidade de movimento do pers.
        [Tooltip("velocidaded de movimentação")]
        public float velocidade = 6f;
        [Tooltip("altura do pulo")]
        public float alturaPulo = 3f;
        [Tooltip("gravidade")]
        public float gravidade = -20f;

        public Transform checaChao;
        public float raioEsfera = 0.4f;
        public LayerMask chaoMask;
        public bool estaNoChao;

        Vector3 velocidadeCai;

        public Transform cameraTransform;
        public bool estaAbaixado;
        public bool levantarBloqueado;
        public float alturaLevantado, alturaAbaixado, posicaoCameraEmPe, posicaoCameraAbaixado;
        RaycastHit hit;

        float velocidadeCorrente = 1f;

        // Start is called before the first frame update
        void Start()
        {
            // controle obtem caractercontroller do personagem
            controle = GetComponent<CharacterController>();

            // personagem começa em pe
            estaAbaixado = false;

            //anexxa camera ao  Camera Trasnform
            cameraTransform = Camera.main.transform;

        }

        // Update is called once per frame
        void Update()
        {
            Verificacoes();
            MovimentoAbaixa();
            Inputs();
       
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
            //chama metodo para abaixar
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                Abaixa();
            }

            //fazer pular
            if (Input.GetButtonDown("Jump") && estaNoChao)
            {
                velocidadeCai.y = Mathf.Sqrt(alturaPulo * -2f * gravidade);
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

    }
}