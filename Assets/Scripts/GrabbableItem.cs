using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableItem : MonoBehaviour
{
    private Rigidbody rb;
    private Collider[] colliders;

    public bool IsGrabbed { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
    }

    public void Grab()
    {
        IsGrabbed = true;

        rb.isKinematic = true;
        rb.useGravity = false;

        foreach (var col in colliders)
        {
            col.enabled = false;
        }
    }

    public void Drop()
    {
        IsGrabbed = false;

        rb.isKinematic = false;
        rb.useGravity = true;

        foreach (var col in colliders)
        {
            col.enabled = true;
        }
    }
}