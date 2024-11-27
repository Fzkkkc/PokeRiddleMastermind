using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
{
    public string SceneName;
    public string MenuName;
    public AudioSource Music;
    [SerializeField] private Animator _gameTransAnimator;
    
    private void Awake()
    {
        GameObject temp = GameObject.FindWithTag("MUSIC");
        if (temp != null)
            Music = temp.GetComponent<AudioSource>();
    }
    public void OnGameClick(int idx)
    {
        SceneManager.LoadScene(SceneName);
    }
    public void OnGoToMenuClick()
    {
        StartCoroutine(ExitCor());
    }
    public void OnExitClicked()
    {
        //TODO: test on build
        DebugControl.Log("quit called", 1);
        Application.Quit();
    }
    public void PlayMusic(bool play)
    {
        if (play) Music.Play();
        else Music.Stop();
    }

    private IEnumerator ExitCor()
    {
        _gameTransAnimator.SetTrigger("GameTransOut");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(0);
    }
}
