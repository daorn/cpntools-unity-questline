using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameMenuScript : MonoBehaviour
{
    private bool isPaused;
    [SerializeField] private GameObject pauseMenu;

    private void Awake()
    {
        pauseMenu.SetActive(false);
    }

    private void OnPause(InputValue value)
    {
        if (isPaused)
        {
            Debug.Log("Pause Menu closed");
            Resume();
        }
        else
        {
            Debug.Log("Pause Menu opened");
            PauseMenu();
        }
    }

    public void CursorOn()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CursorOff()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public void PauseMenu()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        CursorOn();
    }

    public void Resume()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        CursorOff();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMainMenu()
    {
        CursorOn();
        Time.timeScale = 0;
        SceneManager.LoadSceneAsync(0);
    }
}
