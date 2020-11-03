using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public MazePart currentMazePart;

    private PlayerManager localPlayerManager;

    // since the old player character is destroyed every time a new maze is created this boolean will always be false when a new maze is generated
    private bool waterHasStarted = false;

    private void Start()
    {
        localPlayerManager = GameInstance.Instance.playerManager;
    }

    // will move the player character in one of four directions,
    // will also call StartWater if this is the first time the player moves 
    public void Move(MazePart newMazePart, Direction direction)
    {
        if (!waterHasStarted)
        {
            waterHasStarted = true;
            localPlayerManager.StartWater();
        }

        switch (direction)
        {
            case Direction.Left:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                break;
            case Direction.Up:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                break;
            case Direction.Right:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
                break;
            case Direction.Down:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                break;
        }

        transform.position = newMazePart.transform.position;
        currentMazePart = newMazePart;

        localPlayerManager.CheckForPlayerWinOrLoss(currentMazePart);
    }
}
