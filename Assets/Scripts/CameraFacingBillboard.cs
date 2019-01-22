using UnityEngine;
using System.Collections;
// from : https://wiki.unity3d.com/index.php?title=CameraFacingBillboard
public class CameraFacingBillboard : MonoBehaviour
{
    protected Camera m_Camera;

    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate()
    {
        m_Camera = Camera.main;

        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
            m_Camera.transform.rotation * Vector3.up);
    }
}