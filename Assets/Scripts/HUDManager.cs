using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    // Panel HUD principal
    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textHighScore;
    public TextMeshProUGUI textTime;
    public TextMeshProUGUI textBonusTime;
    public TextMeshProUGUI textBonusLife;
    
    public GameObject Life1;
    public GameObject Life2;
    public GameObject Life3;

    public GameObject lifePrefab;
    public Transform bonusLifeAnchor;

    public GameObject Level1;
    public GameObject Level2;

    // Panel final de juego
    public GameObject panelGameOver;
    public TextMeshProUGUI textMessage;

    public int secondsLevel = 180;
    public int score;
    public int highScore;
    public int playerLifes;

    private float _tpInitial;
    private int secondsToEnd;

    private void Awake()
    {
        GameManager.HUD = this;
        GameManager.playerLifes = playerLifes;
        _tpInitial = Time.time;
        panelGameOver.SetActive(false);
    }

    void Update()
    {
        if (GameManager.GameIsPaused)
            return;
        
        float tpGame = Time.time - _tpInitial;
        secondsToEnd = secondsLevel - Mathf.FloorToInt(tpGame);
        if (secondsToEnd <= 0)
        {
            GameManager.timeEnd();
        }

        textTime.text = intSeconds2textMMSS(secondsToEnd);
        textScore.text = score.ToString();
        textHighScore.text = highScore.ToString();
        if ((playerLifes >= 0) && (playerLifes <=3))
        {
            switch (playerLifes)    
            {
                case 0:
                    Life1.SetActive(false);
                    Life2.SetActive(false);
                    Life3.SetActive(false);
                    break;
                case 1:
                    Life1.SetActive(true);
                    Life2.SetActive(false);
                    Life3.SetActive(false);
                    break;
                case 2:
                    Life1.SetActive(true);
                    Life2.SetActive(true);
                    Life3.SetActive(false);
                    break;
                case 3:
                    Life1.SetActive(true);
                    Life2.SetActive(true);
                    Life3.SetActive(true);
                    break;
            }
        }
    }

    public void ShowGameOver(bool win)
    {
        if (win)
        {
            textMessage.text = "You Win !!";
            textBonusTime.enabled = true;
            textBonusLife.enabled = true;
            textBonusTime.text = intSeconds2textMMSS(secondsToEnd) + " x 10 = " + (secondsToEnd * 10) ;
            GameManager.addPoints(secondsToEnd * 10);
            textBonusLife.text = "x 500 = " + (playerLifes * 500) ;
            GameManager.addPoints(playerLifes * 500);
            textScore.text = score.ToString();
            textHighScore.text = highScore.ToString();
            // vidas restantes
            GameObject go1, go2;
            switch (playerLifes)    
            {
                case 0:
                    break;
                case 1:
                    go1 = GameObject.Instantiate(lifePrefab);
                    go1.transform.SetParent(panelGameOver.transform, false);
                    go1.transform.position = bonusLifeAnchor.position;
                    break;
                case 2:
                    go1 = GameObject.Instantiate(lifePrefab);
                    go1.transform.SetParent(panelGameOver.transform, false);
                    go1.transform.position = bonusLifeAnchor.position;
                    go2 = GameObject.Instantiate(lifePrefab);
                    go2.transform.SetParent(panelGameOver.transform, false);
                    go2.transform.position = bonusLifeAnchor.position + Vector3.left * 40;
                    break;
                case 3:
                    go1 = GameObject.Instantiate(lifePrefab);
                    go1.transform.SetParent(panelGameOver.transform, false);
                    go1.transform.position = bonusLifeAnchor.position;
                    go2 = GameObject.Instantiate(lifePrefab);
                    go2.transform.SetParent(panelGameOver.transform, false);
                    go2.transform.position = bonusLifeAnchor.position + Vector3.left * 40;
                    var go3 = GameObject.Instantiate(lifePrefab);
                    go3.transform.SetParent(panelGameOver.transform, false);
                    go3.transform.position = bonusLifeAnchor.position + Vector3.left * 80;
                    break;
            }
        }
        else
        {
            textMessage.text = "Insert Coin";
            textBonusTime.enabled = false;
            textBonusLife.enabled = false;
        }

        panelGameOver.SetActive(true);
    }

    private string intSeconds2textMMSS(int seconds)
    {
        int mm = seconds / 60;
        int ss = seconds % 60;
        return mm.ToString("00") + ":" + ss.ToString("00");
    }
}
