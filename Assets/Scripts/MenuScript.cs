using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    private bool isPaused;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject questLog;

    private void Awake()
    {
        pauseMenu.SetActive(false);
        questLog.SetActive(false);
    }

    private void OnJournal(InputValue value)
    {
        questLog.SetActive(!questLog.activeSelf);
    }

    private void OnPause(InputValue value)
    {
        if (isPaused)
            Resume();
        else
            PauseMenu();
    }
    
    public void PauseMenu()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Resume()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Time.timeScale = 1;
        Application.Quit();
    }
}
