using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToLobby : MonoBehaviour, IInteractAble
{
    [field: SerializeField] public string InteractName { get; set; }
    public bool CanInteract { get; set; } = true;

    public Transform Trm { get; set; }

    private void Awake()
    {
        Trm = transform;

    }

    public virtual void Interact()
    {
        if (TutorialManager.Instance)
        {
            TutorialManager tutorialManager = (TutorialManager.Instance as TutorialManager);
            if (tutorialManager)
                if (tutorialManager.currentStep == 7)
                {
                    tutorialManager.ClearGoal();
                    tutorialManager.UpdateGoal(0);
                }
        }

        CanInteract = false;
        PlayerManager.Instance.Player.InputReader.EnableUIInput(false);
        PlayerManager.Instance.Player.InputReader.EnablePlayerInput(false);
        PlayerManager.Instance.Player.stopMove = true;
        PlayerManager.Instance.Player.stopRotate = true;
        if (PlayerManager.Instance.enteredBossGate)
            PlayerManager.Instance.enteredBossGate = false;

        FadeUI.FadeIn(callBack: () =>
        {
            PlayerManager.Instance.Player.InputReader.EnableUIInput(true);
            PlayerManager.Instance.Player.InputReader.EnablePlayerInput(true);

            LoadSceneManager.LoadSceneWithLoading("Lobby");
        });
    }
}