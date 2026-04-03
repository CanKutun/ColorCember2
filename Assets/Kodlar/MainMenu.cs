using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioSource au;

    public void SesKapa()
    {
        au.volume = 0;
    }
    public void SesAc()
    {
        au.volume = 0.1f;
    }
    public void Gecis() 
    {
        SceneManager.LoadScene(1);
    }

    public void Cikis()
    {
        Application.Quit();
    }
}