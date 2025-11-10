using UnityEngine;

public class DogRagdollController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody[] ragdollBodies;
    private Camera mainCam;

    private bool isRagdoll = false;
    private Rigidbody grabbedBody;
    private float grabDistance;
    private Vector3 grabOffset;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        mainCam = Camera.main;

        SetRagdoll(false);
    }

    void Update()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        Ray ray = mainCam.ScreenPointToRay(touch.position);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Rigidbody rb = hit.rigidbody;
                    if (rb != null && System.Array.Exists(ragdollBodies, b => b == rb))
                    {
                        if (!isRagdoll)
                            SetRagdoll(true);

                        grabbedBody = rb;
                        grabDistance = Vector3.Distance(mainCam.transform.position, hit.point);
                        grabOffset = grabbedBody.position - hit.point;
                    }
                }
                break;

            case TouchPhase.Moved:
                if (grabbedBody != null)
                {
                    Vector3 targetPos = ray.GetPoint(grabDistance) + grabOffset;
                    grabbedBody.linearVelocity = (targetPos - grabbedBody.position) * 15f;
                }
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (grabbedBody != null)
                {
                    grabbedBody = null;
                    Invoke(nameof(ReturnToIdle), 2f);
                }
                break;
        }
    }

    private void SetRagdoll(bool active)
    {
        isRagdoll = active;

        if (animator != null)
            animator.enabled = !active;

        foreach (Rigidbody rb in ragdollBodies)
        {
            rb.isKinematic = !active;
            rb.useGravity = active;
        }
    }

    private void ReturnToIdle()
    {
        if (isRagdoll)
        {
            SetRagdoll(false);
            if (animator != null)
                animator.SetTrigger("Idle");
        }
    }
}