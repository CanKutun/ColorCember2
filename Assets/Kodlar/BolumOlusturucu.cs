using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BolumOlusturucu : MonoBehaviour
{


    public GameObject[] objeler;
    public GameObject renkDegistirici, yildiz,bayrak; // Yazım hatası düzeltildi ve bayrak eklendi
    public int objeSayisi;
    public float objeMesafesi;
    
    void Start()
    {
        
        Vector2 pozisyon1 = new Vector2(0,0); // cember, yildiz  ve benzeri
        Vector2 pozisyon2 = new Vector2(0, 3.5f); // renk deðiþtirici

        for (int i = 0; i < objeSayisi; i++)
        {

            int objeKodu = Random.Range(0,objeler.Length);

            Instantiate(objeler[objeKodu], pozisyon1, Quaternion.identity);
            // Yıldızı engelin biraz altına yerleştir (örneğin 0.5 birim)
            Instantiate(yildiz, pozisyon1 - new Vector2(0, 0f), Quaternion.identity); 
            Instantiate(renkDegistirici, pozisyon2, Quaternion.identity); // Yazım hatası düzeltildi

            pozisyon1.y += objeMesafesi;
            pozisyon2.y += objeMesafesi;

        }

        // Bayrak oluşturma kodunu aktif et. Pozisyonu son renk değiştiricinin biraz altına ayarla.
        Instantiate(bayrak, new Vector2(0, pozisyon2.y - objeMesafesi + 2.3f), Quaternion.identity); 

    }

   
    
}
