using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotacaoCamera : MonoBehaviour
{
    [Tooltip("sensibilidade de movimento do mouse")]
    public float sensibilidadeMouse = 100f;
    [Tooltip("angulo de movimentação da cabeça")]
    public float anguloMin = -45f;
    [Tooltip("angulo de movimentação da cabeça")]
    public float anguloMax = 45f;// angulos em que a camera deve travar

    public Transform transformPlayer;

    float rotacao = 0f;

         

    // Start is called before the first frame update
    void Start()
    {
        // faz mouse desaparecer
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadeMouse * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadeMouse * Time.deltaTime;

        // rotacionar camera pra cima e para baixo
        rotacao -= mouseY;
        rotacao = Mathf.Clamp(rotacao, anguloMin, anguloMax);
        transform.localRotation = Quaternion.Euler(rotacao, 0, 0);

        //mover camera para os lados
        transformPlayer.Rotate(Vector3.up * mouseX);
    }
}
