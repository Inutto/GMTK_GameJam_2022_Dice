using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeSelfUITextBehavior : MonoBehaviour
{

    CanvasGroup _cs;
    [SerializeField] float fadeDelay = 0f;
    [SerializeField] float fadeTime = 4f;
    [SerializeField] bool startFade = false;

    float _fadeDelayTimer;
    

    // Start is called before the first frame update
    void Start()
    {
        _cs = GetComponent<CanvasGroup>();
        _fadeDelayTimer = fadeDelay;
    }

    // Update is called once per frame
    void Update()
    {
        _fadeDelayTimer -= Time.deltaTime;

        if (startFade && _fadeDelayTimer <= 0)
        {
            _cs.alpha -= 1 / fadeTime * Time.deltaTime;
            if (_cs.alpha <= 0) Destroy(this.gameObject);
        }
       
    }

    public void StartFade()
    {
        startFade = true;
    }
}
