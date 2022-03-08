using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public bool tutorialEnabled;
    [Header("Components")]
    public AnimatorManager mainMenuAnimatorManager; //Animation manager

    bool isAnyKeyDown; //splash screen status

    public GameObject continueButton;

    private void Start()
    {
        if (JSInterface.Instance.isIOS)
        {
            Invoke("LoadMainMenuForIOS", 1f);
        }

        GenesisContractService.Instance.RequestLengths();
        if (GameResult.Instance.lastWonPlayer != null)
            continueButton.SetActive(true);
        else continueButton.SetActive(false);
    }

    private void LoadMainMenuForIOS()
    {
        ScenesManager.Instance.LoadLoadingScene("MainMenuIOS");
    }

    private void Update()
    {
        if (!isAnyKeyDown && Input.anyKeyDown) //if any key down
        {
            SplashScreenClose(); //Splash screen disable
        }
    }
    //Splash screen disable method
    void SplashScreenClose()
    {
        isAnyKeyDown = true;

        mainMenuAnimatorManager.PlayPlayableDirector(mainMenuAnimatorManager.timelineAssets[1], DirectorWrapMode.None); //Play main menu animation
        AudioManager.Instance.PlayMusic(0);
    }
    //New game method
    public void NewGame()
    {
        if (tutorialEnabled)
            ScenesManager.Instance.LoadLoadingScene("Tutorial");
        else ScenesManager.Instance.LoadLoadingScene("RD_0");

        AudioManager.Instance.StopMusic();
    }

    public void Continue()
    {
        ScenesManager.Instance.LoadLoadingScene("EndlessMode");
        AudioManager.Instance.StopMusic();
    }

    public void EnterCredits()
    {
        ScenesManager.Instance.LoadAdditiveScene("Credits");
    }
}