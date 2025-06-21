using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Final : MonoBehaviour
{

    public GameObject final;

    

    public void Yeniden()
    {
        PlayerPrefs.SetInt("skor", 0); // Skoru sýfýrla
        SceneManager.LoadScene(0);
        
    }

    public void MenuDonus()
    {
        
        SceneManager.LoadScene(0);

    }

    public void Pause()
    {
        
        Time.timeScale = 0f;

    }

    public void Devam()
    {

        Time.timeScale = 1f;

    }

    public void Menu()
    {

        SceneManager.LoadScene(0);

    }

}
