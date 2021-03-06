using System.Collections.Generic;
using UnityEngine;

public class QuestsManager : MonoBehaviour
{
    [SerializeField] private GameObject _scoreQuestPrefab;
    [SerializeField] private GameObject _itemQuestPrefab;
    [SerializeField] private GameObject _bariersQuestPrefab;
    [SerializeField] private GameObject _uiCanvas;

    public const int ScoreProgressMultiplyer = 8;
    public const int ItemsProgressMultiplyer = 2;

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
        for (var i = 0; i < Quests.Count; i++)
        {
            if (Quests[i].Type == Quest.QuestType.ScoreQuest)
                Quests[i].UpdateQuestProgress(progress);
        }
    }
    
    public static void SpawnItemQuest(int maxProgress)
    {
        Quests.Add(Instantiate(_questsManager._itemQuestPrefab, _questsManager._uiCanvas.transform.GetChild(1))
            .GetComponent<Quest>());
        var spawnedQuest = Quests[Quests.Count - 1];
        spawnedQuest.Type = Quest.QuestType.ItemQuest;
        spawnedQuest.MaxProgress = maxProgress;
        spawnedQuest.GetComponent<RectTransform>().anchoredPosition = 
            new Vector2(10, 640 - (110 * Quests.Count - 2));
    }

    public static void UpdateItemProgress(Item item)
    {
        for (var i = 0; i < Quests.Count; i++)
        {
            if (Quests[i].Type == Quest.QuestType.ItemQuest && Quests[i].TagToCount == item.tag)
                Quests[i].UpdateQuestProgress(1);
        }
    }

    public static void SpawnBariersQuest()
    {
        Quests.Add(Instantiate(_questsManager._bariersQuestPrefab, _questsManager._uiCanvas.transform.GetChild(1))
            .GetComponent<Quest>());
        var spawnedQuest = Quests[Quests.Count - 1];
        spawnedQuest.Type = Quest.QuestType.BarrierQuest;
        spawnedQuest.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(10, 640 - (110 * Quests.Count - 2));
    }
    
    public static void UpdateBarrierProgress(Barrier barrier)
    {
        for (var i = 0; i < Quests.Count; i++)
        {
            if (Quests[i].Type == Quest.QuestType.BarrierQuest && Quests[i].BarrierToCount == barrier.Type)
                Quests[i].UpdateQuestProgress(1);
        }
    }

    public static bool isQuestsCompleted()
    {
        if(Quests.Count <= 0)
            return false;

        for(var i = 0; i < Quests.Count; i++)
        {
            if (!Quests[i].Complete)
                return false;
        }

        return true;
    }
}
