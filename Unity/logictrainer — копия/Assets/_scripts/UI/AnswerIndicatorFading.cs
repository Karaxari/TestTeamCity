using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AnswerIndicatorFading : MonoBehaviour
{
    [SerializeField] private Sprite correct;
    [SerializeField] private Sprite wrong;

    private Image img;
    private Animator animator;

    private void Start()
    {
        img = GetComponent<Image>();
        animator = GetComponent<Animator>();

        gameObject.SetActive(false);
    }

    public void Show(bool isAnswerCorrect)
    {
        gameObject.SetActive(true);

        img.sprite = isAnswerCorrect ? correct : wrong;

        animator.Play("FadeInIndicator", 0, 0f);
    }

    public void OnFaded()
    {
        gameObject.SetActive(false);
    }
}
