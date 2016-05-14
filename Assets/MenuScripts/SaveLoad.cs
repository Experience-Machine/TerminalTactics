using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoad
{
    GameObject myCanvas;
    Button returnButton;
    GlobalGameManager manager;

    // List of saved games
    // Each saved game is a list of objects
    // Currently, this implementation has only one save, but is extensible to many
    public static List<List<System.Object>> savedGames = new List<List<System.Object>>();

    // Use this for initialization
    public SaveLoad()
    {
        manager = GameObject.Find("GlobalGameManager").GetComponent<GlobalGameManager>();
    }

    public void SaveAllData()
    {
        List<System.Object> thisSave = new List<System.Object>();
        thisSave.Add(manager.characterInfos);
        thisSave.Add(manager.cardManager);
        savedGames.Add(thisSave);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd");
        bf.Serialize(file, savedGames);
        file.Close();

        Debug.Log("Saved game to: " + Application.persistentDataPath + "/savedGames.gd");
    }

    public void LoadAllData()
    {
        if (File.Exists(Application.persistentDataPath + "/savedGames.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
            savedGames = (List<List<System.Object>>)bf.Deserialize(file);
            file.Close();

            if (savedGames.Capacity > 0)
            {
                List<System.Object> thisSave = savedGames[0];
                manager.characterInfos = (characterInfo[])thisSave[0];
                manager.cardManager = (CardManager)thisSave[1];
                Debug.Log("Loaded saved game");
            }
            else
            {
                Debug.Log("ERROR: Empty savegame file");
            }
        }
    }
}
