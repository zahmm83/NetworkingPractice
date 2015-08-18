using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class PlayerSyncRotation : NetworkBehaviour {

    [SyncVar]
    private Quaternion syncPlayerRotation;

    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private float lerpRate = 15;

    private Quaternion lastRotation;
    private float threshold = 5f;


    void Update()
    {
        LerpRotation();
    }

    void FixedUpdate ()
    {
        TransmitRotation();
    }

    void LerpRotation()
    {
        if (!isLocalPlayer)
        {
            playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, syncPlayerRotation, Time.deltaTime * lerpRate);
        }
    }

    [Command]
    void CmdProvideRotationToServer(Quaternion rotation)
    {
        syncPlayerRotation = rotation;
    }

    [ClientCallback]
    void TransmitRotation()
    {
        if (isLocalPlayer && hasTurnedPastThreshold)
        {
            CmdProvideRotationToServer(playerTransform.rotation);
            lastRotation = playerTransform.rotation;
        }
    }

    bool hasTurnedPastThreshold
    {
        get { return Quaternion.Angle(playerTransform.rotation, lastRotation) > threshold; }
    }
}
