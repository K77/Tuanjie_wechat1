using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif
#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif

public class StartButton : MonoBehaviour
{
    public void StartGame()
    {
        if (PlayerData.instance.ftueLevel == 0)
        {
            PlayerData.instance.ftueLevel = 1;
            PlayerData.instance.Save();
#if UNITY_ANALYTICS
            Analytics.CustomEvent("post_install_action");
#endif
        }

#if UNITY_PURCHASING
        var module = StandardPurchasingModule.Instance();
        module.useFakeStoreAlways = true;
#endif
        SceneManager.LoadScene("main");
    }
}
