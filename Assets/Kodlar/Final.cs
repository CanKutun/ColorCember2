using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Final : MonoBehaviour
{

    public GameObject final;

    

    public void Yeniden()
    {
        PlayerPrefs.SetInt("skor", 0); // Skoru s�f�rla
        SceneManager.LoadScene(0);
        
    }

    public void MenuDonus()
    {
        
        SceneManager.LoadScene(0);

    }


}
