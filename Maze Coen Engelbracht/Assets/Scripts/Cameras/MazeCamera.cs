using UnityEngine;

public class MazeCamera : MonoBehaviour
{
    private const float CAMERAPADDING = 0.2f;

    private MazeGenerationManager localMazeGenerationManager;
    private Camera mazeCameraComponent;

    private void Start()
    {
        localMazeGenerationManager = GameInstance.Instance.mazeGenerationManager;
        mazeCameraComponent = GetComponent<Camera>();
    }

    // two corners of the maze to get the middle point, 
    // then set the camera at the middle point
    public void SetCameraToMiddleOfMaze()
    {
        AdjustCameraSize();

        MazePart[,] maze = localMazeGenerationManager.maze;

        int finalMazePartX = maze.GetLength(0) - 1;
        int finalMazePartY = maze.GetLength(1) - 1;
        Vector3 middleOfMaze = (maze[0, 0].transform.position
            + maze[finalMazePartX, finalMazePartY].transform.position)
            / 2;
        mazeCameraComponent.transform.position = new Vector3(middleOfMaze.x, middleOfMaze.y, mazeCameraComponent.transform.position.z);
    }

    // use the maze together with the resolution to calculate wich side is the largest (the side that needs the the camera to be at least a certain size)
    // set the camera size so that the largest side is exactly in view plus some padding
    private void AdjustCameraSize()
    {
        float unityUnitsMazeWidth = localMazeGenerationManager.maze.GetLength(0);
        float unityUnitsMazeHeight = localMazeGenerationManager.maze.GetLength(1);

        float minCameraSizeForWidth = unityUnitsMazeWidth * Screen.height / Screen.width * 0.5f;
        float minCameraSizeForHeight = unityUnitsMazeHeight * 0.5f;

        if (minCameraSizeForWidth > minCameraSizeForHeight)
        {
            mazeCameraComponent.orthographicSize = minCameraSizeForWidth + CAMERAPADDING;
        }
        else
        {
            mazeCameraComponent.orthographicSize = minCameraSizeForHeight + CAMERAPADDING;
        }
    }
}
