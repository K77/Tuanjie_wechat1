using UnityEngine;
using UnityEngine.UI;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif
using System.Collections.Generic;
 
/// <summary>
/// state pushed on top of the GameManager when the player dies.
/// </summary>
public class GameOverState : AState
{
    public TrackManager trackManager;
    public Canvas canvas;
    public MissionUI missionPopup;

	public AudioClip gameOverTheme;

	public Button adsButton;

	public Leaderboard miniLeaderboard;
	public Leaderboard fullLeaderboard;

    public GameObject addButton;

	protected bool m_CoinCredited = false;

    public override void Enter(AState from)
    {
        canvas.gameObject.SetActive(true);

		miniLeaderboard.playerEntry.inputName.text = PlayerData.instance.previousName;
		
		miniLeaderboard.playerEntry.score.text = trackManager.score.ToString();
		miniLeaderboard.Populate();

        if (PlayerData.instance.AnyMissionComplete())
            missionPopup.Open();
        else
            missionPopup.gameObject.SetActive(false);

		adsButton.gameObject.SetActive(false);

		m_CoinCredited = false;

		if (trackManager.isRerun)
			CreditCoins();

		if (MusicPlayer.instance.GetStem(0) != gameOverTheme)
		{
            MusicPlayer.instance.SetStem(0, gameOverTheme);
			StartCoroutine(MusicPlayer.instance.RestartAllStems());
        }

#if UNITY_ADS
		if (!trackManager.isRerun && Advertisement.IsReady("rewardedVideo"))
		{
			adsButton.GetComponentInChildren<Text>().text = "AD for 1-up";
			addButton.SetActive(true);
#if UNITY_ANALYTICS
            Analytics.CustomEvent("ad_offer");
#endif
        }
#endif
    }

	public override void Exit(AState to)
    {
        canvas.gameObject.SetActive(false);

        if (!trackManager.isRerun)
        {
            FinishRun();
        }
    }

    public override string GetName()
    {
        return "GameOver";
    }

    public override void Tick()
    {
        
    }

	public void OpenLeaderboard()
	{
		fullLeaderboard.forcePlayerDisplay = false;
		fullLeaderboard.displayPlayer = true;
		fullLeaderboard.playerEntry.playerName.text = miniLeaderboard.playerEntry.inputName.text;
		fullLeaderboard.playerEntry.score.text = trackManager.score.ToString();

		fullLeaderboard.Open();
    }

	public void GoToStore()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("shop", UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }


    public void GoToLoadout()
    {
        trackManager.isRerun = false;
		manager.SwitchState("Loadout");
    }

    public void RunAgain()
    {
        trackManager.isRerun = false; 
		manager.SwitchState("Game");
    }

    //return to currently lost game (i.e. when watched an ad for second wind)
    public void BackToRun()
    {
        trackManager.isRerun = true;
        trackManager.characterController.currentLife += 1;
        manager.SwitchState("Game");
    }

	protected void CreditCoins()
	{
		if (m_CoinCredited)
			return;

		// -- give coins gathered
		PlayerData.instance.coins += trackManager.characterController.coins;
		PlayerData.instance.premium += trackManager.characterController.premium;

		PlayerData.instance.Save();

#if UNITY_ANALYTICS
        if (trackManager.characterController.coins > 0)
        {
            Analytics.CustomEvent("currency_acquired", new Dictionary<string, object>
            {
                {"item_name", "fishbone"},
                {"amount",  trackManager.characterController.coins},
                {"source", "earned" },
                {"new_balance",   PlayerData.instance.coins},
                {"type", "soft"}
            });
        }

        if (trackManager.characterController.premium > 0)
        {
            Analytics.CustomEvent("currency_acquired", new Dictionary<string, object>
            {
                {"item_name", "anchovies"},
                {"amount",  trackManager.characterController.premium},
                {"source", "earned" },
                {"new_balance",   PlayerData.instance.premium},
                {"type", "hard"}
            });
        }
#endif 

        m_CoinCredited = true;
	}

	protected void FinishRun()
    {

		CreditCoins();

		if(miniLeaderboard.playerEntry.inputName.text == "")
		{
			miniLeaderboard.playerEntry.inputName.text = "Trash Cat";
		}
		else
		{
			PlayerData.instance.previousName = miniLeaderboard.playerEntry.inputName.text;
		}

        PlayerData.instance.InsertScore(trackManager.score, miniLeaderboard.playerEntry.inputName.text );

        CharacterCollider.DeathEvent de = trackManager.characterController.characterCollider.deathData;
        //register data to analytics
#if UNITY_ANALYTICS
        Analytics.CustomEvent("game_over", new Dictionary<string, object>
            {
                { "coins", de.coins },
				{ "premium", de.premium },
                { "obstacle_type",  de.obstacleType},
                { "character_used", de.character},
                { "theme_used", de.themeUsed},
                { "score", de.score},
                { "world_distance", de.worldDistance }
            });
#endif

        PlayerData.instance.Save();

        trackManager.End();
    }

    //----------------


    public void ShowRewardedAd()
    {
#if UNITY_ADS
        if (Advertisement.IsReady("rewardedVideo"))
        {
#if UNITY_ANALYTICS
            Analytics.CustomEvent("ad_start");
#endif

            adsButton.GetComponentInChildren<Text>().text = "Loading...";
			var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
#else
		BackToRun();
#endif
    }

	//=== AD
#if UNITY_ADS

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
#if UNITY_ANALYTICS
                Analytics.CustomEvent("ad_complete");
#endif
                BackToRun();
                break;
            case ShowResult.Skipped:
#if UNITY_ANALYTICS
                Analytics.CustomEvent("ad_skip");
#endif
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }
#endif
}
