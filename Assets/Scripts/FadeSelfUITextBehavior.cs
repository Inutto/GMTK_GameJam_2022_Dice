using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeSelfUITextBehavior : MonoBehaviour
{

    CanvasGroup _cs;
    [SerializeField] float fadeTime = 4f;
    [SerializeField] bool startFade = false;
    

    // Start is called before the first frame update
    void Start()
    {
        _cs = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (startFade)
        {
            _cs.alpha -= 1 / fadeTime * Time.deltaTime;
            if (_cs.alpha <= 0) gameObject.SetActive(false);
        }
       
    }

    public void StartFade()
    {
        startFade = true;
    }
}
