using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {

    public float radius = 5.0f;
    public float power = 10.0f;


    void Start () {
    }

    void OnCollisionEnter()
    {
        Vector3 explosionPos = transform.position;

        foreach (Collider hit in Physics.OverlapSphere(explosionPos, radius))
        {
            Rigidbody body = hit.GetComponent<Rigidbody>();
            if (body != null)
            {
                body.AddExplosionForce(power, explosionPos, radius, 0.0f);
            }
        }
    }
	
}
