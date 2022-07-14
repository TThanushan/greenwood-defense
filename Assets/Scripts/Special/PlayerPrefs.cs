#if UNITY_WEBGL && !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

// ***************************************************
// ** PUT YOUR SAVE PATH NAME IN THE VARIABLE BELOW **
// ***************************************************

// This class acts sort of as an emulator for
// PlayerPrefs, but saves to a specified
// place in IndexedDB so saved games don't
// get wiped when game updates are uploaded

// PlayerPrefs is similar (but not identical)
// to a hash of ints, floats, and strings, so
// this uses a Dictionary in C# (like a hash)

// Note that in this implementation all of
// the values are stored as strings even if
// they're really int or float, but they
// get parsed when values are returned

// Also note that I call Save() any time
// a value gets changed, because I think
// the risk of not saving stuff leading to
// bugginess outweighs the computational
// cost of saving more frequently.
// That could potentially be changed fairly
// easily as long as you know it's happening
// and that you just need to comment out the
// Save() statements at the end of the Set
// methods.

public static class PlayerPrefs
{
    public static string savePathName = "849132";
    public static string DataPath => "/idbfs/" + savePathName + "/NGSave.dat";

    [Serializable]
    private class PlayerPrefsData
    {
        public List<KeyValue> data;
    }

    [Serializable]
    private class KeyValue
    {
        public string Key;
        public string Value;
    }

    static Dictionary<string, string> saveData = new Dictionary<string, string>();

    // This is the static constructor for the class
    // When invoked, it looks for a savegame file
    // and reads the keys and values
    static PlayerPrefs()
    {
        // Open the savegame file and read all of the lines
        // into fileContents
        // First make sure the directory and save file exist,
        // and make them if they don't already

        if (File.Exists(DataPath))
        {
            // Read the file if it already existed
            string data = System.Text.Encoding.UTF8.GetString(File.ReadAllBytes(DataPath));

            // If you want to use encryption/decryption, add your
            // code for decrypting here
            // ******* decryption algorithm ********

            PlayerPrefsData ppd = JsonUtility.FromJson<PlayerPrefsData>(data);
            if (ppd != null)
            {
                saveData = ppd.data.ToDictionary(t => t.Key, t => t.Value);
            }
            else Debug.LogError("WebGL PlayerPrefs wrong format!");
        }
    }

    [DllImport("__Internal")]
    private static extern void FlushIDBFS();

    // This saves the saveData to the player's IndexedDB
    public static void Save()
    {
        PlayerPrefsData ppd = new PlayerPrefsData
        { data = saveData.Keys.Select(t => new KeyValue { Key = t, Value = saveData[t] }).ToList() };

        // If you want to use encryption/decryption, add your
        // code for encrypting here
        // ******* encryption algorithm ********

        // Write fileContents to the save file
        string data = JsonUtility.ToJson(ppd);
        byte[] bytedata = Encoding.UTF8.GetBytes(data);

        File.WriteAllBytes(DataPath, bytedata);

        FlushIDBFS();
    }

    // The following methods emulate what PlayerPrefs does
    public static void DeleteAll()
    {
        saveData.Clear();
    }

    public static void DeleteKey(string key)
    {
        saveData.Remove(key);
    }

    public static float GetFloat(string key)
    {
        return float.Parse(saveData[key]);
    }

    public static float GetFloat(string key, float defaultValue)
    {
        if (saveData.ContainsKey(key))
        {
            return float.Parse(saveData[key]);
        }
        else
        {
            return defaultValue;
        }
    }

    public static int GetInt(string key)
    {
        return int.Parse(saveData[key]);
    }

    public static int GetInt(string key, int defaultValue)
    {
        if (saveData.ContainsKey(key))
        {
            return int.Parse(saveData[key]);
        }
        else
        {
            return defaultValue;
        }
    }

    public static string GetString(string key)
    {
        return saveData[key];
    }

    public static string GetString(string key, string defaultValue)
    {
        if (saveData.ContainsKey(key))
        {
            return saveData[key];
        }
        else
        {
            return defaultValue;
        }
    }

    public static bool HasKey(string key)
    {
        return saveData.ContainsKey(key);
    }

    public static void SetFloat(string key, float setValue)
    {
        if (saveData.ContainsKey(key))
        {
            saveData[key] = setValue.ToString();
        }
        else
        {
            saveData.Add(key, setValue.ToString());
        }
    }

    public static void SetInt(string key, int setValue)
    {
        if (saveData.ContainsKey(key))
        {
            saveData[key] = setValue.ToString();
        }
        else
        {
            saveData.Add(key, setValue.ToString());
        }
    }

    public static void SetString(string key, string setValue)
    {
        if (saveData.ContainsKey(key))
        {
            saveData[key] = setValue;
        }
        else
        {
            saveData.Add(key, setValue);
        }
    }
}
#endif

using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScript : MonoBehaviour
{
    public void startGame()
    {
        SceneManager.LoadScene("Level Select", LoadSceneMode.Single);
    }
}


