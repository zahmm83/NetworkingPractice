using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerNetworkSetup : NetworkBehaviour {
    
    void Start () {
        if (isLocalPlayer)
        {
            PracticeCharacterController character = GetComponent<PracticeCharacterController>();
            character.enabled = true;
            
            Camera.main.GetComponent<CameraController>().target = character.transform;
            Camera.main.GetComponent<CameraController>().character = character;
        }
	}

    	
}
