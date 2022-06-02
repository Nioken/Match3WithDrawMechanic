using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
        {
            int randomFruit = Random.Range(0, _fruitsImages.Count);
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
        if(Type == QuestType.BarrierQuest)
        {
            int randomBarrier = Random.Range(0, _barriersImage.Count);
            switch (randomBarrier)
            {
                case 0:
                    BarrierToCount = Barrier.BarrierType.Ice;
                    QuestImage.sprite = _barriersImage[0];
                    break;
                case 1:
                    BarrierToCount = Barrier.BarrierType.Rock;
                    QuestImage.sprite = _barriersImage[1];
                    break;
            }
            for (int i = 0; i < TileGenerator.X; i++)
            {
                for (int j = 0; j < TileGenerator.Y; j++)
                {
                    if (TileGenerator.AllBariers[i, j] != null && TileGenerator.AllBariers[i,j].Type == BarrierToCount)
                        MaxProgress++;
                }
            }
            if (MaxProgress <= 0)
            {
                if(randomBarrier == 0)
                {
                    BarrierToCount = Barrier.BarrierType.Rock;
                    QuestImage.sprite = _barriersImage[1];
                    for (int i = 0; i < TileGenerator.X; i++)
                    {
                        for (int j = 0; j < TileGenerator.Y; j++)
                        {
                            if (TileGenerator.AllBariers[i, j] != null && TileGenerator.AllBariers[i, j].Type == BarrierToCount)
                            {
                                MaxProgress++;
                            }
                        }
                    }
                    if(MaxProgress <= 0)
                    {
                        QuestsManager.Quests.Remove(GetComponent<Quest>());
                        Destroy(gameObject);
                    }
                }
                else
                {
                    BarrierToCount = Barrier.BarrierType.Ice;
                    QuestImage.sprite = _barriersImage[0];
                    for (int i = 0; i < TileGenerator.X; i++)
                    {
                        for (int j = 0; j < TileGenerator.Y; j++)
                        {
                            if (TileGenerator.AllBariers[i, j] != null && TileGenerator.AllBariers[i, j].Type == BarrierToCount)
                                MaxProgress++;
                        }
                    }
                    if (MaxProgress <= 0)
                    {
                        QuestsManager.Quests.Remove(GetComponent<Quest>());
                        Destroy(gameObject);
                    }
                }
            }
        }

        ProgressFillPerOne = 1f / (float)MaxProgress;
        UIManager.UpdateQuestsUI();
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
