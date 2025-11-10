using UnityEngine;
using UnityEngine.EventSystems;

public class TouchRotator : MonoBehaviour
{
    private float rotationSpeed = 0.2f;
    private Vector2 lastTouchPosition;

    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(0))
            return;

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                float rotationY = touch.deltaPosition.x * rotationSpeed;
                transform.Rotate(0, -rotationY, 0, Space.World);
            }
        }
    }
}
