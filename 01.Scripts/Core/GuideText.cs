using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GuideText : MonoBehaviour
{
    [SerializeField] private string[] _guideTexts;
    private TextMeshPro[] _guideTMPs;
    List<int> _guideTextList = new List<int>();

    private void Awake()
    {
        _guideTMPs = GetComponentsInChildren<TextMeshPro>();
    }

    private void Start()
    {
        for (int i = 0; i < _guideTMPs.Length;)
        {
            int currentNumber = Random.Range(0, _guideTexts.Length);

            if (_guideTextList.Contains(currentNumber))
            {
                currentNumber = Random.Range(0, _guideTexts.Length);
            }
            else
            {
                _guideTextList.Add(currentNumber);
                _guideTMPs[i].SetText(_guideTexts[currentNumber]);
                i++;
            }
        }
    }
}