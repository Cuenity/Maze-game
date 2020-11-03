using System.Collections;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private bool enoughTimeSinceLastMovement = true;
    private readonly WaitForSeconds waitForSeconds = new WaitForSeconds(0.15f);

    private PlayerManager localPlayerManager;
    private MazeGenerationManager localMazeGenerationManager;
    private UIManager localUIManager;

    private void Start()
    {
        localPlayerManager = GameInstance.Instance.playerManager;
        localMazeGenerationManager = GameInstance.Instance.mazeGenerationManager;
        localUIManager = GameInstance.Instance.uiManager;
    }

    void Update()
    {
        if (enoughTimeSinceLastMovement && localPlayerManager.gameStarted)
        {
            CheckMovementKeys();
        }

        if (EscapePressed())
        {
            localUIManager.CreateNewMazePressed();
        }

        //// for testing only
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    TestingOnlyQuickStart();
        //}
    }

    private void TestingOnlyQuickStart()
    {
        PlayerManager playerManager = GameInstance.Instance.playerManager;
        playerManager.StopWaterSpreadingCoroutines();

        MazeGenerationManager mazeGenerationManager = GameInstance.Instance.mazeGenerationManager;
        mazeGenerationManager.GenerateMaze(20, 20, Algorithm.Iterative);


        GameInstance.Instance.mazeCamera.SetCameraToMiddleOfMaze();
        GameInstance.Instance.uiManager.CreateNewMazePressed();

        playerManager.chosenDifficulty = Difficulty.Easy;
        playerManager.SpawnPlayer(mazeGenerationManager.maze[0, 0]);
    }

    private void CheckMovementKeys()
    {
        if (LeftHeldDown())
        {
            StartCoroutine(TryMoveInDirection(Direction.Left));
        }
        else if (UpHeldDown())
        {
            StartCoroutine(TryMoveInDirection(Direction.Up));
        }
        else if (RightHeldDown())
        {
            StartCoroutine(TryMoveInDirection(Direction.Right));
        }
        else if (DownHeldDown())
        {
            StartCoroutine(TryMoveInDirection(Direction.Down));
        }
    }

    // will move the player character in the specified direction,
    // except if the new place would be outside the grid or there is a wall between the current MazePart and its neighbour
    private IEnumerator TryMoveInDirection(Direction direction)
    {
        enoughTimeSinceLastMovement = false;

        PlayerCharacter currentPlayerCharacter = localPlayerManager.currentPlayerCharacter;

        MazePart potentialNewMazePart = NextMazePart(currentPlayerCharacter, direction);
        if (potentialNewMazePart != null)
        {
            currentPlayerCharacter.Move(potentialNewMazePart, direction);
        }

        yield return waitForSeconds; //0.15f "cooldown" period before the player can move again

        enoughTimeSinceLastMovement = true;
    }

    // returns a neighbour MazePart in the direction the player wants to move
    // or returns null if no MazePart exists in that direction or the wall between them is closed
    private MazePart NextMazePart(PlayerCharacter currentPlayerCharacter, Direction direction)
    {
        if (currentPlayerCharacter.currentMazePart.wallOpen[(int)direction])
        {
            int newXPosition = currentPlayerCharacter.currentMazePart.xPosition;
            int newYPosition = currentPlayerCharacter.currentMazePart.yPosition;

            switch (direction)
            {
                case Direction.Left:
                    newXPosition -= 1;
                    break;
                case Direction.Up:
                    newYPosition += 1;
                    break;
                case Direction.Right:
                    newXPosition += 1;
                    break;
                case Direction.Down:
                    newYPosition -= 1;
                    break;
            }

            if (AllowedToMoveToNextMazePart(newXPosition, newYPosition))
            {
                return localMazeGenerationManager.maze[newXPosition, newYPosition];
            }
        }

        return null;
    }


    // returns true if the new position lies within the grid
    private bool AllowedToMoveToNextMazePart(int newXPosition, int newYPosition)
    {
        if (newXPosition >= 0 &&
            newYPosition >= 0 &&
            newXPosition < localMazeGenerationManager.maze.GetLength(0) &&
            newYPosition < localMazeGenerationManager.maze.GetLength(1))
        {
            return true;
        }
        return false;
    }

    private bool LeftHeldDown()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            return true;
        }
        return false;
    }

    private bool UpHeldDown()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            return true;
        }
        return false;
    }

    private bool RightHeldDown()
    {
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            return true;
        }
        return false;
    }

    private bool DownHeldDown()
    {
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            return true;
        }
        return false;
    }

    private bool EscapePressed()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            return true;
        }
        return false;
    }
}
