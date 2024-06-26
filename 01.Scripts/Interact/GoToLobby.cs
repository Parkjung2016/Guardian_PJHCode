using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToLobby : InteractAble
{
    public override void Interact()
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

        canInteract = false;
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