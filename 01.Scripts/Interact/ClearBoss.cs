using UnityEngine;

public class ClearBoss : GoToLobby
{
    [SerializeField] private int _bossIdx;

    public override void Interact()
    {
        if (PlayerDataManager.playerData.bossTimer.Count - 1 < _bossIdx)
            PlayerDataManager.playerData.bossTimer.Add((GameManager.Instance as GameManager).timer);
        else
            PlayerDataManager.playerData.bossTimer[_bossIdx] = (GameManager.Instance as GameManager).timer;
        base.Interact();
    }
}