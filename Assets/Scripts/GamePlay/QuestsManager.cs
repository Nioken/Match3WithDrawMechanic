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

    private static QuestsManager _questsManager;
    public static List<Quest> Quests = new List<Quest>();

    private void OnEnable()
    {
        _questsManager = GetComponent<QuestsManager>();
    }
   
    public static void SpawnScoreQuest(int maxProgress)
    {
        Quests.Add(Instantiate(_questsManager._scoreQuestPrefab, _questsManager._uiCanvas.transform.GetChild(1))
            .GetComponent<Quest>());
        Quests[^1].Type = Quest.QuestType.ScoreQuest;
        Quests[^1].MaxProgress = maxProgress;
        Quests[^1].GetComponent<RectTransform>().anchoredPosition = 
            new Vector2(10, 640 - (110 * Quests.Count - 2));
    }  
    
    public static void UpdateScoreProgress(int progress)
    {
        foreach (var quest in Quests)
            if (quest.Type == Quest.QuestType.ScoreQuest)
                quest.UpdateQuestProgress(progress);
    }
    
    public static void SpawnItemQuest(int maxProgress)
    {
        Quests.Add(Instantiate(_questsManager._itemQuestPrefab, _questsManager._uiCanvas.transform.GetChild(1))
            .GetComponent<Quest>());
        
        var spawnedQuest = Quests[^1];
        
        spawnedQuest.Type = Quest.QuestType.ItemQuest;
        spawnedQuest.MaxProgress = maxProgress;
        spawnedQuest.GetComponent<RectTransform>().anchoredPosition = 
            new Vector2(10, 640 - (110 * Quests.Count - 2));
    }

    public static void UpdateItemProgress(Item item)
    {
        foreach (var quest in Quests)
            if (quest.Type == Quest.QuestType.ItemQuest && quest.TagToCount == item.tag)
                quest.UpdateQuestProgress(1);
    }

    public static void SpawnBariersQuest()
    {
        Quests.Add(Instantiate(_questsManager._bariersQuestPrefab, _questsManager._uiCanvas.transform.GetChild(1))
            .GetComponent<Quest>());
        
        var spawnedQuest = Quests[^1];
        
        spawnedQuest.Type = Quest.QuestType.BarrierQuest;
        spawnedQuest.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(10, 640 - (110 * Quests.Count - 2));
    }
    
    public static void UpdateBarrierProgress(Barrier barrier)
    {
        foreach (var quest in Quests)
            if (quest.Type == Quest.QuestType.BarrierQuest && quest.BarrierToCount == barrier.Type)
                quest.UpdateQuestProgress(1);
    }

    public static bool IsQuestsCompleted()
    {
        if(Quests.Count <= 0)
            return false;

        foreach (var quest in Quests)
            if (!quest.Complete)
                return false;

        return true;
    }
}
