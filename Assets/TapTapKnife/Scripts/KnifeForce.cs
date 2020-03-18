using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeForce : MonoBehaviour
{
    [SerializeField] Vector2 direction;
    [SerializeField] float delay;
    [SerializeField] float gravity;

    // Start is called before the first frame update
    Rigidbody2D rigidbody2D;
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.AddRelativeForce(direction);
        StartCoroutine(GravityActivator());
        rigidbody2D.gravityScale = 1;
    }

    IEnumerator GravityActivator()
    {
        yield return new WaitForSeconds(delay);
        //rigidbody2D.AddForce(-direction);
        rigidbody2D.gravityScale = gravity;
    }
}
