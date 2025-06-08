using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuManager : MonoBehaviour
{
    
        public void YeniOyun()
    {
        PlayerPrefs.SetInt("SavedLevel", 1); // Seviye sýfýrla
        PlayerPrefs.SetInt("skor", 0);       // Skoru sýfýrla
        PlayerPrefs.Save();                  // Deðiþiklikleri kaydet

        SceneManager.LoadScene("Level 1");
    }

    public void DevamEt()
    {
        int savedLevel = PlayerPrefs.GetInt("SavedLevel", 1);
        Debug.Log("Kaydedilen seviye: Level " + savedLevel);
        SceneManager.LoadScene(savedLevel);
    }       

}