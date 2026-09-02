using UnityEngine;
using Unity.Cinemachine;

public class CameraBoundaryTrigger : MonoBehaviour
{
    public CinemachineConfiner3D confiner;
    public Collider targetBorder;

    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        confiner.BoundingVolume = targetBorder;
    }
}