using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;


public class PlayerSyncPosition : NetworkBehaviour {

    [SyncVar (hook = "SyncPositionValues")]
    private Vector3 syncPos;

    [SerializeField]
    Transform myTransform;
    [SerializeField]
    bool useHistoryLerping = false;

    float lerpRate = 15;
    float normalLerpRate = 16;
    float fastLerpRate = 27;

    Vector3 lastPosition;
    float threshold = 0.1f;
    float closeEnoughToPosition = 0.1f;

    List<Vector3> syncPositionHistory = new List<Vector3>();


    bool hasMovedPastThreshold
    {
        get { return Vector3.Distance(myTransform.position, lastPosition) > threshold; }
    }


    void Update()
    {
        LerpPosition();
    }

    void FixedUpdate () {
        TransmitPosition();
    }


    void LerpPosition()
    {
        if (!isLocalPlayer)
        {
            if (useHistoryLerping)
            {
                HistoryLerp();
            }
            else
            {
                OrdinaryLerp();
            }
        }
    }

    void HistoryLerp()
    {
        if(syncPositionHistory.Count > 0)
        {
            myTransform.position = Vector3.Lerp(myTransform.position, syncPositionHistory[0], Time.deltaTime * lerpRate);
            if(Vector3.Distance(myTransform.position, syncPositionHistory[0]) < closeEnoughToPosition)
            {
                syncPositionHistory.RemoveAt(0);
            }

            lerpRate = syncPositionHistory.Count > 10 ? fastLerpRate : normalLerpRate;
        }
    }

    void OrdinaryLerp()
    {
        myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime * lerpRate);
    }

    [Command]
    void CmdProvidePositionToServer(Vector3 pos)
    {
        syncPos = pos;
    }

    [ClientCallback]
    void TransmitPosition()
    {
        if (isLocalPlayer && hasMovedPastThreshold)
        {
            CmdProvidePositionToServer(myTransform.position);
            lastPosition = myTransform.position;
        }
    }

    [Client]
    void SyncPositionValues(Vector3 latestPosition)
    {
        syncPos = latestPosition;
        syncPositionHistory.Add(syncPos);
    }

}
