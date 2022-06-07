using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Quest : MonoBehaviour
{
    [SerializeField] private List<Sprite> _fruitsImages;
    [SerializeField] private List<Sprite> _barriersImage;
    public QuestType Type;
    public bool Complete = false;
    public int CurrentProgress = 0;
    public int MaxProgress;
    public float ProgressFillPerOne;
    public Image ProgressBar;
    public TMP_Text ProgressText;
    public Image QuestImage;

    public string TagToCount;
    public Barrier.BarrierType BarrierToCount;

    public enum QuestType
    {
        ScoreQuest,
        ItemQuest,
        BarrierQuest
    }

    private void Start()
    {
        if(Type == QuestType.ItemQuest)
            SetRandomFruit();

        if(Type == QuestType.BarrierQuest)
            SetRandomBarrier();

        ProgressFillPerOne = 1f / (float)MaxProgress;
        UIManager.UpdateQuestsUI();
    }

    private void SetRandomFruit()
    {
        var randomFruit = Random.Range(0, _fruitsImages.Count);
        switch (randomFruit)
        {
            case 0:
                TagToCount = "Item_Blue";
                QuestImage.sprite = _fruitsImages[0];
                break;

            case 1:
                TagToCount = "Item_Green";
                QuestImage.sprite = _fruitsImages[1];
                break;

            case 2:
                TagToCount = "Item_Green2";
                QuestImage.sprite = _fruitsImages[2];
                break;

            case 3:
                TagToCount = "Item_Red";
                QuestImage.sprite = _fruitsImages[3];
                break;

            case 4:
                TagToCount = "Item_Orange";
                QuestImage.sprite = _fruitsImages[4];
                break;
        }
    }

    private void SetRandomBarrier()
    {
        if (!HasAnyBarrier())
        {
            QuestsManager.Quests.Remove(GetComponent<Quest>());
            Destroy(gameObject);
            return;
        }

        var randomBarrier = Random.Range(0, _barriersImage.Count);
        switch (randomBarrier)
        {
            case 0:
                BarrierToCount = Barrier.BarrierType.Ice;
                QuestImage.sprite = _barriersImage[0];
                MaxProgress = CountBarrierProgress(BarrierToCount);
                if (MaxProgress <= 0)
                    goto case 1;
                break;

            case 1:
                BarrierToCount = Barrier.BarrierType.Rock;
                QuestImage.sprite = _barriersImage[1];
                MaxProgress = CountBarrierProgress(BarrierToCount);
                if (MaxProgress <= 0)
                    goto case 0;
                break;
        }
    }

    private int CountBarrierProgress(Barrier.BarrierType type)
    {
        var progressCount = 0;
        for(var i = 0;i< TileGenerator.Width; i++)
        {
            for(var j = 0; j < TileGenerator.Height; j++)
            {
                if(TileGenerator.AllBariers[i,j] != null && TileGenerator.AllBariers[i,j].Type == type)
                    progressCount++;
            }
        }
        return progressCount;
    }

    private bool HasAnyBarrier()
    {
        for(var i = 0; i < TileGenerator.Width; i++)
            for(var j = 0; j < TileGenerator.Height; j++)
                if(TileGenerator.AllBariers[i,j] != null)
                    return true;

        return false;
    }

    public void UpdateQuestProgress(int progress)
    {
        gameObject.transform.DOShakeScale(0.2f, 0.1f).OnComplete(()=>gameObject.transform.localScale = new Vector3(3.6f,0.7f));
        CurrentProgress += progress;
        if(CurrentProgress >= MaxProgress)
        {
            Complete = true;
            CurrentProgress = MaxProgress;
            ProgressBar.fillAmount = 1;
            ProgressText.color = Color.green;
        }

        UIManager.UpdateQuestsUI();
    }
}
