using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour,IInteractAble
{
    [field: SerializeField] public string InteractName { get; set; }
    public bool CanInteract { get; set; } = true;

    public Transform Trm { get; set; }

    private void Awake()
    {
        Trm = transform;

    }

    public void Interact()
    {
        PlayerManager.Instance.Player.InputReader.EnableUIInput(false);
        PlayerManager.Instance.Player.InputReader.EnablePlayerInput(false);
        PlayerManager.Instance.Player.stopMove = true;
        PlayerManager.Instance.Player.stopRotate = true;
        FadeUI.FadeIn(callBack: () =>
        {
            PlayerManager.Instance.Player.InputReader.EnableUIInput(true);
            PlayerManager.Instance.Player.InputReader.EnablePlayerInput(true);
            LoadSceneManager.LoadScene("Tutorial");
        });
    }
}