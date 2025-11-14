using Unity.Netcode;
using UnityEngine;

public class ActivateCameraForOwner : NetworkBehaviour
{
    public Camera cam;
    public AudioListener listener;

    private void Start()
    {
        if (!IsOwner)
        {
            cam.enabled = false;
            listener.enabled = false;
        }
        else
        {
            cam.enabled = true;
            listener.enabled = true;
        }
    }
}
