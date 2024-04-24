using DG.Tweening;
using Fungus;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelBtnManager : MonoBehaviour
{
    [SerializeField] Image[] LevelImages;
    [SerializeField] string[] LevelNames;
    [SerializeField]
    GameObject[] LevelFonts;
    [SerializeField] Sprite[] FontSprite;

    [SerializeField] Button exitButton;

    [SerializeField]public float duration = 0.3f;  // 动画持续时间
    [SerializeField]public Vector3 scaleTo = new Vector3(1.2f, 1.2f, 1.2f);  // 放大到的目标尺寸

    [SerializeField] public GameObject cover;

    private Flowchart flowchart;
    void Start()
    {
        flowchart = GameObject.Find("FlowChart").GetComponent<Flowchart>();
        cover = GameObject.Find("/Canvas/Cover");
        BlockSignals.OnBlockEnd += OnBlockEnd;
        for (int i = 0; i < LevelImages.Length; i++)
        {
            AddHoverEffect(LevelImages[i].gameObject, LevelNames[i]);
        }
        exitButton.onClick.AddListener(() => Exit());
        flowchart.ExecuteBlock("EnterLevelPick");

        CheckLevelUnlock();
    }

    void CheckLevelUnlock()
    {
        int index = 0;
        for (int i = 1; i <= 4; i++)
        {
            string query = "FirstLevel" + i.ToString();
            if (flowchart.GetBooleanVariable(query)) break;
            else index++;
        }
        if(index == 4 && !flowchart.GetBooleanVariable("Completed"))
        {
            flowchart.ExecuteBlock("AllComplete");
            flowchart.SetBooleanVariable("Completed", true);
        }
        for (int i = 1; i <= 4; i++)
        {
            if (i <= index)
            {
                LevelFonts[i].GetComponent<SpriteRenderer>().sprite = FontSprite[i];
                LevelImages[i].gameObject.SetActive(true);
            }
            else
            {
                LevelFonts[i].GetComponent<SpriteRenderer>().sprite = FontSprite[4];
                LevelImages[i].gameObject.SetActive(false);
            }
        }
    }

    void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    void Exit()
    {
        Application.Quit();
    }

    void AddHoverEffect(GameObject uiElement, string levelName)
    {
        EventTrigger eventTrigger = uiElement.AddComponent<EventTrigger>();

        // 添加鼠标进入事件
        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
        pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
        pointerEnterEntry.callback.AddListener((eventData) => {
            //uiElement.transform.DOScale(scaleTo, duration).SetEase(Ease.InOutSine);
            uiElement.GetComponent<CanvasGroup>().DOFade(0.2f, duration).SetEase(Ease.InOutSine);
        });
        eventTrigger.triggers.Add(pointerEnterEntry);

        // 添加鼠标离开事件
        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
        pointerExitEntry.eventID = EventTriggerType.PointerExit;
        pointerExitEntry.callback.AddListener((eventData) => {
            //uiElement.transform.DOScale(Vector3.one, duration).SetEase(Ease.InOutSine);
            uiElement.GetComponent<CanvasGroup>().DOFade(0f, duration).SetEase(Ease.InOutSine);
        });
        eventTrigger.triggers.Add(pointerExitEntry);

        // 添加鼠标点击事件
        EventTrigger.Entry pointerClickEntry = new EventTrigger.Entry();
        pointerClickEntry.eventID = EventTriggerType.PointerClick;
        pointerClickEntry.callback.AddListener((eventData) => {
            SoundManager.Instance.PlayClickSound();
            LoadLevel(levelName);
        });
        eventTrigger.triggers.Add(pointerClickEntry);
    }

    private void OnBlockEnd(Block block)
    {
        if (cover == null || flowchart == null) return;
        if (block == flowchart.FindBlock("EnterLevelPick") && (bool)(flowchart.GetPublicVariables()[0]))
        {
            flowchart.ExecuteBlock("Introduce");
            cover.GetComponent<CanvasGroup>().DOFade(0.95f, 2f).SetEase(Ease.InOutSine);
        }
        else if(block == flowchart.FindBlock("Introduce") && (bool)(flowchart.GetPublicVariables()[0]))
            cover.SetActive(false);
    }
}
