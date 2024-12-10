using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Duck : MonoBehaviour
{
    public float speed;
    Vector3 target;

    float activeTime = 10;
    Animator anim;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    bool isDead;
    bool isStartFalling;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            if (Vector3.Distance(transform.position, target) > 0)
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            else
            {
                if (transform.position.x > 8 || transform.position.x < -8 || transform.position.y > 4)
                {
                    Destroy(gameObject);
                }

                SetTarget();

            }

            if (activeTime > 0)
                activeTime -= Time.deltaTime;

            UpdateSprite();
        }
        else
        {
            if (isStartFalling)
                transform.position = Vector3.MoveTowards(transform.position, transform.position - transform.up, 10*Time.deltaTime);
        }
    }

    public void TimeUp()
    {
        speed *= 2;
        target = transform.position + new Vector3(0, 20, 0);
    }

    private void OnMouseDown()
    {
        if (!isDead)
            StartCoroutine(Dead());
    }

    IEnumerator Dead()
    {
        isDead = true;
        anim.SetTrigger("Die");
        yield return new WaitForSeconds(0.4f);
        isStartFalling = true;
    }

    public void UpdateSprite()
    {
        if (transform.position.x - target.x > 0)
        {
            transform.localScale = new Vector3(-0.7f, 0.7f, 0.7f);
        }
        else
        {
            transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        }
        if (Mathf.Abs(transform.position.x - target.x) < 1)
        {
            anim.SetInteger("Fly", 2);
        }
        else if (Mathf.Abs(transform.position.y - target.y) < 1)
        {
            anim.SetInteger("Fly", 0);
        }
        else
        {
            anim.SetInteger("Fly", 1);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, target);
    }
    public void SetTarget()
    {
        if (activeTime > 0)
        {
            target = target + new Vector3(Random.Range(-12, 12), Random.Range(-12, 12), 0);
            if (target.x > 15)
                target.x = 15;
            if (target.x < -7)
                target.x = -7;
            if (target.y > 6)
                target.y = 6;
            if (target.y < -2)
                target.y = -2;  
        }
    }
}
