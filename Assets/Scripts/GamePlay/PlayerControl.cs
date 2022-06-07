using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private UIManager _uiManager;
    public static int PlayerSteps = 100;
    private RaycastHit _hit;

    private void OnEnable()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        TileGenerator.UpdateCameraSize();
        MatchManager.SelectedItems = new List<Item>();
        _uiManager.UpdateSteps(PlayerSteps);
    }

    private void SelectItem(Item item)
    {
        item.transform.DOScale(1.3f, 0.5f);
        _lineRenderer.positionCount++;
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, item.transform.position);
        MatchManager.SelectedItems.Add(item);
        AudioManager.PlaySelectSound();
    }

    private void UnselectItems(int startIndex)
    {
        for (var i = startIndex; i < MatchManager.SelectedItems.Count; i++)
        {
            MatchManager.SelectedItems[i].transform.DOScale(1f, 0.5f);
            MatchManager.SelectedItems.RemoveAt(i);
            _lineRenderer.positionCount--;
        }
    }
    
    private void SetCurrentTag(string tag)
    {
        if (tag.Contains("Item") && MatchManager.CurrentTag == null)
            MatchManager.CurrentTag = tag;
    }

    private bool CanSelect(Collider itemCollider)
    {
        if (itemCollider.tag.Contains("Item") || itemCollider.CompareTag("Bonus"))
            if (itemCollider.CompareTag("Bonus") || itemCollider.CompareTag(MatchManager.CurrentTag))
                return true;

        return false;
    }

    private void InputItemsCast()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _hit))
        {
            SetCurrentTag(_hit.collider.tag);
            if (CanSelect(_hit.collider))
            {
                Item hitedItem = TileGenerator.ReturnItem(_hit.collider.gameObject);
                if (!MatchManager.SelectedItems.Contains(hitedItem)) // Добавление токенов в цепочку
                {
                    if (MatchManager.SelectedItems.Count > 0 && !MatchManager.CheckNear(hitedItem))
                        return;

                    SelectItem(hitedItem);
                }
                else // Удаление токенов из цепочки
                {
                    if (MatchManager.SelectedItems.Count > 1 && MatchManager.SelectedItems.IndexOf(hitedItem) != MatchManager.SelectedItems.Count - 1)
                        UnselectItems(MatchManager.SelectedItems.IndexOf(hitedItem) + 1);
                }
            }
        }
    }

    private void EndInput()
    {
        _lineRenderer.positionCount = 0;
        MatchManager.CheckMatch();
        _uiManager.UpdateSteps(PlayerSteps);
        if (PlayerSteps == 0)
        {
            if (QuestsManager.isQuestsCompleted())
                _uiManager.ShowWinUI();
            else
                _uiManager.ShowLoseUI();
        }
        else
        {
            if (QuestsManager.isQuestsCompleted())
                _uiManager.ShowWinUI();
        }
    }

    private void Update()
    {

        #region AndroidInput

#if UNITY_ANDROID

        if (Input.touchCount > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began || Input.touches[0].phase == TouchPhase.Moved)
                InputItemsCast();

            if (Input.touches[0].phase == TouchPhase.Ended)
                EndInput();
        }

#endif

        #endregion

        #region EditorInput

#if UNITY_EDITOR

        if (Input.GetMouseButton(0))
            InputItemsCast();

        if (Input.GetMouseButtonUp(0))
            EndInput();

#endif

        #endregion

    }
}
