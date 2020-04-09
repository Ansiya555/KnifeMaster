using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using Strobotnik.GUA;

public class AdScript : MonoBehaviour
{
    public static AdScript AdInstance;
    SimpleJSON.JSONNode JsonValues;
    bool analyticsSetupDone = false;

    GameObject analyticsGameobject;
    // Start is called before the first frame update.
    private void Awake()
    {
        if (AdInstance == null)
        {
            AdInstance = this;
        }

    }
  
   void Start()
    {

        analyticsSetupDone = false;

        if (analyticsGameobject == null)
        {
         //   if (isGooglePlayStoreVersion)
          //  {
                if (analyticsGameobject == null)
                {
                    analyticsGameobject = new GameObject("Analytics");
                    analyticsGameobject.SetActive(false);
                    analyticsGameobject.AddComponent<Analytics>().enabled = false;
                }
                if (JsonValues != null)
                {
                    string trackingId = "";

                    if (JsonValues["mainData"]["analyticsTrackingID"])
                    {
                        trackingId = JsonValues["mainData"]["analyticsTrackingID"];
                    }

                string gameTitle = JsonValues["mainData"]["gameTitle"];
                string appName = gameTitle;

                    if (JsonValues["mainData"]["analyticsAppName"])
                    {
                        appName = JsonValues["mainData"]["analyticsAppName"];
                    }

                    if (analyticsGameobject.GetComponent<Analytics>())
                    {
                        analyticsGameobject.GetComponent<Analytics>().trackingID = trackingId;
                        analyticsGameobject.GetComponent<Analytics>().appName = appName;
                        analyticsGameobject.GetComponent<Analytics>().enabled = true;
                        analyticsGameobject.SetActive(true);
                        analyticsSetupDone = true;
                    }
                }
            }
        }
    
    public void InitializeMobileAds(SimpleJSON.JSONNode mainJsonData)
    {
        JsonValues = mainJsonData;
        string appId = "ca-app-pub-3940256099942544~3347511713";
        if (mainJsonData != null)
        {
            if (mainJsonData["mainData"]["admobAppId"])
            {
                appId = mainJsonData["mainData"]["admobAppId"];
               
            }
        }
        MobileAds.Initialize(appId);

        this.rewardBasedVideo = RewardBasedVideoAd.Instance;
        rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
        RequestRewardBasedVideo();

    }
    RewardBasedVideoAd rewardBasedVideo;

    int deathAdInterval = 3;
    int numOfDeaths = 0;

    private void RequestRewardBasedVideo()
    {
        string adUnitId = "ca-app-pub-3940256099942544/5224354917";
        if (JsonValues != null)
        {
            if (JsonValues["mainData"]["admobAdId"])
            {
                adUnitId = JsonValues["mainData"]["admobAdId"];
            }
            if (JsonValues["mainData"]["deathAdInterval"])
            {
                deathAdInterval = JsonValues["mainData"]["deathAdInterval"];
            }
        }
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded video ad with the request.
        rewardBasedVideo.LoadAd(request, adUnitId);
    }

    public void HandleRewardBasedVideoClosed(object sender, System.EventArgs args)
    {
        RequestRewardBasedVideo();
    }

    public void ShowVideoAd()
    {
        if (rewardBasedVideo.IsLoaded())
        {

            rewardBasedVideo.Show();
        }
    }
}
