using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class TopKontrol : MonoBehaviour
{

    private Rigidbody2D top;
    private SpriteRenderer topsr;

    public Color[] renkler;

    private int skor = 0;
    public TMP_Text skorYazisi;

    private int yuksekSkor = 0;
    public TMP_Text yuksekSkorYazisi;

    public AudioClip au, au1, au2, au3;

    public float ziplamaGucu;
    public float invincibilityDuration = 2.0f;
    public float flashInterval = 0.1f;
    public bool SurekliOdulluReklamIzle = false;

    private bool isNewBron;
    private bool isStopped = false;
    private bool waitingForFirstTapAfterAd = false;
    private static bool rewardedAdUsedThisSession = false;
    private Coroutine flashingCoroutine = null;
    private Color originalTopColor;

    private int lastSentHighScore = 0;
    private float timeSinceLastSend = 0f;
    private float sendInterval = 10f; // 10 saniye

    void Start()
    {
        rewardedAdUsedThisSession = false;

        top = GetComponent<Rigidbody2D>();
        topsr = GetComponent<SpriteRenderer>();
        RastgeleRenk();
        TopuDurumu(true);
        PrepareGame();

        lastSentHighScore = PlayerPrefs.GetInt("yuksekSkor", 0);
    }

    private void PrepareGame()
    {
        yuksekSkor = PlayerPrefs.GetInt("yuksekSkor", 0);
        yuksekSkorYazisi.text = yuksekSkor.ToString();

        skor = PlayerPrefs.GetInt("skor", 0);
        skorYazisi.text = skor.ToString();
    }

    void Update()
    {
        if (waitingForFirstTapAfterAd && Time.timeScale > 0 && Input.GetMouseButtonDown(0))
        {
            waitingForFirstTapAfterAd = false;
            isStopped = false;
            TopuDurumu(true);
            StartCoroutine(ActivateInvincibility());
            top.velocity = Vector2.up * ziplamaGucu;
        }
        else if (!isStopped && !waitingForFirstTapAfterAd && Time.timeScale > 0 && Input.GetMouseButton(0))
        {
            top.velocity = Vector2.up * ziplamaGucu;
        }

        // Skoru 10 saniyede bir kontrol edip PlayFab'a gönder
        timeSinceLastSend += Time.deltaTime;
        if (timeSinceLastSend >= sendInterval)
        {
            timeSinceLastSend = 0f;

            if (skor > lastSentHighScore)
            {
                lastSentHighScore = skor;

                // Yerel yüksek skoru da güncelle
                yuksekSkor = skor;
                PlayerPrefs.SetInt("yuksekSkor", yuksekSkor);
                yuksekSkorYazisi.text = yuksekSkor.ToString();

                if (PlayFabScoreManager.Instance != null)
                {
                    PlayFabScoreManager.Instance.SendHighScore(yuksekSkor);
                    Debug.Log("Yeni yüksek skor PlayFab'a gönderildi: " + yuksekSkor);
                }
            }
        }
    }



    private void OnTriggerEnter2D(Collider2D temas)
    {
        if (temas.CompareTag("RenkDegistirici"))
        {
            RastgeleRenk();
            AudioSource.PlayClipAtPoint(au, transform.position);
            Destroy(temas.gameObject);
            return;
        }

        if (temas.CompareTag("Puan"))
        {
            int artis = Random.Range(1, 11);
            skor += artis;
            skorYazisi.text = skor.ToString();

            if (skor > yuksekSkor)
            {
                yuksekSkor = skor;
                PlayerPrefs.SetInt("yuksekSkor", yuksekSkor);
                yuksekSkorYazisi.text = yuksekSkor.ToString();

                // İşte buraya ekle
                if (PlayFabScoreManager.Instance != null)
                {
                    PlayFabScoreManager.Instance.SendHighScore(yuksekSkor);
                }
            }

            AudioSource.PlayClipAtPoint(au1, transform.position);
            Destroy(temas.gameObject);
            return;
        }

        if (temas.CompareTag("Yan"))
        {
            if (AdsManager.Instance.OlunceReklamGoster && AdsManager.Instance.IsInterstitialAdReady())
            {
                AdsManager.Instance.interstitialAdClosedEvent.RemoveAllListeners();
                AdsManager.Instance.interstitialAdClosedEvent.AddListener(() => SceneManager.LoadScene(0));
                AdsManager.Instance.ShowInterstitialAd();
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }

        if (temas.CompareTag("Kenar") && temas.GetComponent<KenarRenk>().renk != topsr.color && !isNewBron)
        {
            if (isStopped) return;

            TopuDurumu(false);
            isStopped = true;

            Debug.Log("Kenar ile çarpışıldı ve renk yanlış.");

            var adsM = AdsManager.Instance;

            if (adsM.IsRewardedAdReady() && (!rewardedAdUsedThisSession || SurekliOdulluReklamIzle))
            {
                AdsFailPanel.Instance.ShowGameContinue();
                adsM.rewardEvent.RemoveAllListeners();
                adsM.rewardEvent.AddListener(() =>
                {
                    Debug.Log("Ödül alındı, dokunulmazlık başlatılacak (dokunma bekleniyor).");
                    rewardedAdUsedThisSession = true;

                    AdsFailPanel.Instance.HidePanel();

                    // Dokunma bekleniyor
                    waitingForFirstTapAfterAd = true;
                });
            }
            else if (adsM.IsInterstitialAdReady())
            {
                adsM.interstitialAdClosedEvent.RemoveAllListeners();
                adsM.interstitialAdClosedEvent.AddListener(() =>
                {
                    SceneManager.LoadScene(0);
                });
                adsM.ShowInterstitialAd();
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }

        if (temas.CompareTag("Bayrak"))
        {
            TopuDurumu(false);
            PlayerPrefs.SetInt("skor", skor);
            AudioSource.PlayClipAtPoint(au2, transform.position);

            int mevcutBolum = SceneManager.GetActiveScene().buildIndex;
            int sonrakiBolum = mevcutBolum + 1;

            var adsM = AdsManager.Instance;

            if (adsM.IsRewardedAdReady())
            {
                adsM.rewardEvent.RemoveAllListeners();
                adsM.rewardEvent.AddListener(() =>
                {
                    rewardedAdUsedThisSession = true;

                    PlayerPrefs.SetInt("SavedLevel", sonrakiBolum);
                    PlayerPrefs.SetInt("acilanLevel", sonrakiBolum);
                    PlayerPrefs.Save();

                    if (AdsFailPanel.Instance != null)
                        AdsFailPanel.Instance.HidePanel();

                    if (sonrakiBolum < SceneManager.sceneCountInBuildSettings)
                        SceneManager.LoadScene(sonrakiBolum);
                    else
                        SceneManager.LoadScene("MainMenu");
                });
                adsM.ShowRewardedAd();
            }
            else if (adsM.IsInterstitialAdReady())
            {
                adsM.interstitialAdClosedEvent.RemoveAllListeners();
                adsM.interstitialAdClosedEvent.AddListener(() =>
                {
                    rewardedAdUsedThisSession = true;

                    PlayerPrefs.SetInt("SavedLevel", sonrakiBolum);
                    PlayerPrefs.SetInt("acilanLevel", sonrakiBolum);
                    PlayerPrefs.Save();

                    if (AdsFailPanel.Instance != null)
                        AdsFailPanel.Instance.HidePanel();

                    if (sonrakiBolum < SceneManager.sceneCountInBuildSettings)
                        SceneManager.LoadScene(sonrakiBolum);
                    else
                        SceneManager.LoadScene("MainMenu");
                });
                adsM.ShowInterstitialAd();
            }
            else
            {
                SceneManager.LoadScene(mevcutBolum);
            }
        }
    }

    private IEnumerator ActivateInvincibility()
    {
        isNewBron = true;

        if (flashingCoroutine != null)
            StopCoroutine(flashingCoroutine);

        flashingCoroutine = StartCoroutine(FlashEffect());

        yield return new WaitForSeconds(invincibilityDuration);

        if (flashingCoroutine != null)
            StopCoroutine(flashingCoroutine);

        topsr.color = originalTopColor;
        isNewBron = false;
    }

    private IEnumerator FlashEffect()
    {
        Color flashColor = new Color(originalTopColor.r, originalTopColor.g, originalTopColor.b, 0.3f);
        while (true)
        {
            topsr.color = flashColor;
            yield return new WaitForSeconds(flashInterval);
            topsr.color = originalTopColor;
            yield return new WaitForSeconds(flashInterval);
        }
    }

    private void RastgeleRenk()
    {
        int rastgele = Random.Range(0, renkler.Length);
        topsr.color = renkler[rastgele];
        originalTopColor = topsr.color;
    }

    private void TopuDurumu(bool enablePhysics)
    {
        top.isKinematic = !enablePhysics;
        if (!enablePhysics)
        {
            top.velocity = Vector2.zero;
            top.angularVelocity = 0f;
        }
    }
}