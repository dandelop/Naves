using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public float speed = 1f;
    private SpriteRenderer _spriteRenderer;
    
    void Start()
    {
        _spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GameIsPaused)
            return;
        
        if (_spriteRenderer.isVisible)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - (speed * Time.deltaTime));    
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var component = col.gameObject.GetComponent<ControlPlayer>();
        if (component != null)
        {
            component.HitByShoot();
            GameObject.Destroy(this.gameObject);
        }
    }
}
