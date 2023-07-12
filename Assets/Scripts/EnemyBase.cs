using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyBase : MonoBehaviour
{
    public enum Status
    {
        Formation,
        AttackLaunch,
        Attack,
        Reentry
    }

    [Header("Attack")]
    public float speedAttack = 3f;
    public float periodAttack = 6f;
    public float hitAttack = 0.2f;
    public float periodFire = 3f;
    public float hitFire = 0.1f;
    [SerializeField] private GameObject shootPrefab;

    [Header("Formation")]
    public int row = 0;
    public int col = 0;
    public EnumEnemy enemyType = EnumEnemy.Blue;
    
    [Header("Animation")]
    public AnimationClip animFormation;
    public AnimationClip animAttack;
    public AnimationClip animExplotion;

    [Header("Sound")]
    public AudioClip soundExplotion;
    public AudioClip soundAttack;

    [Header("Power Up")] 
    public GameObject coinPrefab;
    private float _hitPowerup = 0.2f;

    private Animator _animator;
    private AudioSource _audioSource;

    protected EnumDirection _attackDirection;
    private float _tpChangeAttackDirection;
    private float _tpMovingDown;
    private float _tpToAttack;
    private float _tpToFire;
    
    private Status _status = Status.Formation;

    protected const float _TP_ATTACK_LAUNCH = 4f;
    protected float _tpAttackLaunch = _TP_ATTACK_LAUNCH;
    private float _vectorDelta = 0.05f;
    private bool _die = false;
    protected float _radio = 2f;
    protected Vector3 _centerXY;
    private float _tpReentry;
    private Vector3 _reentryPosition0;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        _animator = this.gameObject.GetComponent<Animator>();
        _audioSource = this.gameObject.GetComponent<AudioSource>();
        // Añade retardo inicial y cierto desfase aleatorio (para que no todas las naves empiecen a dispara a la vez)
        _tpToAttack = periodAttack * Random.Range(1, 3);
        _tpToFire = periodFire * Random.Range(1, 2);
        GameManager.Formation.AddShip(this, row, col);
        switch (enemyType)
        {
            case EnumEnemy.Blue:
                // Los Blue no cambian de dirección en el ataque (_tpChangeAttackDirection = "infinito")
                _tpChangeAttackDirection = float.MaxValue;
                break;
            case EnumEnemy.Purple:
                _tpChangeAttackDirection = 0.6f;
                break;
            case EnumEnemy.Red:
                _tpChangeAttackDirection = 1f;
                break;
            case EnumEnemy.Flagship:
                _tpChangeAttackDirection = 1f;
                break;
        }
        EnemyShoot shoot = shootPrefab.GetComponent<EnemyShoot>();
        shoot.speed = Mathf.Max(shoot.speed, speedAttack); // establece la velocidad de disparo al menos a la de la nave
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStatus();
        UpdatePhysic();
        UpdateGraphic();
        UpdateSound();
    }

    private void UpdateGraphic()
    {
        if (_die)
            return;
        
        // cambios de estado
        switch (_status)
        {
            case Status.Formation:
                _animator.Play(animFormation.name);
                break;
            case Status.AttackLaunch:
                _animator.Play(animAttack.name);
                break;
            case Status.Attack:
                _animator.Play(animFormation.name);
                break;
            case Status.Reentry:
                _animator.Play(animFormation.name);
                break;
        }
    }

    private void UpdateSound()
    {
        if (_die)
            return;
        
        // cambios de estado
        switch (_status)
        {
            case Status.Formation:
                _audioSource.Stop();
                break;
            case Status.AttackLaunch:
                _audioSource.clip = soundAttack;
                if (!_audioSource.isPlaying)
                    _audioSource.Play();
                break;
            case Status.Attack:
                break;
            case Status.Reentry:
                _audioSource.Stop();
                break;
        }
    }
    private void UpdatePhysic()
    {
        switch (_status)
        {
            case Status.Formation:
                this.transform.position = GameManager.Formation.GetShipPosition(row, col);
                this.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case Status.AttackLaunch:
                // interpolacion entre 0 .. pi
                float angleLaunch = (Mathf.PI / _TP_ATTACK_LAUNCH) * (_TP_ATTACK_LAUNCH - _tpAttackLaunch);
                var dir = _attackDirection == EnumDirection.Left ? 1 : -1;
                transform.position = new Vector3(
                    _centerXY.x + dir * (_radio * Mathf.Cos(angleLaunch)),
                    _centerXY.y + (_radio * Mathf.Sin(angleLaunch)));
                break;
            case Status.Attack:
                if (_tpMovingDown < 0f)
                {
                    _tpMovingDown = _tpChangeAttackDirection;
                    if (Random.Range(0f, 1f) > 0.5f)
                    {
                        ChangeAttackDirection();
                    }                    
                }
                // rotación de ataque
                float angleAttack = Vector3.Angle(transform.position, GameManager.Player.transform.position);
                transform.rotation = Quaternion.Euler(0, 0, angleAttack);
                // posición de ataque
                transform.position = new Vector3(
                    transform.position.x + (speedAttack * Time.deltaTime) * (_attackDirection == EnumDirection.Left ? 1 : -1), 
                    transform.position.y - (speedAttack * Time.deltaTime));
                break;
            case Status.Reentry:
                // reentrada
                if (transform.position.y < -5.2f)
                {
                    transform.position = new Vector3(transform.position.x, 5.2f);
                    _reentryPosition0 = transform.position;
                }
                // vuelta a la posicion original
                Vector3 to = GameManager.Formation.GetShipPosition(row, col);
                float distanceToFormationPosition = Vector3.Distance(_reentryPosition0, to);
                float distanceCovered = (Time.time - _tpReentry) * speedAttack;
                float fractionOfDistance = distanceCovered / distanceToFormationPosition;
                transform.position = Vector3.Lerp(_reentryPosition0, to, fractionOfDistance);
                // rotación de reentrada
                float angleReentry = Vector3.Angle(transform.position, to);
                transform.rotation = Quaternion.Euler(0, 0, angleReentry);
                break;
        }
    }

    protected virtual void ChangeAttackDirection()
    {                
        _attackDirection = 1 - _attackDirection;  // cambia la dirección de izquierda (0) a derecha (1)       
    }

    private void UpdateStatus()
    {
        if (_die)
            return;
        
        // el tiempo desde el último disparo se actualiza siempre (independiente del estado)
        _tpToFire -= Time.deltaTime;
        if (_tpToFire < 0)
        {
            _tpToFire = periodFire;
            if (Random.Range(0f, 1f) > (1 - hitFire))
            {
                // le pregunta al escuadrón si se está replegando... y solo dispara en caso de no repliegue
                if (!GameManager.Formation.Retracting)
                    Fire();
            }
        }
        
        // cambios de estado
        switch (_status)
        {
            case Status.Formation: 
                // tiempo hasta el próximo ataque                
                _tpToAttack -= Time.deltaTime;
                if (_tpToAttack < 0)
                {
                    _tpToAttack = periodAttack;
                    // Además del componente aleatorio + un componente según los vecinos de formación                     
                    if ((Random.Range(0f, 1f) > (1 - hitAttack)) && GameManager.Formation.CanAttack(row, col))
                    {
                        BeginAttack(GameManager.Formation.Direction);
                    }                    
                }
                break;
            case Status.AttackLaunch:
                // incia el lanzamiento del ataque
                _tpAttackLaunch -= (Time.deltaTime * speedAttack);
                if (_tpAttackLaunch <= 0)
                {
                    _status = Status.Attack;
                    _tpMovingDown = _tpChangeAttackDirection;
                }
                break;
            case Status.Attack:
                _tpMovingDown -= Time.deltaTime;
                // reentrada por la parte superior de la pantalla
                if (transform.position.y < -5.2f)
                {
                    _status = Status.Reentry;
                    _tpReentry = Time.time;
                }
                break;
            case Status.Reentry:
                //if (transform.position == GameManager.Formation.GetShipPosition(row, col))
                if (Vector3.SqrMagnitude(transform.position - GameManager.Formation.GetShipPosition(row, col)) < _vectorDelta)
                {
                    _status = Status.Formation;
                }
                break;
        }
    }

    protected virtual void BeginAttack(EnumDirection direction)
    {
        _status = Status.AttackLaunch;
        _attackDirection = direction;
        var pos = GameManager.Formation.GetShipPosition(row, col);
        _centerXY = new Vector3(pos.x + (GameManager.Formation.Direction == EnumDirection.Left ? -_radio : _radio),
            pos.y, pos.z);
        _tpAttackLaunch = _TP_ATTACK_LAUNCH;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (_die)
            return;
        
        var component = col.gameObject.GetComponent<ControlPlayer>();
        if (component != null)
        {
            component.HitByEnemy();
            Die();
        }
    }

    public void HitByShoot()
    {
        if (Random.Range(0f, 1f) > (1 - _hitPowerup))
        {
            GameManager.createCoin(coinPrefab, transform);
        }
        GameManager.addPoints(100);
        Die();
    }

    private void Die()
    {
        if (_die)
            return;
        _die = true;
         GameManager.removeEnemy();
        _animator.Play(animExplotion.name);
        _audioSource.clip = soundExplotion;     
        _audioSource.Play();
        //GameObject.Destroy(this.gameObject, Mathf.Max(animExplotion.length, soundExplotion.length));
         GameObject.Destroy(this.gameObject, animExplotion.length);
    }
    
    private void Fire()
    {
        GameObject go = GameObject.Instantiate(shootPrefab);
        go.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.2f);
    }

    private void OnDrawGizmos()
    {
        // DEBUG
        /*
        if (_status == Status.Formation) {
            Gizmos.color = Color.red;
            Gizmos.color = Color.red;
            //Gizmos.DrawWireSphere(new Vector3(this.transform.position.x + (GameManager.Formation.Direction == EnumDirection.Left ? -_radio : _radio),this.transform.position.y, this.transform.position.z), _radio);
            Gizmos.DrawWireSphere(_centerXY, _radio);
        }        
        if (_status == Status.Attack)
        {
            Gizmos.DrawLine(transform.position, GameManager.Player.transform.position);
        }
        if (_status == Status.Reentry)
        {
            Gizmos.DrawLine(transform.position, GameManager.Formation.GetShipPosition(row, col));
        }
        */
    }

    private void OnDestroy()
    {
        GameManager.Formation.DelShip(row, col);
    }
}
