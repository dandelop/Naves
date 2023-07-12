using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float speed = 3f;
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    { 
        transform.position = new Vector3(transform.position.x, transform.position.y - (speed * Time.deltaTime));    
        
        if (transform.position.y < -5.2f)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var component = col.gameObject.GetComponent<ControlPlayer>();
        if (component != null)
        {
            component.HitByCoin();
            GameObject.Destroy(this.gameObject);
        }
    }
}
