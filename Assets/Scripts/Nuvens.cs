using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nuvens : MonoBehaviour
{
    //mover a nuvem contrario ao player
    public float velocidade = 0;
    public GameObject Jogador;
    
    //Controla o Jogo
    private GerenciadorJogo GJ;

    void Start()
    {
        //Recebe a informação do GameObject
        GJ = GameObject.FindGameObjectWithTag("GameController").GetComponent<GerenciadorJogo>();
    }

    void Update()
    {
        if(GJ.EstadoDoJogo() == true)
        {
            NuvenVelocidade();
        }
    }

    void NuvenVelocidade()
    {
        transform.position = new Vector3(transform.position.x + velocidade, transform.position.y, transform.position.z);

        if(transform.position.x  < Jogador.transform.position.x - 20)
        {
            float posY = Random.RandomRange(6.9f, 9.8f);//posição minima e maxima da nuvem aleatoriamente no transform position Y da nuvem 
            transform.position = new Vector3(Jogador.transform.position.x + 20, posY, transform.position.z);
            Destroy(gameObject);
        }
    }
}