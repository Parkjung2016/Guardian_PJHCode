using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

[Serializable]
public struct Timer
{
    public byte hour;
    public byte minute;
    public byte second;


    public void AddSecond(byte value)
    {
        second += value;

        if (second >= 60)
        {
            second = 0;
            minute++;
        }
    }

    public bool IsNull()
    {
        return hour == 0 && minute == 0 && second == 0;
    }

    public string ToString()
    {
        if (second < 10)
        {
            return $"{minute} :0{second}";
        }

        return $"{minute} : {second}";
    }
}

public class PlayerDataManager
{
    private static readonly string privateKey = "1718hy9dsf0jsdlfjds0pa9ids78ahgf81h32re";
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

        public List<Timer> bossTimer = new();
    }

    public static PlayerData playerData = new();
    private static bool encrypt = false;

    public static void Save()
    {
        string json = JsonUtility.ToJson(playerData, true);
        if (encrypt)
        {
            string encryptString = Encrypt(json);
            SaveFile(encryptString);
        }
        else
        {
            File.WriteAllText(path, json);
        }
    }

    public static bool Load()
    {
        if (!ExistsData()) return false;
        string json = "";

        if (encrypt)
        {
            string encryptData = LoadFile(path);
            json = Decrypt(encryptData);
        }
        else
        {
            json = File.ReadAllText(path);
        }

        playerData = JsonUtility.FromJson<PlayerData>(json);

        return playerData != null;
    }

    static void SaveFile(string jsonData)
    {
        using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

            fs.Write(bytes, 0, bytes.Length);
        }
    }

    static string LoadFile(string path)
    {
        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            byte[] bytes = new byte[(int)fs.Length];

            fs.Read(bytes, 0, (int)fs.Length);

            string jsonString = System.Text.Encoding.UTF8.GetString(bytes);
            return jsonString;
        }
    }

    private static string Encrypt(string data)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
        RijndaelManaged rm = CreateRijndaelManaged();
        ICryptoTransform ct = rm.CreateEncryptor();
        byte[] results = ct.TransformFinalBlock(bytes, 0, bytes.Length);
        return System.Convert.ToBase64String(results, 0, results.Length);
    }

    private static string Decrypt(string data)
    {
        byte[] bytes = System.Convert.FromBase64String(data);
        RijndaelManaged rm = CreateRijndaelManaged();
        ICryptoTransform ct = rm.CreateDecryptor();
        byte[] resultArray = ct.TransformFinalBlock(bytes, 0, bytes.Length);
        return System.Text.Encoding.UTF8.GetString(resultArray);
    }


    private static RijndaelManaged CreateRijndaelManaged()
    {
        byte[] keyArray = System.Text.Encoding.UTF8.GetBytes(privateKey);
        RijndaelManaged result = new RijndaelManaged();

        byte[] newKeysArray = new byte[16];
        System.Array.Copy(keyArray, 0, newKeysArray, 0, 16);

        result.Key = newKeysArray;
        result.Mode = CipherMode.ECB;
        result.Padding = PaddingMode.PKCS7;
        return result;
    }

    public static bool ExistsData() => File.Exists(path);
}