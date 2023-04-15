using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BallToSend : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI numberUI;
    SpriteRenderer sprite;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void UpdateUI(int number,Color color)
    {
        numberUI.text = number.ToString();
        sprite.color = color;
    }
}
