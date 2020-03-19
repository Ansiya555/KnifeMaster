using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeForce : MonoBehaviour
{
    [SerializeField] Vector2 direction;
    [SerializeField] float delay;
    [SerializeField] float gravity;

   int steps=5;
    // Start is called before the first frame update
    Rigidbody2D rigidbody2D;
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.AddRelativeForce(direction);
        StartCoroutine(GravityActivator());
     //   StartCoroutine(Rotate());
        rigidbody2D.gravityScale = 1;
    }

    IEnumerator GravityActivator()
    {
        yield return new WaitForSeconds(delay);
        //rigidbody2D.AddForce(-direction);
        rigidbody2D.gravityScale = gravity;
    }

    IEnumerator Rotate()
    {
        for (int i = 0; i < steps; i++)
        {
            transform.Rotate(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, (Mathf.Round(transform.localRotation.eulerAngles.z * 10) / 10) + (30f / steps));
            yield return new WaitForSeconds(.15f);
        }
    }
}
