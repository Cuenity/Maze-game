using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreateMazeCanvas : MonoBehaviour
{
    private TMP_InputField inputFieldWidth;
    private TMP_InputField inputFieldHeight;
    private TMP_Dropdown dropdownDifficulty;
    private TMP_Dropdown dropdownAlgorithm;
    private Color inputFieldDefaultColor;
    private TextMeshProUGUI errorText;

    private void Start()
    {
        inputFieldWidth = transform.Find("CreateMazePanel/InputFieldWidth").GetComponent<TMP_InputField>();
        inputFieldHeight = transform.Find("CreateMazePanel/InputFieldHeight").GetComponent<TMP_InputField>();
        dropdownDifficulty = transform.Find("CreateMazePanel/DropdownDifficulty").GetComponent<TMP_Dropdown>();
        dropdownAlgorithm = transform.Find("CreateMazePanel/DropdownAlgorithm").GetComponent<TMP_Dropdown>();
        errorText = transform.Find("CreateMazePanel/ErrorText").GetComponent<TextMeshProUGUI>();

        inputFieldDefaultColor = inputFieldWidth.image.color;

        dropdownDifficulty.options.Clear();
        // gets a string array of all enum values in Difficulty, 
        // then creates a list using the array and adds the list to the dropdown options
        dropdownDifficulty.AddOptions(new List<string>(Enum.GetNames(typeof(Difficulty))));

        dropdownAlgorithm.options.Clear();
        dropdownAlgorithm.AddOptions(new List<string>(Enum.GetNames(typeof(Algorithm))));
    }

    // triggered by "Generate maze" button
    public void CreateMazePressed()
    {
        bool inputValid = true;
        errorText.gameObject.SetActive(false);

        if (int.TryParse(inputFieldWidth.text, out int width))
        {
            if (width > 1 && width <= 100)
            {
                inputFieldWidth.image.color = inputFieldDefaultColor;
            }
            else
            {
                inputValid = false;
                inputFieldWidth.image.color = Color.red;
                errorText.gameObject.SetActive(true);
            }
        }
        else
        {
            inputValid = false;
            inputFieldWidth.image.color = Color.red;
        }

        if (int.TryParse(inputFieldHeight.text, out int height))
        {
            if (height > 1 && height <= 100)
            {
                inputFieldHeight.image.color = inputFieldDefaultColor;
            }
            else
            {
                inputValid = false;
                inputFieldHeight.image.color = Color.red;
                errorText.gameObject.SetActive(true);
            }
        }
        else
        {
            inputValid = false;
            inputFieldHeight.image.color = Color.red;
        }

        // if all input if valid, 
        // generate the maze with the chosen width, height, and algoritm
        // start the water after some time depending on difficulty (only after the player has started moving)
        // spawn the player
        // set camera to correct size and to middle of maze
        // set this canvas inactive
        if (inputValid)
        {
            PlayerManager playerManager = GameInstance.Instance.playerManager;
            playerManager.StopWaterSpreadingCoroutines();

            MazeGenerationManager mazeGenerationManager = GameInstance.Instance.mazeGenerationManager;
            mazeGenerationManager.GenerateMaze(width, height, (Algorithm)dropdownAlgorithm.value);

            GameInstance.Instance.mazeCamera.SetCameraToMiddleOfMaze();
            GameInstance.Instance.uiManager.CreateNewMazePressed();

            playerManager.chosenDifficulty = (Difficulty)dropdownDifficulty.value;
            playerManager.SpawnPlayer(mazeGenerationManager.maze[0, 0]);
        }
    }

    // triggered by "Create new maze" button
    public void NewMazeButtonPressed()
    {
        GameInstance.Instance.uiManager.CreateNewMazePressed();
    }
}
