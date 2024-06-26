using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FMODUnity;
using UnityEngine;

public class ClearAllBoss : InteractAble
{
    [SerializeField] private EventReference _clearBGM;

    public override void Interact()
    {
        canInteract = false;
        StartCoroutine(Clear());
        // PlayerManager.Instance.Player.InputReader.EnableUIInput(false);
    }

    IEnumerator Clear()
    {
        PlayerManager.Instance.Player.InputReader.EnablePlayerInput(false);
        PlayerManager.Instance.Player.stopMove = true;
        PlayerManager.Instance.Player.stopRotate = true;
        if (PlayerManager.Instance.enteredBossGate)
            PlayerManager.Instance.enteredBossGate = false;
        SoundManager.PlaySFX(_clearBGM);
        yield return new WaitForSeconds(1f);
        EndGameUI.Instance.ShowText(async () =>
        {
            EndGameUI.Instance.ShowMidText("플레이 해주셔서 감사합니다.", .5f);
            await Task.Delay(3000);
            FadeUI.FadeIn(1, async () =>
            {
                await Task.Delay(1000);
                LoadSceneManager.LoadScene("Title");
            });
        });
    }
}