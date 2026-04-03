using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fps : MonoBehaviour
{
    private static Fps instance;

    void Awake()
    {
        // Tek instance kontrolü (Singleton gibi)
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Ayný scriptten birden fazla varsa fazlalarý silinir
        }
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;

        int screenRefreshRate = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);
        Application.targetFrameRate = screenRefreshRate;

        Debug.Log("FPS ekran Hz'ine sabitlendi: " + screenRefreshRate + " FPS");
    }
}