using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private LineRenderer LineRenderer;
    [SerializeField]
    private UIManager _UIManager;
    private RaycastHit _hit;
    public static int PlayerSteps = 100;

    private void OnEnable()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        TileGenerator.UpdateCameraSize();
        MatchManager.SelectedItems = new List<Item>();
        Debug.Log(MatchManager.SelectedItems.Count);
        _UIManager.UpdateSteps(PlayerSteps);
    }

    private void InputItemsCast()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _hit))
        {
            if (_hit.collider.tag.Contains("Item") || _hit.collider.tag == "Bonus")
            {
                if (_hit.collider.tag != "Bonus")
                {
                    if (MatchManager.CurrentTag == null)
                    {
                        MatchManager.CurrentTag = _hit.collider.tag;
                    }
                }
                if (_hit.collider.tag == MatchManager.CurrentTag || _hit.collider.tag == "Bonus")
                {
                    Item cachedItem = null;
                    for (int i = 0; i < TileGenerator.X; i++)
                    {
                        for (int j = 0; j < TileGenerator.Y; j++)
                        {
                            if (TileGenerator.AllItems[i, j] == null) continue;
                            if (_hit.collider.gameObject == TileGenerator.AllItems[i, j].gameObject)
                            {
                                if (TileGenerator.AllTiles[i, j].IsBarried == true)
                                {
                                    return;
                                }
                                cachedItem = TileGenerator.AllItems[i, j];
                            }
                        }
                    }
                    if (!MatchManager.SelectedItems.Contains(cachedItem))
                    {
                        if (MatchManager.SelectedItems.Count > 0)
                        {
                            if (!MatchManager.CheckNear(cachedItem))
                            {
                                return;
                            }
                        }
                        _hit.collider.gameObject.transform.DOScale(1.3f, 0.5f);
                        LineRenderer.positionCount++;
                        LineRenderer.SetPosition(LineRenderer.positionCount - 1, new Vector3(_hit.collider.gameObject.transform.position.x, _hit.collider.gameObject.transform.position.y, 19));
                        MatchManager.SelectedItems.Add(cachedItem);
                        AudioManager.PlaySelectSound();
                    }
                    else
                    {
                        if (MatchManager.SelectedItems.Count > 1 && MatchManager.SelectedItems.IndexOf(cachedItem) != MatchManager.SelectedItems.Count - 1)
                        {

                            for (int i = MatchManager.SelectedItems.IndexOf(cachedItem) + 1; i < MatchManager.SelectedItems.Count; i++)
                            {
                                MatchManager.SelectedItems[i].gameObject.transform.DOScale(1f, 0.5f);
                                MatchManager.SelectedItems.RemoveAt(i);
                                LineRenderer.positionCount--;
                            }
                        }
                    }
                }

            }
        }
    }
    private void EndInput()
    {
        LineRenderer.positionCount = 0;
        MatchManager.CheckMatch();
        _UIManager.UpdateSteps(PlayerSteps);
        if (PlayerSteps == 0)
        {
            if (QuestsManager.isQuestsCompleted())
            {
                _UIManager.ShowWinUI();
            }
            else
            {
                _UIManager.ShowLoseUI();
            }
        }
        else
        {
            if (QuestsManager.isQuestsCompleted())
            {
                _UIManager.ShowWinUI();
            }
        }
    }

    private void Update()
    {
        #region AndroidInput
#if UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began || Input.touches[0].phase == TouchPhase.Moved)
            {
                InputItemsCast();
            }
            if (Input.touches[0].phase == TouchPhase.Ended)
            {
                EndInput();
            }
        }
#endif
        #endregion
        #region EditorInput
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            InputItemsCast();
        }
        if (Input.GetMouseButtonUp(0))
        {
            EndInput();
        }
#endif
        #endregion
    }
}
