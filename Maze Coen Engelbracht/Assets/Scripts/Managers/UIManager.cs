using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
#pragma warning disable 649
    [SerializeField]
    private CreateMazeCanvas createMazeCanvasPrefab;
    [SerializeField]
    private GameOverCanvas gameOverCanvasPrefab;
#pragma warning restore 649

    private CreateMazeCanvas createMazeCanvas;
    private Transform createMazePanel;
    private Transform createNewMazeButtonPanel;
    private GameOverCanvas gameOverCanvas;

    private void Start()
    {
        Camera mazeCamera = GameInstance.Instance.mazeCamera.GetComponent<Camera>();
        createMazeCanvasPrefab.GetComponent<Canvas>().worldCamera = mazeCamera;
        createMazeCanvasPrefab.GetComponent<Canvas>().sortingLayerName = "UI";
        createMazeCanvas = Instantiate(createMazeCanvasPrefab);
        createMazePanel = createMazeCanvas.transform.Find("CreateMazePanel");
        
        gameOverCanvasPrefab.gameObject.SetActive(false);
        gameOverCanvasPrefab.GetComponent<Canvas>().worldCamera = mazeCamera;
        gameOverCanvasPrefab.GetComponent<Canvas>().sortingLayerName = "UI";
        gameOverCanvas = Instantiate(gameOverCanvasPrefab);

        createNewMazeButtonPanel = createMazeCanvas.transform.Find("NewMazeButtonPanel");
        createNewMazeButtonPanel.gameObject.SetActive(false);
    }

    // if the create new maze button panel is currently active, 
    // activate the create maze panel and deactivate the create new maze button panel
    // and the other way round
    public void CreateNewMazePressed()
    {
        bool setCreateMazePanelActive = createNewMazeButtonPanel.gameObject.activeSelf;

        createMazePanel.gameObject.SetActive(setCreateMazePanelActive);
        createNewMazeButtonPanel.gameObject.SetActive(!setCreateMazePanelActive);
    }

    public void GameOver(bool playerWon)
    {
        createMazePanel.gameObject.SetActive(false);
        createNewMazeButtonPanel.gameObject.SetActive(false);
        gameOverCanvas.gameObject.SetActive(true);
        gameOverCanvas.GameOver(playerWon);
    }

    public void SetCreateMazePanelActive()
    {
        createMazePanel.gameObject.SetActive(true);
    }
}
