using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakinfForce : MonoBehaviour
{
    [SerializeField] Vector2 direction;
    [SerializeField] float delay;
    [SerializeField] float gravity;
    [SerializeField] Quaternion finalRotValue;
     float steps =5;

    // Start is called before the first frame update
    Rigidbody2D rigidbody2D;
    void Start()
    {
         rigidbody2D = GetComponent<Rigidbody2D>();
         rigidbody2D.AddForce(direction);
        StartCoroutine(GravityActivator());
        rigidbody2D.gravityScale = 1;
        StartCoroutine(Rotate());
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
            transform.Rotate(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, (Mathf.Round(transform.localRotation.eulerAngles.z * 10) / 10) + (finalRotValue.z/steps));
            yield return new WaitForSeconds(.15f);
        }
    }


}
