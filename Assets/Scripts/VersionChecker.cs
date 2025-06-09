using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class VersionChecker : MonoBehaviour
{
    public string versionUrl = "https://raw.githubusercontent.com/CanKutun/ColorCember2/main/version.txt";

    public GameObject updatePanel;  // G�ncelleme paneli
    public Button updateButton;     // G�ncelleme butonu

    private bool hasOpenedStore = false; // �ift a��lmay� �nler

    void Start()
    {
        updatePanel.SetActive(false); // Ba�lang��ta kapal�
        StartCoroutine(CheckVersion());
    }

    IEnumerator CheckVersion()
    {
        UnityWebRequest www = UnityWebRequest.Get(versionUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Version check failed: " + www.error);
        }
        else
        {
            string latestVersion = www.downloadHandler.text.Trim();
            Debug.Log("Latest version from server: " + latestVersion);

            string currentVersion = Application.version;
            Debug.Log("Current app version: " + currentVersion);

            if (currentVersion != latestVersion)
            {
                Debug.Log("Update available!");
                updatePanel.SetActive(true);
                updateButton.onClick.RemoveAllListeners(); // Eski listener'lar� temizle
                updateButton.onClick.AddListener(OpenStorePage); // Yeniden ba�la
            }
            else
            {
                Debug.Log("App is up to date.");
            }
        }
    }

    public void OpenStorePage()
    {
        if (hasOpenedStore) return; // Daha �nce a��ld�ysa tekrar a�maz
        hasOpenedStore = true;
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.kutuns");
    }
}