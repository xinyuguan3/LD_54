using Assets.Scripts;
using UnityEngine;

public class ArrowKeysDetector : MonoBehaviour, IInputDetector
{
    #if UNITY_EDITOR
    public InputDirection? DetectInputDirection()
    {
        if (Input.GetKeyUp(KeyCode.UpArrow)|| Input.GetKeyUp(KeyCode.W))
            return InputDirection.Top;
        else if (Input.GetKeyUp(KeyCode.DownArrow)|| Input.GetKeyUp(KeyCode.S))
            return InputDirection.Bottom;
        else if (Input.GetKeyUp(KeyCode.RightArrow)|| Input.GetKeyUp(KeyCode.D))
            return InputDirection.Right;
        else if (Input.GetKeyUp(KeyCode.LeftArrow)|| Input.GetKeyUp(KeyCode.A))
            return InputDirection.Left;
        else
            return null;
    }
    #else
    public InputDirection? DetectInputDirection()
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
            {
                var touchPosition = touch.position;
                var screenWidth = Screen.width;
                var screenHeight = Screen.height;
                if (touchPosition.x < screenWidth / 2)
                {
                    if (touchPosition.y < screenHeight / 2)
                        return InputDirection.Left;
                    else
                        return InputDirection.Bottom;
                }
                else
                {
                    if (touchPosition.y < screenHeight / 2)
                        return InputDirection.Right;
                    else
                        return InputDirection.Top;
                }
            }
        }
        return null;
    }
    #endif
}