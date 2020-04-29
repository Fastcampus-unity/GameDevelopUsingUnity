using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InputController
{
    // Update is called once per frame
    public void UpdateInput()
    {
        if (SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().CurrentGameState != GameState.Running)
            return;

        UpdateKeyboard();
        UpdateMouse();
    }

    void UpdateKeyboard()
    {
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            moveDirection.y = 1;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            moveDirection.y = -1;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            moveDirection.x = -1;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            moveDirection.x = 1;
        }

        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().Hero.ProcessInput(moveDirection);

    }

    void UpdateMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().Hero.Fire();
        }

        if (Input.GetMouseButtonDown(1))
        {
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().Hero.FireBomb();
        }

    }
}
