using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public RectTransform background;
    public RectTransform handle;
    public float handleRange = 100f;

    [HideInInspector]
    public Vector2 InputDirection = Vector2.zero;

    public bool isLocked = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isLocked)
        {
            // Unlock on click
            Unlock();
            FindObjectOfType<scr_CherectorController>().OnJoystickClicked();
        }
        else
        {
            OnDrag(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked)
            return;

        Vector2 position = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, eventData.pressEventCamera, out position);
        position /= handleRange;

        InputDirection = Vector2.ClampMagnitude(position, 1.0f);

        handle.anchoredPosition = InputDirection * handleRange;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isLocked)
            return;

        InputDirection = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    public void Lock()
    {
        isLocked = true;

        // Force handle to max forward
        InputDirection = new Vector2(0, 1);
        handle.anchoredPosition = InputDirection * handleRange;
    }

    public void Unlock()
    {
        isLocked = false;

        // Reset joystick handle to center
        InputDirection = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

}
