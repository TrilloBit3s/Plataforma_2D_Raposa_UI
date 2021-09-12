using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : MonoBehaviour
{
    public float speed;//velocidade da serra
    public float moveTime;//tempo que a serra se movimenta

    private bool dirRight = true;//padrao para a direita
    private float timer;

    void Update()
    {
        if(dirRight)
        {
            //se verdadeiro vai para a direita
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
        else
        {
            //se verdadeiro vai para a direita
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }
        
        timer += Time.deltaTime;
        if(timer >= moveTime)
        {
            dirRight = !dirRight;//se verdadeiro entao false, se false então verdadeiro
            timer = 0f;
        }
    }
}