using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerationManager : MonoBehaviour
{
#pragma warning disable 649, IDE0044
    [SerializeField]
    private MazePart mazePartPrefab;
    [SerializeField]
    private WallPart wallPartPrefab;
    [SerializeField]
    private Sprite[] kitchenSprites;
    [SerializeField]
    private SpriteRenderer fruitPrefab;
#pragma warning restore 649, IDE0044

    public Sprite waterSprite;
    public MazePart[,] maze;

    private List<WallPart> wallParts = new List<WallPart>();
    private MazeCamera localMazeCamera;
    private System.Random random = new System.Random();
    private MazePart fruitMazePart;
    private SpriteRenderer fruit;

    private void Start()
    {
        localMazeCamera = GameInstance.Instance.mazeCamera;
    }

    // will generate a maze with the specified width, height, and chosen algorithm
    // after the maze is generated a piece of fruit is placed at the end of the longest path
    // finally the walls are placed
    public void GenerateMaze(int width, int height, Algorithm algorithm)
    {
        if (maze != null)
        {
            DestroyPreviousMaze();
        }

        CreateMaze(width, height);

        fruitMazePart = maze[0, 0];

        switch (algorithm)
        {
            case Algorithm.Iterative:
                DepthFirstIterativeImplementation();
                break;
            case Algorithm.Recursive:
                DepthFirstRecursiveImplementation(maze[0, 0], 1);
                break;
        }

        fruitMazePart.hasFruit = true;
        fruit = Instantiate(fruitPrefab, fruitMazePart.transform.position, new Quaternion(0, 0, 0, 0));

        CreateWalls();
    }

    // instantiates mazeParts in a grid formation and saves them in the maze array (size = MazePart[width, height])
    private void CreateMaze(int width, int height)
    {
        maze = new MazePart[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                mazePartPrefab.xPosition = x;
                mazePartPrefab.yPosition = y;

                float worldSpaceX = x;
                float worldSpaceY = y;

                mazePartPrefab.GetComponent<SpriteRenderer>().sprite = kitchenSprites[random.Next(0, kitchenSprites.Length)];

                maze[x, y] = Instantiate(mazePartPrefab, new Vector2(worldSpaceX, worldSpaceY), new Quaternion(0, 0, 0, 0));
            }
        }
    }

    // iterative implementation of randomised depth first algorithm
    private void DepthFirstIterativeImplementation()
    {
        Stack mazeStack = new Stack();

        // mark as visited, push to stack
        maze[0, 0].visited = true;
        mazeStack.Push(maze[0, 0]);

        while (mazeStack.Count > 0)
        {
            MazePart currentMazePart = (MazePart)mazeStack.Pop();

            List<MazePart> currentMazePartUnvistedNeighbours = currentMazePart.GetAllUnvistedNeighbours(maze);
            if (currentMazePartUnvistedNeighbours.Count > 0)
            {
                // push current cell to stack
                mazeStack.Push(currentMazePart);

                // choose unvisited neighbour
                MazePart chosenNeighbour = currentMazePartUnvistedNeighbours[random.Next(0, currentMazePartUnvistedNeighbours.Count)];

                // remove wall
                chosenNeighbour.RemoveWall(currentMazePart);

                // mark chosenNeighbour visited
                chosenNeighbour.visited = true;
                mazeStack.Push(chosenNeighbour);

                // keep track of pathLength to put fruit at the end of the longest path
                chosenNeighbour.pathLength = currentMazePart.pathLength + 1;
            }
            // the longest path in the maze gets a piece of fruit as the end goal for the player
            else
            {
                if (currentMazePart.pathLength > fruitMazePart.pathLength)
                {
                    fruitMazePart = currentMazePart;
                }
            }
        }
    }

    // recursive implementation of randomised depth first algorithm
    private void DepthFirstRecursiveImplementation(MazePart currentMazePart, int pathLength)
    {
        // added a pathLength tracker to put fruit at the end of the longest path after the maze is finished
        currentMazePart.pathLength = pathLength;
        if (currentMazePart.pathLength > fruitMazePart.pathLength)
        {
            fruitMazePart = currentMazePart;
        }

        // mark as visited
        currentMazePart.visited = true;

        // read comment in the else for why "true" is put as a condition
        while (true)
        {
            // get a list of all unvisitedNeighbours of this MazePart
            List<MazePart> currentMazePartUnvistedNeighbours = currentMazePart.GetAllUnvistedNeighbours(maze);

            if (currentMazePartUnvistedNeighbours.Count > 0)
            {
                // choose random unvisted neighbour, then remove it from the list
                MazePart chosenNeighbour = currentMazePartUnvistedNeighbours[random.Next(0, currentMazePartUnvistedNeighbours.Count)];
                currentMazePartUnvistedNeighbours.Remove(chosenNeighbour);

                // remove wall between chosenNeighbour and currentMazePart
                currentMazePart.RemoveWall(chosenNeighbour);

                // call this function with chosenNeighbour 
                DepthFirstRecursiveImplementation(chosenNeighbour, pathLength + 1);
            }
            // when there are no more unvisted neighbours for currentMazePart,
            // break out of the while loop.
            else
            {
                // I understand this isn't the nicest way to do it but it works well, 
                // and currentMazePartUnvistedNeighbours.Count > 0 should not be put as a condition,
                // because there could be (will be) MazeParts in that list that have been changed (visited) by a deeper call of this function.
                break;
            }
        }
    }

    private void DestroyPreviousMaze()
    {
        for (int i = 0; i < wallParts.Count; i++)
        {
            Destroy(wallParts[i].gameObject);
        }
        wallParts.Clear();

        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                Destroy(maze[x, y].gameObject);
            }
        }

        Destroy(GameInstance.Instance.playerManager.currentPlayerCharacter.gameObject);
        Destroy(fruit.gameObject);
    }

    // goes through all the MazeParts + another horizontal and vertical row 
    // creates a wall if the wall is not open for these mazeparts
    private void CreateWalls()
    {
        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                MazePart mazePart = maze[x, y];

                // wallOpen array goes clockwise starting from the left (0 = left, 1 = top, 2 = right, 3 = bottom)

                if (!mazePart.wallOpen[(int)Direction.Left])
                {
                    CreateWall(Direction.Left, x, y);
                }
                if (!mazePart.wallOpen[(int)Direction.Up])
                {
                    CreateWall(Direction.Up, x, y);
                }

                // for the last vertical row
                if (x == maze.GetLength(0) - 1
                    && !mazePart.wallOpen[(int)Direction.Right])
                {
                    CreateWall(Direction.Right, x, y);
                }
                // for the last horizontal row
                if (y == 0
                    && !mazePart.wallOpen[(int)Direction.Down])
                {
                    CreateWall(Direction.Down, x, y);
                }
            }
        }
    }

    private void CreateWall(Direction side, int xPosition, int yPosition)
    {
        // sortingOrder differences are to prevent any potential flickering issues when two sprites overlap on the same layer
        switch (side)
        {
            case Direction.Left:
                wallPartPrefab.GetComponent<SpriteRenderer>().sortingOrder = 1;
                wallParts.Add(Instantiate(wallPartPrefab, new Vector2(xPosition - 0.5f, yPosition), Quaternion.Euler(new Vector3(0, 0, 90))));
                break;
            case Direction.Up:
                wallPartPrefab.GetComponent<SpriteRenderer>().sortingOrder = 2;
                wallParts.Add(Instantiate(wallPartPrefab, new Vector2(xPosition, yPosition + 0.5f), Quaternion.Euler(new Vector3(0, 0, 0))));
                break;
            case Direction.Right:
                wallPartPrefab.GetComponent<SpriteRenderer>().sortingOrder = 3;
                wallParts.Add(Instantiate(wallPartPrefab, new Vector2(xPosition + 0.5f, yPosition), Quaternion.Euler(new Vector3(0, 0, 90))));
                break;
            case Direction.Down:
                wallPartPrefab.GetComponent<SpriteRenderer>().sortingOrder = 4;
                wallParts.Add(Instantiate(wallPartPrefab, new Vector2(xPosition, yPosition - 0.5f), Quaternion.Euler(new Vector3(0, 0, 0))));
                break;
        }
    }
}