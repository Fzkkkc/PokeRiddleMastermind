using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
{
    public string SceneName;
    public string MenuName;
    public AudioSource Music;
    [SerializeField] private Animator _gameTransAnimator;
    [SerializeField] private UnoGameManager _unoGameManager;
    
    private void Awake()
    {
        var temp = GameObject.FindWithTag("MUSIC");
        if (temp != null)
            Music = temp.GetComponent<AudioSource>();
    }

    public void OnGameClick(int idx)
    {
        SceneManager.LoadScene(SceneName);
    }

    public void OnGoToMenuClick()
    {
        if (_unoGameManager.IsWin)
        {
            StartCoroutine(ExitToMenu());
        }
        else
        {
            StartCoroutine(Restart());
        }
    }

    public void OnExitClicked()
    {
        DebugControl.Log("quit called", 1);
        Application.Quit();
    }

    public void PlayMusic(bool play)
    {
        if (play) Music.Play();
        else Music.Stop();
    }

    private IEnumerator ExitToMenu()
    {
        _gameTransAnimator.SetTrigger("GameTransOut");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(0);
    }
    
    private IEnumerator Restart()
    {
        _gameTransAnimator.SetTrigger("GameTransOut");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(1);
    }
}