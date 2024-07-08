using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapWeapon : MonoBehaviour
{
    [SerializeField] private byte _weaponIdx;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            PlayerDataManager.playerData.currentWeapon = _weaponIdx;
            PlayerWeapon weapon = PlayerManager.Instance.weapons[_weaponIdx];
            player.SwapWeapon(weapon);
        }
    }
}