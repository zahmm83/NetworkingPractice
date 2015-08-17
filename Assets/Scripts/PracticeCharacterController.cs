using UnityEngine;
using System.Collections;

public class PracticeCharacterController : MonoBehaviour {

    public float inputDelay = 0.1f;
    public float forwardVelocity = 12;
    public float rotateVelocity = 100;
    public Vector3 mouseToFloorStart;
    public Vector3 mouseToFloorPosition;

    public Rigidbody projectile;
    public Rigidbody explosiveProjectile;

    Quaternion targetRotation;
    Rigidbody body;
    float forwardInput;
    float turnInput;
    int burstCounter = 0;

    public Quaternion TargetRotation
    {
        get { return targetRotation; }
    }


    void Start()
    {
        targetRotation = transform.rotation;
        if (GetComponent<Rigidbody>())
        {
            body = GetComponent<Rigidbody>();
        }
        else
        {
            Debug.LogError("The character needs a rigid body.");
        }

        forwardInput = turnInput = 0;
    }

    void GetInput()
    {
        forwardInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
    }

    void Update()
    {
        GetInput();

        if (Input.GetMouseButton(1))
        {
            TurnWithMouse();
        }
        else
        {
            TurnWithKeyboard();
        }
        
        if (Input.GetMouseButtonDown(0) || burstCounter != 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                FireExplosiveProjectile();
            }
            else
            {
                burstCounter = (burstCounter + 1) % 4;
                FireProjectile();
            }
        }
    }

    void FixedUpdate()
    {
        Run();
    }
    
    void Run()
    {
        if (Mathf.Abs(forwardInput) > inputDelay)
        {
            body.velocity = transform.forward * forwardInput * forwardVelocity;
        }
        else
        {
            body.velocity = Vector3.zero;
        }
    }

    void TurnWithKeyboard()
    {
        if (Mathf.Abs(turnInput) > inputDelay)
        {
            targetRotation *= Quaternion.AngleAxis(rotateVelocity * turnInput * Time.deltaTime, Vector3.up);
        }

        transform.rotation = targetRotation;
    }

    void TurnWithMouse()
    {
        float angleBetween = Vector3.Angle(transform.forward, mouseToFloorPosition - transform.position);
        // Using the pure angle measure is way too touchy, so we scale it and clamp.
        angleBetween = Mathf.Clamp(angleBetween * 0.025f, 0, 2);


        // Using the fact that the orientation of a triangle is found by the determinant of a specific matrix which 
        // tells us whether our mouse point is on the "left" or "right" side of the character.
        Vector3 pointOnLine = transform.forward + transform.position;
        float[,] triangleMatrix = { { 1, transform.position.x, transform.position.z }, { 1, pointOnLine.x, pointOnLine.z }, { 1, mouseToFloorPosition.x, mouseToFloorPosition.z } };
        float subDet1 = triangleMatrix[1, 1] * triangleMatrix[2, 2] - triangleMatrix[1, 2] * triangleMatrix[2, 1];
        float subDet2 = triangleMatrix[1, 0] * triangleMatrix[2, 2] - triangleMatrix[1, 2] * triangleMatrix[2, 0];
        float subDet3 = triangleMatrix[1, 0] * triangleMatrix[2, 1] - triangleMatrix[1, 1] * triangleMatrix[2, 0];

        // Though the orientations are backwards from what I want, so * -1
        float determinant = -1 * (triangleMatrix[0, 0] * (subDet1) - triangleMatrix[0, 1] * (subDet2) + triangleMatrix[0, 2] * (subDet3));

        if (angleBetween > 0.2f)
        {
            targetRotation *= Quaternion.AngleAxis(angleBetween * Mathf.Sign(determinant), Vector3.up);
        }

        transform.rotation = targetRotation;
    }

    void FireProjectile()
    {
        Vector3 spawnLocation = transform.GetChild(1).position + transform.forward * 0.2f;
        Rigidbody projectileClone = (Rigidbody)Instantiate(projectile, spawnLocation, transform.GetChild(1).rotation);
        projectileClone.velocity = transform.forward * 10;
    }

    void FireExplosiveProjectile()
    {
        Vector3 spawnLocation = transform.GetChild(1).position + transform.forward * 0.5f;
        Rigidbody projectileClone = (Rigidbody)Instantiate(explosiveProjectile, spawnLocation, transform.GetChild(1).rotation);
        projectileClone.velocity = transform.forward * 3f;
    }

}
