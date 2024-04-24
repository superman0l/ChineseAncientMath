using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level2_ButtonManager : MonoBehaviour
{
    [SerializeField] Button AddButton;
    [SerializeField] Button MinusButton;
    [SerializeField] Player player;

    void Start()
    {
        AddButton.onClick.AddListener(() => OpeButtonClicked(true));
        MinusButton.onClick.AddListener(() => OpeButtonClicked(false));
    }

    void OpeButtonClicked(bool ope)
    {
        SoundManager.Instance.PlayClickSound();

        if (player.isMoving) return;
        AbacusManager.Instance.AbacusChange(ope);
    }
}
