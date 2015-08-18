using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerId : NetworkBehaviour {

    [SyncVar]
    string playerUniqueIdentity;
    NetworkInstanceId playerNetId;
    Transform myTransform;


    public override void OnStartLocalPlayer()
    {
        GetNetIdentity();
        SetIdentity();
    }
    

    void Awake () {
        myTransform = transform;
	}
	
	void Update () {
	    if(myTransform.name == "" || myTransform.name == "Character(Clone)")
        {
            SetIdentity();
        }
	}

    
    string MakeUniqueName()
    {
        string uniqueName = "Player " + playerNetId.ToString();
        return uniqueName;
    }

    void SetIdentity()
    {
        myTransform.name = !isLocalPlayer ? playerUniqueIdentity : MakeUniqueName();
    }

    [Client]
    void GetNetIdentity()
    {
        playerNetId = GetComponent<NetworkIdentity>().netId;
        CmdTellServerMyIdentity(MakeUniqueName());
    }
    
    [Command]
    void CmdTellServerMyIdentity(string name)
    {
        playerUniqueIdentity = name;
    }

}
