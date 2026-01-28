using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    GameManager gameManager;
    public Gradient gradient;
    public Image fill;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        Slider slider = GetComponent<Slider>();
        slider.maxValue = gameManager.health;
        slider.value = gameManager.health;

        fill = transform.GetChild(0).GetComponent<Image>();
        fill.color = gradient.Evaluate(1f);
    }
}
