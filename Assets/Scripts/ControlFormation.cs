using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UIElements;

public class ControlFormation : MonoBehaviour
{
    public float Speed = 3f;
    public float Period = 4f;
    public EnumDirection Direction = EnumDirection.Left;

    private const int ROWS = 6;
    private const int COLS = 10;
    private EnemyBase[,] arrayShips;
    private Vector3[,] arrayShipPositions;
    private float _period;
    private int _shipsAlive = 0;
    private bool _retracting = false;
    public bool Retracting => _retracting;
    private const float _TP_RETRACTING_ = 5f;
    private float _tpRetracting = _TP_RETRACTING_;
    private void Awake()
    {
        GameManager.Formation = this;
        arrayShips = new EnemyBase[ROWS,COLS];
        arrayShipPositions = new Vector3[ROWS, COLS];
        _period = Period / 2;   // el "primer pase" parte desde el centro -> es la mitad de largo que los normales (de extremo a extremo)
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _period -= Time.deltaTime;
        if (_period <= 0f)
        {
            _period = Period;
            Direction = 1 - Direction;
        }

        if (_retracting)
        {
            _tpRetracting -= Time.deltaTime;
            if (_tpRetracting < 0)
            {
                _retracting = false;
            }
        }

        UpdatePositions();
    }

    private void UpdatePositions()
    {
        int dir = (Direction == EnumDirection.Left ? -1 : 1);

        for (int i = 0; i < ROWS; i++)
        {
            for (int j = 0; j < COLS; j++)
            {
                if (arrayShipPositions[i, j] != null)
                {
                    arrayShipPositions[i, j] =
                        new Vector3(arrayShipPositions[i, j].x + (dir * Speed * Time.deltaTime),
                        arrayShipPositions[i, j].y, arrayShipPositions[i, j].z);
                }
            }
        }
    }
    
    public void AddShip(EnemyBase ship, int row, int col)
    {        
        if ((row >= 0) && (row < ROWS) && (col >= 0) && (col < COLS))
        {
            arrayShips[row, col] = ship;
            arrayShipPositions[row, col] = ship.transform.position;
            _shipsAlive++;
            GameManager.NumEnemenies = _shipsAlive;
        }
        else
        {
            throw new Exception("Invalid position");
        }
    }
    
    public void DelShip(int row, int col)
    {
        if ((row >= 0) && (row < ROWS) && (col >= 0) && (col < COLS))
        {
            arrayShips[row, col] = null;
            _shipsAlive--;
        }
        else
        {
            throw new Exception("Invalid position");
        }

    }

    public Vector3 GetShipPosition(int row, int col)
    {
        if ((row >= 0) && (row < ROWS) && (col >= 0) && (col < COLS))
        {
            return arrayShipPositions[row, col];
        }
        else
        {
            throw new Exception("Invalid position");
        }        
    }

    public bool CanAttack(int row, int col)
    {
        if (_retracting)
            return false;
        
        // solo se puede atacar cuando se tiene un máximo de 3 vecinos (vertical / horizontal)
        if ((row >= 0) && (row < ROWS) && (col >= 0) && (col < COLS))
        {
            List<Tuple<int,int>> vecinos = new List<Tuple<int, int>>();
            int numVecinos = 0;
            // vecinos horizontales
            if ((row - 1) >= 0)
                vecinos.Add(new Tuple<int, int>(row - 1, col));
            if ((row + 1) < ROWS)
                vecinos.Add(new Tuple<int, int>(row + 1, col));
            // vecinos verticales
            if ((col - 1) >= 0)
                vecinos.Add(new Tuple<int, int>(row, col - 1));
            if ((col + 1) < COLS)
                vecinos.Add(new Tuple<int, int>(row, col + 1));
            foreach (var vecino in vecinos)
            {
                if (arrayShips[vecino.Item1, vecino.Item2] != null)
                    numVecinos++;
            }
            return numVecinos <= 3;
        }
        else
        {
            throw new Exception("Invalid position");
        }
    }

    public void Retract()
    {
        _retracting = true;
        _tpRetracting = _TP_RETRACTING_;
    }
}
