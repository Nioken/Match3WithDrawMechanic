using System.Collections.Generic;
using UnityEngine;

public class QuestsManager : MonoBehaviour
{
    [SerializeField] private GameObject _scoreQuestPrefab;
    [SerializeField] private GameObject _itemQuestPrefab;
    [SerializeField] private GameObject _bariersQuestPrefab;
    [SerializeField] private GameObject _uiCanvas;
    public static QuestsManager _questsManager;
    public static List<Quest> Quests = new List<Quest>();

    private void OnEnable()
    {
        _questsManager = GetComponent<QuestsManager>();
    }
   
    public static void SpawnScoreQuest(int maxProgress)
    {
        Quests.Add(Instantiate(_questsManager._scoreQuestPrefab, _questsManager._uiCanvas.transform.GetChild(1))
            .GetComponent<Quest>());
        Quests[Quests.Count - 1].Type = Quest.QuestType.ScoreQuest;
        Quests[Quests.Count - 1].MaxProgress = maxProgress;
        Quests[Quests.Count - 1].GetComponent<RectTransform>().anchoredPosition = 
            new Vector2(10, 640 - (110 * Quests.Count - 2));
    }  
    
    public static void UpdateScoreProgress(int progress)
    {
        for (int i = 0; i < Quests.Count; i++)
        {
            if (Quests[i].Type == Quest.QuestType.ScoreQuest)
                Quests[i].UpdateQuestProgress(progress);
        }
    }
    
    public static void SpawnItemQuest(int maxProgress)
    {
        Quests.Add(Instantiate(_questsManager._itemQuestPrefab, _questsManager._uiCanvas.transform.GetChild(1))
            .GetComponent<Quest>());
        Quests[Quests.Count - 1].Type = Quest.QuestType.ItemQuest;
        Quests[Quests.Count - 1].MaxProgress = maxProgress;
        Quests[Quests.Count - 1].GetComponent<RectTransform>().anchoredPosition = 
            new Vector2(10, 640 - (110 * Quests.Count - 2));
    }

    public static void UpdateItemProgress(Item item)
    {
        for (int i = 0; i < Quests.Count; i++)
        {
            if (Quests[i].Type == Quest.QuestType.ItemQuest && Quests[i].TagToCount == item.tag)
                Quests[i].UpdateQuestProgress(1);
        }
    }

    public static void SpawnBariersQuest()
    {
        Quests.Add(Instantiate(_questsManager._bariersQuestPrefab, _questsManager._uiCanvas.transform.GetChild(1))
            .GetComponent<Quest>());
        Quests[Quests.Count - 1].Type = Quest.QuestType.BarrierQuest;
        Quests[Quests.Count - 1].GetComponent<RectTransform>().anchoredPosition =
            new Vector2(10, 640 - (110 * Quests.Count - 2));
    }
    
    public static void UpdateBarrierProgress(Barrier barrier)
    {
        for (int i = 0; i < Quests.Count; i++)
        {
            if (Quests[i].Type == Quest.QuestType.BarrierQuest && Quests[i].BarrierToCount == barrier.Type)
                Quests[i].UpdateQuestProgress(1);
        }
    }

    public static bool isQuestsCompleted()
    {
        if(Quests.Count <= 0) return false;
        for(int i = 0; i < Quests.Count; i++)
        {
            if (!Quests[i].Complete) return false;
        }
        return true;
    }
}
