using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformHorizontal : MonoBehaviour
{
    private bool moveRigth = true;
    
    public float velocidade = 3f;
    public Transform pontoA;
    public Transform pontoB;

    void Update()
    {
        if(transform.position.x < pontoA.position.x)
            moveRigth = true;
        if(transform.position.x > pontoB.position.x)
            moveRigth = false;

        if(moveRigth)
            transform.position = new Vector2(transform.position.x + velocidade * Time.deltaTime, transform.position.y);
        else
            transform.position = new Vector2(transform.position.x - velocidade * Time.deltaTime, transform.position.y);    
    }
}