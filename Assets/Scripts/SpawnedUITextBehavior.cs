using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnedUITextBehavior : MonoBehaviour
{

    CanvasGroup _cs;
    [SerializeField] float fadeTime = 4f;
    

    // Start is called before the first frame update
    void Start()
    {
        _cs = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        _cs.alpha -= 1 / fadeTime * Time.deltaTime;
        if(_cs.alpha <= 0) gameObject.SetActive(false);
    }
}
