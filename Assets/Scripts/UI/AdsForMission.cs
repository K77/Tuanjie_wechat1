using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif

public class AdsForMission : MonoBehaviour
{
#if UNITY_ADS && UNITY_ANALYTICS
    static int shown = 0;
#endif

    public MissionUI missionUI;
    public Button addButton;

    void Update()
    {
#if UNITY_ADS
        addButton.gameObject.SetActive(Advertisement.IsReady("rewardedVideo"));
#else
        addButton.gameObject.SetActive(false);
#endif
    }

    public void ShowAds()
    {
#if UNITY_ADS
        if (Advertisement.IsReady("rewardedVideo"))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
#endif
    }

#if UNITY_ADS

    private void HandleShowResult(ShowResult result)
    {

#if UNITY_ANALYTICS
        shown += 1;
        Analytics.CustomEvent("ad_play", new Dictionary<string, object>
            {
                { "distance", TrackManager.instance == null ? 0 : TrackManager.instance.worldDistance },
                { "skipped", (result == ShowResult.Skipped)  },
                { "shown", shown }
            });
#endif

        switch (result)
        {
            case ShowResult.Finished:
                AddNewMission();
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }
#endif

    void AddNewMission()
    {
        PlayerData.instance.AddMission();
        PlayerData.instance.Save();
        missionUI.Open();
    }
}
