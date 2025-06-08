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

    public AudioClip au, au1, au2;

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

    void Start()
    {
        rewardedAdUsedThisSession = false;

        top = GetComponent<Rigidbody2D>();
        topsr = GetComponent<SpriteRenderer>();
        RastgeleRenk();
        TopuDurumu(true);
        PrepareGame();
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
            top.velocity = Vector2.up * ziplamaGucu;
        }
        else if (!isStopped && !waitingForFirstTapAfterAd && Time.timeScale > 0 && Input.GetMouseButton(0))
        {
            top.velocity = Vector2.up * ziplamaGucu;
        }
    }

    private void OnTriggerEnter2D(Collider2D temas)
    {
        if (temas.CompareTag("Yan"))
        {
            if (AdsManager.Instance.OlunceReklamGoster)
            {
                if (AdsManager.Instance.IsInterstitialAdReady())
                {
                    var adsM = AdsManager.Instance;
                    adsM.interstitialAdClosedEvent.RemoveAllListeners();
                    adsM.interstitialAdClosedEvent.AddListener(() => SceneManager.LoadScene(0));
                    adsM.ShowInterstitialAd();
                }
                else
                {
                    SceneManager.LoadScene(0);
                }
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

            // **Burada skor sıfırlanmıyor, oyuncu kaldığı skorla devam edecek**
            // PlayerPrefs.DeleteKey("skor");  <-- BU SATIRI KALDIRDIK!

            var adsM = AdsManager.Instance;
            Debug.Log("Kenar ile çarpışıldı ve renk yanlış.");

            if (adsM.IsRewardedAdReady() && (!rewardedAdUsedThisSession || SurekliOdulluReklamIzle))
            {
                AdsFailPanel.Instance.ShowGameContinue();
                adsM.rewardEvent.RemoveAllListeners();
                adsM.rewardEvent.AddListener(() =>
                {
                    Debug.Log("Ödül alındı, dokunulmazlık başlıyor.");
                    rewardedAdUsedThisSession = true;

                    AdsFailPanel.Instance.HidePanel();

                    StartCoroutine(ActivateInvincibility());
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

        if (temas.CompareTag("RenkDegistirici"))
        {
            RastgeleRenk();
            AudioSource.PlayClipAtPoint(au1, transform.position);
            Destroy(temas.gameObject);
        }

        if (temas.CompareTag("Yildiz"))
        {
            skor += Random.Range(5, 11);
            skorYazisi.text = skor.ToString();
            AudioSource.PlayClipAtPoint(au, transform.position);
            Destroy(temas.gameObject);

            if (skor > yuksekSkor)
            {
                yuksekSkor = skor;
                yuksekSkorYazisi.text = yuksekSkor.ToString();
                PlayerPrefs.SetInt("yuksekSkor", yuksekSkor);
            }
            PlayerPrefs.SetInt("skor", skor); // Her puan artışında kaydet
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
                    Debug.Log("Ödül alındı, sonraki bölüme geçiliyor...");
                    rewardedAdUsedThisSession = true;

                    // ❗️Kayıt sadece reklamdan sonra yapılır:
                    PlayerPrefs.SetInt("SavedLevel", sonrakiBolum);
                    PlayerPrefs.SetInt("acilanLevel", sonrakiBolum);
                    PlayerPrefs.Save();
                    Debug.Log("Kayıt Edilen Level: " + sonrakiBolum);

                    if (AdsFailPanel.Instance != null)
                        AdsFailPanel.Instance.HidePanel();

                    if (sonrakiBolum < SceneManager.sceneCountInBuildSettings)
                    {
                        SceneManager.LoadScene(sonrakiBolum);
                    }
                    else
                    {
                        Debug.Log("Son sahneye ulaşıldı. Ana menüye dönülüyor...");
                        SceneManager.LoadScene("MainMenu");
                    }
                });

                adsM.ShowRewardedAd();
            }
            else
            {
                Debug.LogWarning("Ödüllü reklam hazır değil, geçiş engellendi.");

                if (AdsFailPanel.Instance != null)
                {
                    AdsFailPanel.Instance.ShowFaildPanel();
                }

                // Reklam yoksa kayıt yapılmaz, bölüm geçilmez.
            }
        }



        IEnumerator ActivateInvincibility()
    {
        waitingForFirstTapAfterAd = true;
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

    IEnumerator FlashEffect()
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