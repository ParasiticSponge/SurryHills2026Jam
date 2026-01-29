using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TutorialPage : MonoBehaviour
{
    public int number = 0;
    public static Action<int> changeNumber;
    GameObject nextButton;
    GameObject previousButton;

    Animator animator;

    private void OnEnable()
    {
        changeNumber += ChangeNumber;
    }
    private void OnDisable()
    {
        changeNumber += ChangeNumber;
    }
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        ChangeNumber(1);
        TutorialButtons[] buttons = FindObjectsOfType<TutorialButtons>();
        previousButton = buttons[0].gameObject;
        nextButton = buttons[1].gameObject;
    }
    private void Start()
    {
        previousButton.SetActive(false);
    }

    void ChangeNumber(int value)
    {
        number += value;
        animator.SetFloat("Page", number);
        switch (number)
        {
            case 1:
                animator.Play("Tutorial_1");
                break;
            case 2:
                animator.Play("Tutorial_2");
                break;
            case 3:
                animator.Play("Tutorial_3");
                break;
            case 4:
                animator.Play("Tutorial_4");
                break;
            case 5:
                animator.Play("Tutorial_End");
                break;
        }

        if (previousButton != null)
        {
            if (number == 1 && previousButton.activeSelf == true)
                previousButton.SetActive(false);
            else if (number != 1 && previousButton.activeSelf == false)
                previousButton.SetActive(true);
        }

        if (number == 5)
        {
            StartCoroutine(ExitTutorial());
        }
    }

    IEnumerator ExitTutorial()
    {
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);
        Actions.Begin.Invoke();
        gameObject.SetActive(false);
    }
}
