using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSelfDestruct : MonoBehaviour
{
    public float life;

    float _timer;

    private void Awake()
    {
        _timer = life;
    }

    private void FixedUpdate()
    {
        _timer -= Time.fixedDeltaTime;
        if (_timer <= 0)
            Destroy(gameObject);
    }
}
