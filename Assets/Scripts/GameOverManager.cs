using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public int skor = 1234; // oyun sonunda gelen skor

    void Start()
    {
        // Skoru ve ülkeyi otomatik gönder
        PlayFabManager.Instance.SendScore(skor);
    }
}