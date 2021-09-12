using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformContadorCair : MonoBehaviour
{
    public Rigidbody2D rb;
    public float tempo = 2f;

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.name.Equals("Player"))
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.mass = 50f;
            rb.gravityScale = 0.5f;
            Destroy(gameObject, 03f);
        }
    }        
}
