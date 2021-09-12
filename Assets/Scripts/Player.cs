using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public ParticleSystem dust;

    [Header("Parametros Player")]
    public float speed;
    public float jumpForce;
    private int nJump = 1;
    public Rigidbody2D rb2D;
    private float direction;
    public Animator animator;
    private int facingDirection = 1;
    private bool facingright = true;
    private bool canMove = true;
     
    [Header("Parametros Wall Slide")]
    public float wallSlideSpeed;
    private bool isTouchinWall;
    private bool isWallSliding;
    private bool Wall;

    [Header("Parametros Colisores")]
    public Transform posicaoDoPe;
    public Transform posicaoDoPe2;
    public bool isGrounded;
    public float radius;
    public LayerMask ground;
    public LayerMask wall;
    private bool isCrouching;
    public bool onWall;
    public Transform wallCheck;
    public float wallCheckDistance;
   
    [Header("Parametros Wall Jump")]
    public float wallJumpForce;//200 wallJumpForce multiplica pelo wallJumpDirection
    public Vector2 wallJumpDirection;

    [Header("Parametros do Dash")]
    private float dashAtual;
    private bool canDash = true;
    private bool isDashing;
    public float duracaoDash;
    public float dashSpeed;  
 
    [Header("Vida do Player")]
    public int Vida = 10;

    [Header("Dano do Player")]
    private float TempoDano = 0;
    private bool PodeDano = true;
    public SpriteRenderer ImagemPersonagem;

    [Header("Barra de HP")]
    private Image BarraHp;

    [Header("Itens")]
    public int Star = 0;
    private Text StarText;

    [Header("vidas do jogador")]
    private int chances = 3;
    private Text ChancesText;

    [Header("Check Point")]
    public Vector3 posInicial;//variavel com a posição inicial do player

    [HideInInspector]
    public bool usingLadder = false;

    //Controla o Jogo com o GerenciadorDoJogo
    private GerenciadorJogo GJ;

    AudioSource audioS; //tocar som

    void Start()
    {   
        audioS = gameObject.GetComponent<AudioSource>();

        //Recebe a informação do GameObject
        GJ = GameObject.FindGameObjectWithTag("GameController").GetComponent<GerenciadorJogo>();

        posInicial = new Vector3(1.7f, 2.86f, 0f);//posição no inicio do jogador no jogo
        transform.position = posInicial;//Muda a posição do Player

        dashAtual = duracaoDash;

        //Elementos do canvas
        BarraHp = GameObject.FindGameObjectWithTag("HpBarra").GetComponent<Image>();
        StarText = GameObject.FindGameObjectWithTag("starText").GetComponent<Text>();
        ChancesText = GameObject.FindGameObjectWithTag("lifesText").GetComponent<Text>();
        ChancesText.text = "Vidas: " + chances.ToString();//acrescentar vidas 
    }

    void Update()
    {
        if(GJ.EstadoDoJogo() == true)
        {   
            animator.SetFloat("speedY", rb2D.velocity.y);
            animator.SetFloat("speedX", Mathf.Abs(direction));
            animator.SetBool("isGround", isGrounded);
            animator.SetBool("isCrouching", isCrouching); 
            animator.SetBool("OnWall", onWall); 
 
            Flip();
            CheckGround();
            CheckInput(); 
            Crouch();
            Dash();
            CheckWallSliding();
            Dano();
        }
    }

    private void MovePlayer()
    {
        direction = Input.GetAxisRaw("Horizontal");//se esquerda <- = -1  ou se direita -> = 1 
        rb2D.velocity = new Vector2(direction * speed, rb2D.velocity.y); 

        if(isWallSliding)
        {
            if(rb2D.velocity.y < -wallSlideSpeed)
            {
                rb2D.velocity = new Vector2(rb2D.velocity.x, - wallSlideSpeed);//+wallSlideSpeed vai para cima
            }
        }
    }

    void Flip()
    {
        if((direction < 0 && facingright) || (direction > 0 && !facingright))
        {
            CreateDust();
            facingDirection *= -1;
            facingright = !facingright;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    void CheckGround()
    {
        //cria um circulo na posição do pé que devera encostar no chao
        isGrounded = Physics2D.OverlapCircle(posicaoDoPe.position, 0.3f, ground);//ground anula pulo infinito
        isGrounded = Physics2D.OverlapCircle(posicaoDoPe2.position, 0.3f, ground);

        //Verifica se esta encostando na parede
        isTouchinWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, wall);
    }

    void CheckInput()
    {
        if(canMove)
        {
            MovePlayer();
        }

        if(isGrounded)
        {
            nJump = 1;
        }

        if(Input.GetKeyDown(KeyCode.Space) && nJump > 0)
        {
            Jump();
        }
    }

    void Jump()
    {
        //Pulo normal
        CreateDust();
        if(nJump > 0 && !isWallSliding)
        {
            nJump--;
            rb2D.velocity = Vector2.zero;
            rb2D.velocity = Vector2.up * jumpForce;

            //para pular, se o usuario pressionar a tecla espaço
            if (isGrounded && Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                //altera a velocidade no eixo y para multiplicando pela força do pulo
                rb2D.velocity = Vector2.up * jumpForce;
            }

            if(!usingLadder)
            animator.SetFloat("speedX", rb2D.velocity.y);
        }
        //Wall Jump
        else if(isWallSliding)
        {
            // X = força * x * (-1 ou 1 - Esquerda ou Direita)
            // Y = força * Y (sempre para cima)
            Vector2 force = new Vector2(wallJumpForce * wallJumpDirection.x * -facingDirection, wallJumpForce * wallJumpDirection.y);

            //zera a velocidade antes de aplicar o proximo pulo, evita acumulos
            rb2D.velocity = Vector2.zero;

            // Adiciona a força do Wall Jump
            rb2D.AddForce(force, ForceMode2D.Impulse);

            //Retira temporariamente o controle do pertsonagem 
            StartCoroutine("StopMove");//ou StopDirection
        }
    }

    void Crouch()
    {
        isCrouching = (Input.GetKey("down")) ? true : false;//esta agachado?
        animator.SetBool("isCrouching", isCrouching);

        if(isCrouching)//cancelar andar agachado
        {
            animator.SetFloat("speedX", 0);//zera o parametro da animação
            rb2D.velocity = new Vector2(0, rb2D.velocity.y);//zera a velocidade
            StopDash();
        }
    }

    void Dash()
    {
        if(Input.GetKey(KeyCode.Q) && canDash)//if(Input.GetKey(KeyCode.Q) && isGrounded && canDash)
        {
            CreateDust();
            if(dashAtual <= 0)
            {
                StopDash();
            }
            else
            {
                isDashing = true;
                dashAtual -= Time.deltaTime;

                if(facingright)
                {
                    rb2D.velocity = Vector2.right * dashSpeed;
                    animator.SetBool("isCrouching", isCrouching);
                }
                else
                    rb2D.velocity = Vector2.left * dashSpeed;
                    animator.SetBool("isCrouching", isCrouching);
            }   
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            isDashing = false;
            canDash = true;
            dashAtual = duracaoDash;
        }
    }

    private void StopDash()
    {
        rb2D.velocity = Vector2.zero;
        dashAtual = duracaoDash;
        isDashing = false;
        canDash = false;
    }

    private void CheckWallSliding()
    {   
        if(isTouchinWall && !isGrounded && rb2D.velocity.y < 0 && direction != 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    IEnumerator StopMove()
    {
        //Retira o controle do player
        canMove = false;
        //Inverte o lado do transform
        transform.localScale = transform.localScale.x == 1 ? new Vector2(-1, 1): Vector2.one; 
        yield return new WaitForSeconds(.3f);
        //Normaliza o lado do transform
        transform.localScale = Vector2.one;
        //Devolve o controle do player
        canMove = true;
    }

    void Dano()
    {
        if(PodeDano == false)
        {
            TemporizadorDano();
        }
    }

    void OnDrawGizmos()//Mostra os Gizmos na tela para orientação 
    {
     // Gizmos.DrawWireSphere(posicaoDoPe.position, radius);//pé esquerdo
     // Gizmos.DrawWireSphere(posicaoDoPe2.position, radius);//pé direito

        //para ver a linha do Raycast
        Gizmos.color = Color.blue;
        if(facingright){
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
            }else{
                Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x - wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
                }
                
    }

    //Faz o player permanecer parado sobre a colisao da plataforma
    void OnCollisionEnter2D(Collision2D col)
    {   
        if(col.gameObject.name.Equals("Plataforma"))//O objeto deve conter o mesmo nome
            this.transform.parent = col.transform;

        if(col.gameObject.tag == "Spike")
        {
            //GerenciadorJogo.instance.ShowGameOver();
            Destroy(gameObject);
        }

        if(col.gameObject.tag == "Saw")
        {
           //GerenciadorJogo.instance.ShowGameOver();
            Destroy(gameObject);
        }
    }

    //Sair da plataforma
    void OnCollisionExit2D(Collision2D col)
    {
        if(col.gameObject.name.Equals("Plataforma"))
            this.transform.parent = null;
    }

    //Dano de Inimigo que usa a tag Enemy
    //Caso queira dano de imediato use OnCollisionEnter2D
    void OnCollisionStay2D(Collision2D other) {
        
        if(other.gameObject.tag == "Enemy")
        {
            if(PodeDano == true)
            {
                Vida--;
                animator.SetBool("hurt", true);
                PerderHP();
                PodeDano = false;
                ImagemPersonagem.color = UnityEngine.Color.red;//Muda a cor do personagem para vermelho
                
                //morrer quando a vida for menor ou = 0
                if(Vida <= 0)
                {
                    Morrer();
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D gatilho)
    {
        if(gatilho.gameObject.tag == "NextLevel")
        {
            GJ.Level2();
        }  

        if(gatilho.gameObject.tag == "NextLevel2")
        {
            GJ.Level1();
        }    

        if(gatilho.gameObject.tag == "Star")
        {
            Destroy(gatilho.gameObject);
            Star++;
            StarText.text = Star.ToString();
        }    

        if(gatilho.gameObject.tag == "CheckPoint")
        {
            posInicial = gatilho.gameObject.transform.position;
            Destroy(gatilho.gameObject);
        }

        if(gatilho.gameObject.tag == "MorteImediata")
        {
            if(PodeDano == true)
            {
                PodeDano = false;
                Vida = Vida - 20;
                PerderHP();
                Morrer();
            }
        }
    }

    //tempo em que fico tomando dano do inimigo
    void TemporizadorDano()
    {
        TempoDano += Time.deltaTime;
        if(TempoDano > 0.5f)
        {
            PodeDano = true;
            TempoDano = 0;
            ImagemPersonagem.color = UnityEngine.Color.white;//Muda a cor do personagem para a cor original
            animator.SetBool("hurt", false);
            audioS.Play();
        }
    }

    void PerderHP()
    {
        int vidaParaBarra = Vida * 10;//este numero refere-se ao tamanho da barra de vida do player 
        BarraHp.rectTransform.sizeDelta = new Vector2(vidaParaBarra, 10);
    }

    void Morrer()
    {
        chances--;
        ChancesText.text = "Vidas: " + chances.ToString();//acrescentar vidas 

        //só reiniciar quando acabarem as chances
        if(chances <= 0)
        {
           GJ.PersonagemMorreu();
        }
        else
        {
            Inicializar();
        }
    }

    void Inicializar()
    {
        transform.position = posInicial;

        //ponto inicial do player e cada game tem sua posição diferente "cada um escolhe sua posição inicial"
        //transform.position = new Vector3(1.31f, 3.04f, transform.position.z); 

        //recuperar vida
        Vida = 10;

        //retorna a cor da Barra de HP do Player
        int vidaParaBarra = Vida * 10;//este numero refere-se ao tamanho da barra de vida do player 
        BarraHp.rectTransform.sizeDelta = new Vector2(vidaParaBarra, 10);
    }

    //Particulas de poeira
    void CreateDust()
    {
        dust.Play();
    }
}