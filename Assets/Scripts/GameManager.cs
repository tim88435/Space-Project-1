using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private void OnMoveUnit(InputValue inputValue)
    {
        if (inputValue.Get<float>() > 0)
        {
            foreach (FlockAgent item in Resources.FindObjectsOfTypeAll(typeof(FlockAgent)))
            {
                item.targetDestination = CameraControl.Singleton.MousePositionWorld();
            }
        }
    }
}
