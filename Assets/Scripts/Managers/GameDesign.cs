using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RemoteConfig;

public enum Difficulty
{
    easy=0,
    medium=1,
    hard=2,
}
[System.Serializable]
public class GameDifficulty
{
    public Difficulty difficulty;
    public int LineSpawnedDuringGame;//how many are the total number of lines we can spawn during the game
    public int NumberOfBallsToCreateLine;//how many balls does the player have to shoot in order t create line
}
public class GameDesign : MonoBehaviour
{
    [SerializeField] List<GameDifficulty> gameDifficulties;
    [SerializeField] Difficulty currentDifficulty;
    public delegate void IntitliazeDesign(GameDifficulty d);
    public static event IntitliazeDesign intitliazeDesign;

    public struct userAttributes { }
    public struct appAttributes { }

    private void Awake()
    {
        ConfigManager.FetchCompleted += setDifficulty;
        ConfigManager.FetchConfigs<userAttributes,appAttributes>(new userAttributes(),new appAttributes());
    }
    void setDifficulty(ConfigResponse response)
    {
        int diff = ConfigManager.appConfig.GetInt("GameDifficulty");
        if (diff > 2) diff = 2;
        if (diff < 0) diff = 0;
        currentDifficulty = (Difficulty)diff;
        GameDifficulty difficulty = gameDifficulties.Find(x => x.difficulty == currentDifficulty);
        if (difficulty != null)
        {
            intitliazeDesign?.Invoke(difficulty);
        }
    }
    private void OnDestroy()
    {
        ConfigManager.FetchCompleted -= setDifficulty;
    }

}
