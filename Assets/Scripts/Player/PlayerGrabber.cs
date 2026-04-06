using UnityEngine;

public class PlayerGrabber : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private Camera playerCamera;

    [Header("Grab Settings")]
    [SerializeField] private float grabDistance = 3f;
    [SerializeField] private LayerMask grabbableLayer;
    [SerializeField] private float holdSmoothSpeed = 12f;

    private GrabbableItem currentItem;
    private Rigidbody currentItemRb;

    private float grabbedDistance;
    private bool interactWasPressedLastFrame;

    private Vector3 lastHeldPosition;
    private Vector3 heldVelocity;

    private void Update()
    {
        HandleGrabInput();
        UpdateHeldItemPosition();
    }

    private void HandleGrabInput()
    {
        bool grabPressed = inputHandler.GrabTriggered;

        if (grabPressed && !interactWasPressedLastFrame)
        {
            if (currentItem == null)
            {
                TryGrabItem();
            }
            else
            {
                DropItem();
            }
        }

        interactWasPressedLastFrame = grabPressed;
    }

    private void TryGrabItem()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // Check for grabbable objects in front of the player
        if (Physics.Raycast(ray, out RaycastHit hit, grabDistance, grabbableLayer))
        {
            GrabbableItem item = hit.collider.GetComponentInParent<GrabbableItem>();

            if (item != null && !item.IsGrabbed)
            {
                currentItem = item;
                currentItemRb = item.GetComponent<Rigidbody>();

                grabbedDistance = Vector3.Distance(playerCamera.transform.position, item.transform.position);

                currentItem.Grab();

                lastHeldPosition = currentItem.transform.position;
                heldVelocity = Vector3.zero;
            }
        }
    }

    private void UpdateHeldItemPosition()
    {
        if (currentItem == null) return;

        Vector3 targetPosition = playerCamera.transform.position +
                                 playerCamera.transform.forward * grabbedDistance;

        currentItem.transform.position = Vector3.Lerp(
            currentItem.transform.position,
            targetPosition,
            holdSmoothSpeed * Time.deltaTime
        );

        heldVelocity = (currentItem.transform.position - lastHeldPosition) / Time.deltaTime;
        lastHeldPosition = currentItem.transform.position;
    }

    private void DropItem()
    {
        if (currentItem == null) return;

        currentItem.Drop();

        if (currentItemRb != null)
        {
            currentItemRb.linearVelocity = heldVelocity;
        }

        currentItem = null;
        currentItemRb = null;
    }
}