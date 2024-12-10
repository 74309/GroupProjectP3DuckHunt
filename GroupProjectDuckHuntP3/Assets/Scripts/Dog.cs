using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour
{
    public Vector2 jumpPosition;

    Animator anim;
    bool isDead;
    Vector2 upPos;
    float delay = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, jumpPosition) > 0 && !isDead)
            transform.position = Vector2.MoveTowards(transform.position, jumpPosition, 3 * Time.deltaTime);
        else
        {

            anim.SetTrigger("Jump");
            if (!isDead)
            {

                isDead = true;
                Destroy(gameObject, 1f);
                upPos = transform.position + transform.up;
                GameManager.Instance.CallCreateDucks();
            }
            if (delay <= 0)
                transform.position = Vector3.Lerp(transform.position, upPos, 10 * Time.deltaTime);
            else
                delay -= Time.deltaTime;
        }
    }
}
