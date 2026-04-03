using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class AdsFailPanel : Singleton<AdsFailPanel>
{
    public Button comeMenuButton, continueButton; // Restart butonu
    public TextMeshProUGUI panelStatusText;
    private GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        panel = transform.GetChild(0).gameObject;
        if (comeMenuButton != null)
        {
            comeMenuButton.onClick.AddListener(RestartLevel);
            continueButton.onClick.AddListener(() => AdsManager.Instance.ShowRewardedAd());
        }
    }
    public void ShowGameContinue()
    {
        OpenPanel();
        panelStatusText.text = "Devam Etmek İstyormusunuz?";
        comeMenuButton?.gameObject.SetActive(true);
        continueButton?.gameObject.SetActive(true);

    }

    public void ShowFaildPanel()
    {
        OpenPanel();
        panelStatusText.text = "Reklamlar Yüklenemedi!";
        comeMenuButton?.gameObject.SetActive(true);
        continueButton?.gameObject.SetActive(false);
    }
    private void RestartLevel()
    {
        ClosePanel();
        // Mevcut sahneyi yeniden yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // parantez içindeki sahneye döner
        //SceneManager.LoadScene(1);
    }

    private void ClosePanel()
    {
        panel.SetActive(false);
    }

    public void OpenPanel()
    {
        panel.SetActive(true);
    }

    public void HidePanel()
    {
        ClosePanel();
    }
}
