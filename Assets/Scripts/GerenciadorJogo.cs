using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GerenciadorJogo : MonoBehaviour
{
    public bool GameLigado = false;  //verifica se o jogo esta ligado ou nao
    public GameObject TelaGameOver; //Chama tela de Game Over
  
    void Start()
    {   
        //Sempre que for otimizar o jogo, deixar GameLigado = true e timeScale = 1
        //Quando não for editar retornar os valores para GameLigado = false e timeScale = 0
       
        //Pausa os Scripts 
        GameLigado = true;//valor original é "false"
        //Pausa Fisica do jogo
        Time.timeScale = 1;//valor original é "0"
    }

    public bool EstadoDoJogo()
    {
        return GameLigado;
    }

    public void LigarJogo()
    {
        GameLigado = true;
        Time.timeScale = 1;
    }

    public void PersonagemMorreu()
    {    
        TelaGameOver.SetActive(true);//Chamar tela de GameOver
        GameLigado = false;
        Time.timeScale = 0;//0
    }

    //Reiniciar Jogo
    public void Reiniciar()
    {
        SceneManager.LoadScene(1);
    }

    public void Level2()
    {
        SceneManager.LoadScene(2);//a numeração voce encontra os leveis em "File" "Biuld Settings" "Scene in Biuld" 
    }

    public void Level1()
    {
        SceneManager.LoadScene(1);//a numeração voce encontra os leveis em "File" "Biuld Settings" "Scene in Biuld" 
    }

    //Resolver este problema e excluir pois ja existe um GAMEOVER
    public void ShowGameOver()
    {
        TelaGameOver.SetActive(true);
    }
}