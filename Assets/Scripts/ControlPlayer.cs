using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ControlPlayer : MonoBehaviour
{
    public float speed = 1f;

    public float maxLeft;
    public float maxRight;

    public GameObject shootPrefab;
    public SpriteRenderer laserSpriteRenderer;

    [Header("Animation")]    
    public AnimationClip animIdle;
    public AnimationClip animExplotion;
    public AnimationClip animShield;
    
    [Header("Sound")]
    public AudioClip soundExplotion;
    public AudioClip soundPowerup;

    private bool _canFire = true;

    private bool _shield = false;
    
    private bool _isDiying = false;
    private const float _TP_DIYING_ = 3f;
    private float _tpDiying = _TP_DIYING_;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private AudioSource _audioSource;

    private void Awake()
    {
        GameManager.Player = this;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        if (_isDiying)
        {
            _tpDiying -= Time.deltaTime;
            // Modo Dios (para cuando revive)
            if (_tpDiying < 0)
            {
                _isDiying = false;
                _spriteRenderer.color = Color.white;                
            }
            else
            {
                int tpFlash = ((int)Mathf.Floor((_TP_DIYING_ - _tpDiying) / 0.2f)) % 2;
                _spriteRenderer.color = tpFlash == 0 ? Color.white : Color.red;
            }            

            // Fin de animaci�n de explosi�n
            if ((_TP_DIYING_ - _tpDiying) > (animExplotion.length * 2))
            {
                if (GameManager.playerLifes <= 0)
                {
                    gameObject.SetActive(false);
                }
                _animator.Play(animIdle.name);
            }
        }
        else
        {
            if (GameManager.GameIsPaused)
                return;

            // Despliega el escudo
            if (Input.GetKey(KeyCode.S))
            {
                _shield = true;
                _animator.Play(animShield.name);
                laserSpriteRenderer.enabled = false;
            }
            else
            {
                // mientras se tiene el escudo activo, no se puede disparar... si no ser�a excesivamente f�cil
                _shield = false;
                _animator.Play(animIdle.name);
                laserSpriteRenderer.enabled = true;
                if (Input.GetKeyDown(KeyCode.Space))
                {                    
                    if (_canFire)
                    {
                        _canFire = false;
                        GameObject go = GameObject.Instantiate(shootPrefab);
                        go.transform.position = laserSpriteRenderer.gameObject.transform.position;
                        laserSpriteRenderer.enabled = false;
                    }
                }
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.position = new Vector3(Mathf.Max(maxLeft, transform.position.x - (speed * Time.deltaTime)),
                    transform.position.y);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.position = new Vector3(Mathf.Min(maxRight, transform.position.x + (speed * Time.deltaTime)),
                    transform.position.y);
            }

            
        }
    }

    public void canFire()
    {
        _canFire = true;        
        laserSpriteRenderer.enabled = true;
    }
    
    public void HitByShoot()
    {
        if (!_shield)
            Die();
    }

    public void HitByEnemy()
    {
        if (!_shield)
            Die();
    }
    
    private void Die()
    {
        if (_isDiying)
            return;
        
        _isDiying = true;
        _tpDiying = _TP_DIYING_;
        _animator.Play(animExplotion.name);
        _audioSource.clip = soundExplotion;
        _audioSource.Play();
        GameManager.playerDie();
    }

    public void HitByCoin()
    {
        _audioSource.clip = soundPowerup;
        _audioSource.Play();
        GameManager.addPoints(300);
    }
}
