using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slug : MonoBehaviour
{
    public float speed;//velocidade que o inimigo se movimenta

    private Rigidbody2D rb;
    private Animator anim;//manipular animação de morte do inimigo
    public Transform rightCol;//objetos para detectar colisão 
    public Transform leftCol;//objetos para detectar colisão 
    public Transform headPoint;//colisor na cabeça para saber quando o player pisou em cima

    private bool colliding;//detectar quando o inimigo colide na parede ou não

    public LayerMask inimigo;

    public BoxCollider2D boxCollider2D;
   //public BoxCollider2D boxCollider2D2;

    public float jumpForce = 4000; //700 joga o player para cima
    AudioSource audioS; //tocar som
        
    //Controla o Jogo
    private GerenciadorJogo GJ;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioS = gameObject.GetComponent<AudioSource>();

                //Recebe a informação do GameObject
        GJ = GameObject.FindGameObjectWithTag("GameController").GetComponent<GerenciadorJogo>();
    }

    void Update()
    {
        if(GJ.EstadoDoJogo() == true)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);//nao altera o eixo Y
        
            colliding = Physics2D.Linecast(rightCol.position, leftCol.position, inimigo);//detecta se bateu em algo

            if(colliding)//toda vez que o inimigo colide ele vira
            {
                transform.localScale = new Vector2(transform.localScale.x * -1f, transform.localScale.y);//rotacionando o inimigo
                speed *= -1;
            }
        }
    }

    bool playerDestroyed = false;
    void OnCollisionEnter2D(Collision2D col)// 
    {
        if (col.gameObject.tag == "Player")
        {
            audioS.Play();
            float height = col.contacts[0].point.y - headPoint.position.y;
           //Debug.Log(height);//se for Negativo Destroi o Player, se for positivo Destroi o Inimigo
            if (height > 0 && !playerDestroyed)
            {
                //col.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 4, ForceMode2D.Impulse);
                speed = 0;
                anim.SetTrigger("TriggerDie");
                boxCollider2D.enabled = false;
                //boxCollider2D2.enabled = false;
                rb.bodyType = RigidbodyType2D.Kinematic;
                //Debug.Log("matou o Inimigo");

                col.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce));

                Destroy(gameObject, 0.25f);
            }/*else
            {
                playerDestroyed = true;
                //GameController.instance.ShowGameOver();
                Destroy(col.gameObject);
                //Debug.Log("matou o jogador");
            }*/
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
         if (col.gameObject.CompareTag("Player"))
        {
            audioS.Play();
            BoxCollider2D[] boxes = gameObject.GetComponents<BoxCollider2D>();
            foreach(BoxCollider2D box in boxes)
            {
                box.enabled = false;
            }

            col.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            col.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce));

            speed = 0;
            transform.Rotate(new Vector3(0, 0, -180));
            Destroy(gameObject, 3);
        }
    }
}