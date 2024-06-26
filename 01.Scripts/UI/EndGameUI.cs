using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;


public class EndGameUI : MonoSingleton<EndGameUI>
{
    [Serializable]
    public struct EndGameTextInfo
    {
        [TextArea] public string text;
        public float endTextTime;
    }

    [SerializeField] private List<EndGameTextInfo> _textInfoList;
    private TextMeshProUGUI _leftTMP;
    private TextMeshProUGUI _rightTMP;
    private TextMeshProUGUI _midTMP;
    private CanvasGroup _midCanvasGroup;

    private int _currentTextInfoIdx;

    private void Awake()
    {
        _leftTMP = transform.Find("LeftText").GetComponent<TextMeshProUGUI>();
        _rightTMP = transform.Find("RightText").GetComponent<TextMeshProUGUI>();
        _midCanvasGroup = transform.Find("Mid").GetComponent<CanvasGroup>();
        _midTMP = _midCanvasGroup.transform.Find("MidText").GetComponent<TextMeshProUGUI>();
        _leftTMP.DOFade(0, 0);
        _rightTMP.DOFade(0, 0);
        _midCanvasGroup.alpha = 0;
        _midCanvasGroup.gameObject.SetActive(false);
        Instance.gameObject.SetActive(false);
    }

    public void ShowText(Action callBack = null)
    {
        gameObject.SetActive(true);
        StartCoroutine(ShowTextCoroutine(callBack));
    }

    public void ShowMidText(string text, float duration)
    {
        _midCanvasGroup.gameObject.SetActive(true);
        _midTMP.SetText(text);
        _midCanvasGroup.DOFade(1, duration);
    }

    private IEnumerator ShowTextCoroutine(Action callBack = null)
    {
        int direction = 0;

        while (_currentTextInfoIdx < _textInfoList.Count)
        {
            EndGameTextInfo textInfo = _textInfoList[_currentTextInfoIdx];
            switch (direction)
            {
                case 0:
                    _leftTMP.DOFade(1, .5f);
                    _rightTMP.DOFade(0, .5f);
                    _leftTMP.SetText(textInfo.text);
                    break;
                case 1:
                    _rightTMP.DOFade(1, .5f);
                    _leftTMP.DOFade(0, .5f);
                    _rightTMP.SetText(textInfo.text);

                    break;
            }

            yield return CoroutineHelper.WaitForSeconds(textInfo.endTextTime);

            _currentTextInfoIdx++;
            direction ^= 1;
        }

        callBack?.Invoke();
    }
}