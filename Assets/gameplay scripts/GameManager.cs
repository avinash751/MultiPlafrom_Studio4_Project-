
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static Action GameIsStarting;
    public static Action<bool> GameIsInPlayMode;
    public static Action<bool> GameIsOver;
    protected static bool runOnce = false;


    [SerializeField] bool skipStartMenu;

    [Header("Game Menus")]
    [SerializeField] protected GameObject startMenu;
    [SerializeField] protected GameObject pauseMenu;
    [SerializeField] protected GameObject gameOverMenu;
    [SerializeField] protected GameObject winMenu;

    [Header("In Game UI")]
    [SerializeField] protected GameObject inGameUI;
    [SerializeField] bool allowToDisableGameUIThroughInput;

    [Header("Debug Info")]
    [SerializeField] bool paused = false;



    public enum GameState
    {
        Start,
        Playing,
        Paused,
        GameOver,
        Win,
    }

    public static GameState StartState = GameState.Start;
    public static GameState PlayingState = GameState.Playing;
    public static GameState PausedState = GameState.Paused;
    public static GameState GameOverState = GameState.GameOver;
    public static GameState WinState = GameState.Win;

    public GameState CurrentGameState = GameState.Start;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

    }

    protected virtual void Start()
    {

        DisableAllUI();
        StartGame();
    }

    protected virtual void Update()
    {
        PlayGameState();
        TransitionToGameOverState();
        GameOver();
        TransitionToPauseState();
        PauseGameOrContinueGame();
        OnInputDisableGameUI(allowToDisableGameUIThroughInput);

    }
    protected virtual bool StartGame()
    {
        GameIsStarting?.Invoke();

        if (skipStartMenu)
        {
            TransitionToPlayState();
            return false;
        }

        EnableOrDisableUI(ref startMenu, true);
        return true;
    }


    public virtual void TransitionToPlayState()
    {
        UpdateCurrentGameState(PlayingState);
    }

    protected virtual bool PlayGameState()
    {
        if (runOnce) return false;

        if (IsCurrentState(PlayingState))
        {
            GameIsInPlayMode?.Invoke(true);
            EnableOrDisableUI(ref inGameUI, true);
            EnableOrDisableUI(ref startMenu, false);
            Time.timeScale = 1;
            runOnce = true;
            return true;
        }
        return false;
    }

    protected virtual void TransitionToPauseState()
    {
        
        if (!IsCurrentState(PlayingState) && pauseMenu ==null) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UpdateCurrentGameState(PausedState);
        }
    }

    protected virtual void PauseGameOrContinueGame()
    {
        // later on this needs to have a customised condtion because mouse hover is not disabled
        if (IsCurrentState(PausedState) && !runOnce)
        {
            EnableOrDisableUI(ref pauseMenu, true);
            Time.timeScale = 0;
            runOnce = true;
            return;
        }
        ResumeGameThroughInput();
    }

    public virtual void ResumeGame()
    {
        TransitionToPlayState();
        EnableOrDisableUI(ref pauseMenu, false);
        Time.timeScale = 1;
    }

    void ResumeGameThroughInput()
    {
        if (!IsCurrentState(PausedState) && !runOnce) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResumeGame();
            Debug.Log("Pause");
        }
    }

    protected virtual bool TransitionToGameOverState()
    {
        if (IsCurrentState(GameOverState)) return false;

        if (IsCurrentState(PlayingState))
        {
            // you have to inherit from this script and set a custom condtion to run game over function 
            return true;
        }
        return false;

    }

    protected virtual bool GameOver()
    {
        if (runOnce) return false;

        if (IsCurrentState(GameOverState))
        {
            GameIsOver?.Invoke(true);
            EnableOrDisableUI(ref inGameUI, false);
            EnableOrDisableUI(ref gameOverMenu, true);
            Debug.Log("Game Over");
            return true;
        }
        return false;
    }

    #region Game Manager Helper Functions
    public static bool IsCurrentState(GameState _state)
    {
        if (Instance.CurrentGameState == _state)
        {
            return true;
        }
        return false;
    }

    public static void UpdateCurrentGameState(GameState _state)
    {
        Instance.CurrentGameState = _state;
        runOnce = false;
    }


    protected void EnableOrDisableUI(ref GameObject _menu, bool _enabled)
    {
        _menu.SetActive(_enabled);
    }

    public void DisableAllUI()
    {
        startMenu?.SetActive(false);
        pauseMenu?.SetActive(false);
        gameOverMenu?.SetActive(false);
        //winMenu?.SetActive(false);
        inGameUI?.SetActive(false);
    }


    public void RestartGame(bool _skipStartScreen)
    {    
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        skipStartMenu = _skipStartScreen;
        UpdateCurrentGameState(StartState);
        Start();

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void OnInputDisableGameUI(bool _disableGameUI)
    {
        if (Input.GetKeyDown(KeyCode.U) && allowToDisableGameUIThroughInput)
        {
            inGameUI.SetActive(!inGameUI.activeInHierarchy);
        }
    }
    #endregion


}
