using UnityEngine;

public class GameInstance : MonoBehaviour
{
    private static GameInstance instance;
    public static GameInstance Instance { get { return instance; } }

    public MazeCamera mazeCamera;
    public MazeGenerationManager mazeGenerationManager;
    public UIManager uiManager;
    public InputManager inputManager;
    public PlayerManager playerManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        mazeCamera = Instantiate(mazeCamera);
        mazeCamera.transform.SetParent(transform);

        mazeGenerationManager = Instantiate(mazeGenerationManager);
        mazeGenerationManager.transform.SetParent(transform);

        uiManager = Instantiate(uiManager);
        uiManager.transform.SetParent(transform);

        inputManager = Instantiate(inputManager);
        inputManager.transform.SetParent(transform);

        playerManager = Instantiate(playerManager);
        playerManager.transform.SetParent(transform);
    }
}
