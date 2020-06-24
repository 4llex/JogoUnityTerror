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
            // para saber se personagen esta tocando no chao e assim poder pular
            estaNoChao = Physics.CheckSphere(checaChao.position, raioEsfera, chaoMask);

            // suaviza a velocidade de queda apos o pulo
            if(estaNoChao && velocidadeCai.y < 0)
            {
                velocidadeCai.y = -2f;
            }

            //fazendo o personagem andar
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            // vetor para mover o personagem
            Vector3 move = (transform.right * x + transform.forward * z).normalized;
            controle.Move(move * velocidade * Time.deltaTime);

            //fazer pular
            if (Input.GetButtonDown("Jump") && estaNoChao)
            {
                velocidadeCai.y = Mathf.Sqrt(alturaPulo * -2f * gravidade);
            }
            velocidadeCai.y += gravidade * Time.deltaTime;
        
            controle.Move(velocidadeCai * Time.deltaTime);

            //chama metodo para abaixar
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                Abaixa();
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
            estaAbaixado = !estaAbaixado;
            if (estaAbaixado)
            {
                controle.height = alturaAbaixado;
                cameraTransform.localPosition = new Vector3(0, posicaoCameraAbaixado, 0);
            }
            else
            {
                controle.height = alturaLevantado;
                cameraTransform.localPosition = new Vector3(0, posicaoCameraEmPe, 0);
            }
        }

    }
}