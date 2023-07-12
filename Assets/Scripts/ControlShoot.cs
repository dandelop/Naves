using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ControlShoot : MonoBehaviour
{
    public float speed = 1f;

    //public ControlPlayer controlPlayer;
    
    private SpriteRenderer _spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_spriteRenderer.isVisible)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + (speed * Time.deltaTime));    
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var component = col.gameObject.GetComponent<EnemyBase>();
        if (component != null)
        {
            component.HitByShoot();
            GameObject.Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Player != null)
        GameManager.Player.canFire();
    }
}
