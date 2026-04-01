using System;
using UnityEngine;
public class Vegetable : MonoBehaviour
{
    private Drag drag;
    private Rigidbody2D rb; 
    private static int numberOfDrops=0;
    public float radiusOffset = 0f;

   // public static event Action GameIsOver;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.simulated = false;
    }

    public void Initialize(Drag _drag)
    {
        drag = _drag;
        if (drag != null)
        {
            drag.WhileDrag += Move;
            drag.OnDragFinished += Drop;
        }
    }

    private void Move()
    {
        transform.position = transform.parent.position;
    }

    private void Drop()
    {
        numberOfDrops++;
        if ( numberOfDrops!= 0 && numberOfDrops % 70 == 0)
        {
            // Реклама
            AdsManager.Instance.interstitialAds.ShowInterstitialAd();
        }
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        
        rb.simulated = true;
        AudioManager.Instance.PlayDropSound();
        if (drag != null)
        {
            drag.WhileDrag -= Move;
            drag.OnDragFinished -= Drop;
        }
        this.enabled = false;
      
    }
}
