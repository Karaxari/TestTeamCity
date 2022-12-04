using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextShowEffector : MonoBehaviour
{
    [SerializeField] private float showTime = 1f;

    [SerializeField] private TMP_Text targetText;

    public void StartEffect()
    {
        targetText.maxVisibleCharacters = 0;
        StartCoroutine(ShowEffect());
    }

    private IEnumerator ShowEffect()
    {
        float timePerChar = showTime / targetText.text.Length;

        while(targetText.maxVisibleCharacters ++< targetText.text.Length)
            yield return new WaitForSeconds(timePerChar);
    }

    public void ShowInstead()
    {
        StopAllCoroutines();
        targetText.maxVisibleCharacters = targetText.text.Length;
    }
}
