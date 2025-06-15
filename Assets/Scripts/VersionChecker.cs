using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class VersionChecker : MonoBehaviour
{
    public string versionUrl = "https://raw.githubusercontent.com/CanKutun/ColorCember2/main/version.txt";

    // G�ncelleme paneli ve butonu (Unity'de inspector'dan atay�n)
    public GameObject updatePanel;
    public Button updateButton;

    void Start()
    {
        Debug.Log("VersionChecker: Start() �al��t�");
        updatePanel.SetActive(false);  // Ba�lang��ta panel kapal� olsun
        StartCoroutine(CheckVersion());
    }

    IEnumerator CheckVersion()
    {
        Debug.Log("VersionChecker: Versiyon kontrol� ba�lat�ld�.");
        UnityWebRequest www = UnityWebRequest.Get(versionUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("VersionChecker: Versiyon kontrol hatas� - " + www.error);
        }
        else
        {
            string latestVersion = www.downloadHandler.text.Trim();
            Debug.Log("VersionChecker: Sunucudan gelen versiyon = '" + latestVersion + "'");

            string currentVersion = Application.version;
            Debug.Log("VersionChecker: Uygulamadaki versiyon = '" + currentVersion + "'");

            try
            {
                // Versiyonlar� System.Version olarak parse et, kar��la�t�r
                Version currentVer = new Version(currentVersion);
                Version latestVer = new Version(latestVersion);

                if (currentVer.CompareTo(latestVer) < 0)
                {
                    Debug.Log("VersionChecker: G�ncelleme mevcut. G�ncelleme paneli a��l�yor.");
                    updatePanel.SetActive(true);

                    // �nceki listener'lar� temizle, sonra butona yeni listener ekle
                    updateButton.onClick.RemoveAllListeners();
                    updateButton.onClick.AddListener(OpenStorePage);
                }
                else
                {
                    Debug.Log("VersionChecker: Uygulama g�ncel.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("VersionChecker: Versiyon kar��la�t�rma hatas� - " + e.Message);
            }
        }
    }

    public void OpenStorePage()
    {
        Debug.Log("VersionChecker: Play Store a��l�yor.");
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.kutuns");
    }
}