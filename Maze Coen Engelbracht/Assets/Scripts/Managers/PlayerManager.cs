using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
#pragma warning disable 649, IDE0044
    [SerializeField]
    private PlayerCharacter playerCharacterPrefab;
#pragma warning restore 649, IDE0044

    public PlayerCharacter currentPlayerCharacter;
    public bool gameStarted = false;
    public Difficulty chosenDifficulty;

    private UIManager localUIManager;
    private MazeGenerationManager localMazeGenerationManager;
    private List<MazePart> outermostUnderwaterMazeParts;

    private Coroutine currentStartWaterAfterSomeTimeCoroutine;
    private Coroutine currentSpreadWaterCoroutine;

    private void Start()
    {
        localUIManager = GameInstance.Instance.uiManager;
        localMazeGenerationManager = GameInstance.Instance.mazeGenerationManager;
    }

    public void SpawnPlayer(MazePart mazePart)
    {
        currentPlayerCharacter = Instantiate(playerCharacterPrefab, mazePart.transform.position, new Quaternion(0, 0, 0, 0));
        currentPlayerCharacter.currentMazePart = mazePart;
        gameStarted = true;

        // just in case the player generates a 1 x 1 maze
        CheckForPlayerWinOrLoss(mazePart);
    }

    public void StartWater()
    {
        currentStartWaterAfterSomeTimeCoroutine = StartCoroutine(StartWaterAfterSomeTime(chosenDifficulty));
    }

    // will set off the SpreadWater coroutine after a delay depending on selected difficulty
    private IEnumerator StartWaterAfterSomeTime(Difficulty difficulty)
    {
        outermostUnderwaterMazeParts = new List<MazePart>
        {
            localMazeGenerationManager.maze[0, 0]
        };

        switch (difficulty)
        {
            case Difficulty.Easy:
                yield return new WaitForSeconds(4f);
                outermostUnderwaterMazeParts[0].UnderWater = true;
                currentSpreadWaterCoroutine = StartCoroutine(SpreadWater(new WaitForSeconds(1f)));
                break;
            case Difficulty.Medium:
                yield return new WaitForSeconds(3f);
                outermostUnderwaterMazeParts[0].UnderWater = true;
                currentSpreadWaterCoroutine = StartCoroutine(SpreadWater(new WaitForSeconds(0.3f)));
                break;
            case Difficulty.Hard:
                yield return new WaitForSeconds(2f);
                outermostUnderwaterMazeParts[0].UnderWater = true;
                currentSpreadWaterCoroutine = StartCoroutine(SpreadWater(new WaitForSeconds(0.15f)));
                break;
        }
    }

    // will continuesly spread water to neighbours (without walls in-between) of MazeParts in the outermostUnderwaterMazeParts list
    // this happens at an interval determined in the StartWaterAfterSomeTime coroutine (dependend on difficulty)
    private IEnumerator SpreadWater(WaitForSeconds waitTime)
    {
        yield return waitTime;

        if (gameStarted)
        {
            List<MazePart> newUnderwaterMazeparts = new List<MazePart>();
            foreach (MazePart underwaterMazepart in outermostUnderwaterMazeParts)
            {
                List<MazePart> dryNeighbours = underwaterMazepart.GetAllDryNeighbours(localMazeGenerationManager.maze);
                foreach (MazePart dryNeighbour in dryNeighbours)
                {
                    dryNeighbour.UnderWater = true;
                    newUnderwaterMazeparts.Add(dryNeighbour);
                }
            }

            outermostUnderwaterMazeParts = new List<MazePart>(newUnderwaterMazeparts);

            currentSpreadWaterCoroutine = StartCoroutine(SpreadWater(waitTime));
        }
    }

    // will call GameOver function if the player character comes into contact with water (loss) or fruit (win)
    public void CheckForPlayerWinOrLoss(MazePart potentiallyUnderWaterMazePart)
    {
        if (potentiallyUnderWaterMazePart.hasFruit && potentiallyUnderWaterMazePart == currentPlayerCharacter.currentMazePart)
        {
            StopWaterSpreadingCoroutines();
            gameStarted = false;
            localUIManager.GameOver(true);
        }
        else if (potentiallyUnderWaterMazePart.UnderWater && potentiallyUnderWaterMazePart == currentPlayerCharacter.currentMazePart)
        {
            StopWaterSpreadingCoroutines();
            gameStarted = false;
            localUIManager.GameOver(false);
        }
    }

    public void StopWaterSpreadingCoroutines()
    {
        if (currentStartWaterAfterSomeTimeCoroutine != null)
        {
            StopCoroutine(currentStartWaterAfterSomeTimeCoroutine);
        }

        if (currentSpreadWaterCoroutine != null)
        {
            StopCoroutine(currentSpreadWaterCoroutine);
        }
    }
}