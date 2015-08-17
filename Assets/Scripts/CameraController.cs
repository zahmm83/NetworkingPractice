using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public Transform target;
    public float lookSmooth = 0.03f;
    public Vector3 offsetFromTarget = new Vector3(0, 0.5f, -1);
    public float xTilt = 30;
    public PracticeCharacterController character;

    Vector3 destination = Vector3.zero;
    float rotateVeloctiy = 0;


    void Start()
    {
        SetCameraTarget(target);
    }

    public void SetCameraTarget(Transform transform)
    {
        if (target != null)
        {
            if (target.GetComponent<PracticeCharacterController>())
            {
                character = target.GetComponent<PracticeCharacterController>();
            }
            else
            {
                Debug.LogError("Your camera's target needs a PracticeCharacterController");
            }
        }
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            character.mouseToFloorStart = GetMouseHitTarget();
        }

        if (Input.GetMouseButton(1))
        {
            character.mouseToFloorPosition = GetMouseHitTarget();
        }

        MoveToTarget();
        LookAtTarget();
    }
    
    Vector3 GetMouseHitTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 target = Vector3.zero;
        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag == "Floor")
        {
            target = hit.point;
        }

        Debug.DrawRay(transform.position, ray.direction * 10, Color.red);

        return target;
    }

    void TrackMouse()
    {
              
    }

    void MoveToTarget()
    {
        if(character != null)
        {
            destination = character.TargetRotation * offsetFromTarget;
            destination += target.position;
            transform.position = destination;
        }
    }

    void LookAtTarget()
    {
        if (target != null)
        {
            float eulerYAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, target.eulerAngles.y, ref rotateVeloctiy, lookSmooth);
            transform.rotation = Quaternion.Euler(xTilt, eulerYAngle, 0);
        }
    }
}
