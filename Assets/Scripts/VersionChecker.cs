using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class VersionChecker : MonoBehaviour
{
    public string versionUrl = "https://raw.githubusercontent.com/CanKutun/ColorCember2/main/version.txt";

    public GameObject updatePanel;  // Güncelleme paneli
    public Button updateButton;     // Güncelleme butonu

    private bool hasOpenedStore = false; // Çift açýlmayý önler

    void Start()
    {
        updatePanel.SetActive(false); // Baþlangýçta kapalý
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
                updateButton.onClick.RemoveAllListeners(); // Eski listener'larý temizle
                updateButton.onClick.AddListener(OpenStorePage); // Yeniden baðla
            }
            else
            {
                Debug.Log("App is up to date.");
            }
        }
    }

    public void OpenStorePage()
    {
        if (hasOpenedStore) return; // Daha önce açýldýysa tekrar açmaz
        hasOpenedStore = true;
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.kutuns");
    }
}