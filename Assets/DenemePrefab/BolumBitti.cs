using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BolumBitti : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public float fadeDuration = 1f;
    public float visibleDuration = 1.5f;

    private Color targetColor;

    void Start()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        levelText.text = "Level " + currentLevel;

        targetColor = GetColorForLevel(currentLevel); // Rengi sahneye göre seç
        levelText.color = new Color(targetColor.r, targetColor.g, targetColor.b, 0); // Baþta saydam

        StartCoroutine(FadeTextInAndOut());
    }

    Color GetColorForLevel(int level)
    {
        // Ýstediðin renkleri buraya gir (sahneye göre)
        switch (level % 4)
        {
            case 1: return Color.green;
            case 2: return Color.yellow;
            case 3: return Color.blue;
            case 0: return Color.red;
            default: return Color.white;
        }
    }

    System.Collections.IEnumerator FadeTextInAndOut()
    {
        // Fade in
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            levelText.color = new Color(targetColor.r, targetColor.g, targetColor.b, alpha);
            yield return null;
        }

        // Sabit görünür bekle
        yield return new WaitForSeconds(visibleDuration);

        // Fade out
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            levelText.color = new Color(targetColor.r, targetColor.g, targetColor.b, alpha);
            yield return null;
        }

        levelText.gameObject.SetActive(false);
    }
}