using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class VersionChecker : MonoBehaviour
{
    public string versionUrl = "https://raw.githubusercontent.com/CanKutun/ColorCember2/main/version.txt";
    public GameObject updatePanel;
    public Button updateButton;

    void Start()
    {
        Debug.Log("VersionChecker: Start() çalýþtý");
        updatePanel.SetActive(false);
        StartCoroutine(CheckVersion());
    }

    IEnumerator CheckVersion()
    {
        Debug.Log("VersionChecker: Versiyon kontrolü baþlatýldý.");
        UnityWebRequest www = UnityWebRequest.Get(versionUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("VersionChecker: Versiyon kontrol hatasý - " + www.error);
        }
        else
        {
            string latestVersion = www.downloadHandler.text.Trim();
            Debug.Log("VersionChecker: Sunucudan gelen versiyon = " + latestVersion);

            string currentVersion = Application.version;
            Debug.Log("VersionChecker: Uygulamadaki versiyon = " + currentVersion);

            if (currentVersion != latestVersion)
            {
                Debug.Log("VersionChecker: Güncelleme mevcut. Güncelleme paneli açýlýyor.");
                updatePanel.SetActive(true);
                updateButton.onClick.RemoveAllListeners();
                updateButton.onClick.AddListener(OpenStorePage);
            }
            else
            {
                Debug.Log("VersionChecker: Uygulama güncel.");
            }
        }
    }

    public void OpenStorePage()
    {
        Debug.Log("VersionChecker: Play Store açýlýyor.");
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.kutuns");
    }
}