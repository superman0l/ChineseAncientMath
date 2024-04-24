using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelBtnUI : MonoBehaviour
{
    private Button button;
    [SerializeField] string LevelPickScene;
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => BackToPicker());
    }

    void BackToPicker()
    {
        SceneManager.LoadScene(LevelPickScene);
    }
}
