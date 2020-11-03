using System.Collections.Generic;
using UnityEngine;

public class MazePart : MonoBehaviour
{
    public int xPosition;
    public int yPosition;
    public bool visited = false;
    public int pathLength = 0;
    public bool hasFruit = false;

    // wallOpen array goes clockwise starting from the left (0 = left, 1 = top, 2 = right, 3 = bottom)
    public bool[] wallOpen = new bool[4];

    private bool underWater = false;
    public bool UnderWater
    {
        get { return underWater; }
        set
        {
            underWater = value;
            if (underWater)
            {
                SetWater();
            }
        }
    }

    private PlayerManager localPlayerManager;
    private MazeGenerationManager localMazeGenerationManager;
    private Animator animator;

    private void Awake()
    {
        localPlayerManager = GameInstance.Instance.playerManager;
        localMazeGenerationManager = GameInstance.Instance.mazeGenerationManager;
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    public List<MazePart> GetAllUnvistedNeighbours(MazePart[,] maze)
    {
        List<MazePart> unvistedNeighbours = new List<MazePart>();

        AddMazePartIfWithinGridAndNotVisited(xPosition + 1, yPosition, maze, unvistedNeighbours);
        AddMazePartIfWithinGridAndNotVisited(xPosition - 1, yPosition, maze, unvistedNeighbours);
        AddMazePartIfWithinGridAndNotVisited(xPosition, yPosition + 1, maze, unvistedNeighbours);
        AddMazePartIfWithinGridAndNotVisited(xPosition, yPosition - 1, maze, unvistedNeighbours);

        return unvistedNeighbours;
    }

    private void AddMazePartIfWithinGridAndNotVisited(int x, int y, MazePart[,] maze, List<MazePart> unvistedNeighbours)
    {
        if (x >= 0 && x < maze.GetLength(0) &&
            y >= 0 && y < maze.GetLength(1))
        {
            if (!maze[x, y].visited)
            {
                unvistedNeighbours.Add(maze[x, y]);
            }
        }
    }

    public void RemoveWall(MazePart neighbour)
    {
        bool xIsSame = xPosition == neighbour.xPosition;

        // wallOpen array goes clockwise starting from the left (0 = left, 1 = top, 2 = right, 3 = bottom)
        // remove bottom wall
        if (xIsSame && yPosition - 1 == neighbour.yPosition)
        {
            wallOpen[(int)Direction.Down] = true;
            neighbour.wallOpen[(int)Direction.Up] = true;
        }
        // remove top wall
        else if (xIsSame)
        {
            wallOpen[(int)Direction.Up] = true;
            neighbour.wallOpen[(int)Direction.Down] = true;
        }
        // remove left wall
        else if (xPosition - 1 == neighbour.xPosition)
        {
            wallOpen[(int)Direction.Left] = true;
            neighbour.wallOpen[(int)Direction.Right] = true;
        }
        // remove right wall
        else
        {
            wallOpen[(int)Direction.Right] = true;
            neighbour.wallOpen[(int)Direction.Left] = true;
        }
    }

    public List<MazePart> GetAllDryNeighbours(MazePart[,] maze)
    {
        List<MazePart> dryNeighbours = new List<MazePart>();

        AddMazePartIfWithinGridAndDry(xPosition + 1, yPosition, maze, dryNeighbours, Direction.Right);
        AddMazePartIfWithinGridAndDry(xPosition - 1, yPosition, maze, dryNeighbours, Direction.Left);
        AddMazePartIfWithinGridAndDry(xPosition, yPosition + 1, maze, dryNeighbours, Direction.Up);
        AddMazePartIfWithinGridAndDry(xPosition, yPosition - 1, maze, dryNeighbours, Direction.Down);

        return dryNeighbours;
    }

    private void AddMazePartIfWithinGridAndDry(int x, int y, MazePart[,] maze, List<MazePart> dryNeighbours, Direction direction)
    {
        if (x >= 0 && x < maze.GetLength(0) &&
            y >= 0 && y < maze.GetLength(1))
        {
            if (!maze[x, y].underWater && wallOpen[(int)direction])
            {
                dryNeighbours.Add(maze[x, y]);
            }
        }
    }

    // the MazePart shows a random tile sprite until it is flooded, 
    // at that point the animator is enabled and an animation showing water moving is started,
    // the animation has no loop time so the water comes to a standstil after the animation ends
    private void SetWater()
    {
        localPlayerManager.CheckForPlayerWinOrLoss(this);
        
        animator.enabled = true; 
    }
}
