using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyScript : MonoBehaviour
{
    [SerializeField] Sprite gameBackground;
    [SerializeField] SpriteRenderer bg;

    float startValue = 0;
    float endValue = 255;

    [SerializeField] float timeElasped = 0f;
    [SerializeField] float duration = 1f;

    private void Start()
    {
        timeElasped = 0f;
        bg.sprite = gameBackground;
    }


    private void Update()
    {
        if (timeElasped <= duration)
        {
            Debug.Log($"{timeElasped}/{duration}");
            float value = Mathf.Lerp(startValue, endValue, timeElasped / duration);
            Debug.Log(value);
            bg.color = new Color32(255, 255, 255, (byte)value);
        }

        timeElasped += Time.deltaTime;
    }
}
