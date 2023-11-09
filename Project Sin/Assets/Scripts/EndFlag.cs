using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerSaveData 
{
    public string Time, Level; 
}

public class EndFlag : MonoBehaviour
{
    string saveFilePath;
    string StoppedTime;
    PlayerSaveData PlayerData;
    private void Awake()
    {
        PlayerData = new PlayerSaveData();
        saveFilePath = Application.persistentDataPath + "/PlayerData.json";
    }
    private void OnTriggerEnter(Collider other)
    {
        SpeedRunTimer.Instance.StopTimer();
        //TODO: Use the save to file function here
        StoppedTime = SpeedRunTimer.Instance.GetTimer().ToString("0.000");
        SaveGame();
    }
    public void SaveGame()
    {

        PlayerData.Time = StoppedTime;
        PlayerData.Level = SceneManager.GetActiveScene().name.ToString();
        string savePlayerData = JsonUtility.ToJson(PlayerData);
        //File.WriteAllText(saveFilePath, savePlayerData);
        // Create or open the file in append mode
        using (StreamWriter streamWriter = new StreamWriter(saveFilePath, true))
        { 
            streamWriter.WriteLine(savePlayerData);
        }
       
        Debug.Log(StoppedTime);
        Debug.Log("Save file created at: " + saveFilePath);
    }
}
