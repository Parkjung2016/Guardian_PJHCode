using System.Collections;
using UnityEngine;
using EventReference = FMODUnity.EventReference;

public class LobbyManager : Manager
{
    [SerializeField] private BossRoom[] _bossRooms;
    [SerializeField] private EventReference _bgm;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => PlayerManager.Instance.Player);
        PlayerController player = PlayerManager.Instance.Player;
        // player.canCombat = false;
        player.canReduceEnergy = false;
        int bossRoomCount = PlayerDataManager.playerData.bossStep;
        for (int i = 0; i < bossRoomCount; i++)
        {
            OpenBossRoom(i);
        }

        SoundManager.PlayBGM(_bgm);
    }


    public void OpenBossRoom(int idx)
    {
        _bossRooms[idx].OpenBossRoom(true);
    }
}