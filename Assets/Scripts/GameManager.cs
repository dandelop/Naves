using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{
    public static ControlPlayer Player = null;
    public static ControlFormation Formation = null;
    public static HUDManager HUD = null;

    private static int numEnemenies;
    public static int NumEnemenies
    {
        get => numEnemenies;
        set => numEnemenies = value;
    }

    public static int playerLifes = 3;
    
    private static int score = 0;
    private static int highScore = 0;

    private static int numLevel = 0;

    private static bool gameIsPaused = false;

    public static bool GameIsPaused => gameIsPaused;

    public static void addPoints(int amount)
    {
        score += amount;
        if (score > highScore)
        {
            highScore = score;
        }

        HUD.score = score;
        HUD.highScore = highScore;
    }

    public static void removeEnemy()
    {
        numEnemenies--;
        if (numEnemenies == 0)
        {
            HUD.ShowGameOver(true);
            numLevel++;
            pauseGame();
        }
    }

    public static void playerDie()
    {
        playerLifes--;
        HUD.playerLifes = playerLifes;
        if (playerLifes == 0)
        {
            HUD.ShowGameOver(false);
            pauseGame();
        }
        else
        {
            Formation.Retract();
        }
    }

    private static void pauseGame()
    {
        //Time.timeScale = 0f;
        //AudioListener.pause = true;
        gameIsPaused = true;
    }

    public static void timeEnd()
    {
        HUD.ShowGameOver(false);
        pauseGame();
    }

    public static void createCoin(GameObject coinPrefab, Transform transformCoin)
    {
        GameObject go = GameObject.Instantiate(coinPrefab);
        go.transform.position = new Vector3(transformCoin.position.x, transformCoin.position.y - 0.2f);
    }
}
