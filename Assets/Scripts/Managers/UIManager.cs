﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BASA { 
    public class UIManager : MonoBehaviour
    {

        public Slider sliderHP, sliderStamina;
        public MovimentaPersonagem scriptMovimenta;
        public Text municao;
        public Image imageModoTiro;
        public Sprite[] spriteModoTiro;
        public RectTransform mira;
             
        // Start is called before the first frame update
        void Start()
        {
            scriptMovimenta = GameObject.FindWithTag("Player").GetComponent<MovimentaPersonagem>();
            municao.enabled = true;
            imageModoTiro.enabled = true;
        }

        // Update is called once per frame
        void Update()
        {
            sliderHP.value = scriptMovimenta.hp;
            sliderStamina.value = scriptMovimenta.stamina;
        }
    }
}