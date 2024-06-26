using System;
using System.IO;
using System.Net;
using UnityEngine;

public class PlayerDataManager
{
    private static readonly string path = Path.Combine(Application.persistentDataPath, "PlayerData.json");

    [Serializable]
    public class PlayerData
    {
        public byte bossStep = 1;
        public byte currentWeapon = 3;
        public bool completeTutorial = false;
        public Single masterVolume = 1;
        public Single bgmVolume = 1;
        public Single sfxVolume = 1;
    }

    public static PlayerData playerData = new();

    public static void Save()
    {
        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(path, json);
    }

    public static bool Load()
    {
        if (!ExistsData()) return false;
        string loadJson = File.ReadAllText(path);
        playerData = JsonUtility.FromJson<PlayerData>(loadJson);
        return playerData != null;
    }

    public static bool ExistsData() => File.Exists(path);
}