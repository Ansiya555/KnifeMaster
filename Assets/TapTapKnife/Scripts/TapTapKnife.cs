using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Strobotnik.GUA;
using GoogleMobileAds.Api;
using System.IO;

public class TapTapKnife : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] TextMeshProUGUI stageText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI plusFiftyText;
    [SerializeField] Vector2 plusFiftyFinalPos;
   

    [Header("Gameplay Variables")]
    [SerializeField] Sprite[] boardSkins;
    [SerializeField] Sprite[] bonusBoardSkins;
    [SerializeField] GameObject[] brokenBoards;

    [SerializeField] GameObject[] bonusBrokenBoards;
    [SerializeField] GameObject knifePrefab;


    [SerializeField] Vector3 knifeInitialPos;
    [SerializeField] Vector3 knifeThrownPos;
    public Vector3 particlePos;
    public Vector3 KnifehitparticlePos;

    [SerializeField] float knifeFadeLerpSpeed = 0.5f;
    [SerializeField] float knifeThrowSpeed = .15f;

    [Header("Boards based on Difficulty")]
    [SerializeField] GameObject[] easyBoards;
    [SerializeField] GameObject[] easyMediumBoards;
    [SerializeField] GameObject[] mediumBoards;
    [SerializeField] GameObject[] mediumHardBoards;
    [SerializeField] GameObject[] hardBoards;
    [SerializeField] GameObject[] easyHardBoards;
    [SerializeField] GameObject bonusBoard;
    List<GameObject> UIKnives = new List<GameObject>();


    [Header("UI Elements")]
    [SerializeField] Image[] StageDots;
    [SerializeField] Button playButton;
    [SerializeField] Button volumeButton;
    [SerializeField] Button restartButton;
    [SerializeField] Button homeButton;
    [SerializeField] Button infoButton;
    [SerializeField] Button tutoralButton;
    [SerializeField] GameObject homeScreenPanel;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] Image volumeImage;
    [SerializeField] GameObject redPanel;
    [SerializeField] GameObject tutorialPanel;
    public Canvas canvas;
    public GameObject plusFifty;
    [SerializeField] GameObject PauseScreenObject;
    [SerializeField] Button homeFromPauseButton;
    [SerializeField] Button resumeButton;


    [SerializeField] Sprite ActiveStageDot;
    [SerializeField] Sprite InActiveStageDots;
    [SerializeField] Sprite ActiveBonusDots;
    [SerializeField] Sprite InActiveBonusDots;
    [SerializeField] Sprite VolumeOFF;
    [SerializeField] Sprite VolumeOn;



    [Header("Values Based on Difficulty")]
    [SerializeField] Vector2[] numOfKnives;
    [SerializeField] float[] rotationSpeed;
    [SerializeField] Vector3[] forwardBackwardAngles; //x is Forward Angle // y is Backwards Angle // z is num of frames to complete one whole circle
    [SerializeField] float stopWaitTime;
    [SerializeField] AnimationCurve rotationType;

    [Header("Scoring System")]
    public int knifeHitPoints = 10;
    public int collectiblesPoints = 50;



    [Header("SFX and VFX")]
    public AudioSource collectibleSFX;
    [SerializeField] AudioSource knifeHitBoardSFX;
    [SerializeField] AudioSource knifeHitKnifeSFX;
    [SerializeField] AudioSource boardBreakSFX;
    [SerializeField] AudioSource buttonSFX;
    // AudioSource[] audioSources;



    public GameObject collectibleVFX;
    public GameObject knifeHitKnifeVfx;
    public GameObject boardHitVFX;
    public GameObject boardDestroyVFX;
    public GameObject knifeUIParent;
    public GameObject knifeUIObject;

    //Singleton
    public static TapTapKnife instance;

    //Static Variables
    public static string knifeString = "Knife";
    public static string collectibleString = "Collectible";
    public static string boardString = "Board";

    public static bool isGameOver = false;

    //private Variables

    GameObject currentBoard;
    GameObject knifeParent = null;

    List<GameObject> knifeObjects = new List<GameObject>();
    List<GameObject> knives = new List<GameObject>();//List for saving all the spawned knives till board breaks


    Stages currentStage;
    Difficulty currentDifficulty;

    bool isAttacking = false;
    bool canThrow = false;
    bool gameStarted = false;
    bool volume = true;

    SpriteRenderer currentBoardSprite;

    int currentLevel = 0;
    int score = 0;
    int stageCount;
    int tutorialCount = 0;
    int brokenboardIndex;
    int knivesLeft;


    float boardShakeDuration = .15f;
    float shakeMagnitude = 1f;


    public bool isGooglePlayStoreVersion;
    string devGameName;
    string gameTitle;
    string paytmGamesLink;
    public SimpleJSON.JSONNode mainJsonData;
    GameDownloader gameDownloader;
    public System.Reflection.Assembly assembly;
    public SimpleJSON.JSONNode gameValuesData;
    public AssetBundle mainAssetBundle = null;


    string mainJsonDataText;
    string scriptFilePath;
    string gameFolderName = "";

   bool analyticsSetupDone = false;

    GameObject analyticsGameobject;
    GameObject TapTapKnifePrefab;

    public TextAsset localJsonGameValuesText;
    string gameValuesVersionPrefs = "";
    string gameValuesFileName = "";
    List<AudioSource> audioSources = new List<AudioSource>();
    int currentSoundState;

    string soundStringPrefs;

    [SerializeField] System.Type Knife; 
    [SerializeField] System.Type BreakingForce; 
    [SerializeField] System.Type KnifeForce; 
   
    Vector3 camerPos=new Vector3(-.23f,0f,-10f);
    [SerializeField] SpriteRenderer BgRenderer;
    SimpleJSON.JSONNode gameValuesJsonData;
    [SerializeField] Material circleMat;
    [SerializeField] Material glowOneMat;
    [SerializeField] Material glowMat;
    [SerializeField] Material hitABMat;
    [SerializeField] Material kHitABMat;
    [SerializeField] Material rayABMat;
    [SerializeField] Material ringABMat;
    [SerializeField] Material triangleSoftOne;
    [SerializeField] Material woodABMat;
    [SerializeField] Material woodABMatOne;
    [SerializeField] Material woodABMatTwo;
    
    



    private void Awake()
    {
        instance = this;
        // audioSources = GetComponentsInChildren<AudioSource>();
        audioSources.Add(knifeHitBoardSFX);
        audioSources.Add(knifeHitKnifeSFX);
        audioSources.Add(boardBreakSFX);
        audioSources.Add(buttonSFX);
        audioSources.Add(collectibleSFX);
        //   audioSources.Add();
        //test
        
   
}


    IEnumerator Start()
    {
        SetScreenOrientation();
        isGooglePlayStoreVersion = false;
        devGameName = "TapTapKnife";

        gameTitle = "Tap Tap Knife";
        paytmGamesLink = "https://paytmfirstgames.com/";

        GetMainAssetBundleAndMainJsonData();
        SetStringsFromMainJsonAndLoadPrefs();
      
        StartCoroutine(GameValuesJsonLoader());
       
     //   LoadValuesFromJSON();

         GetReferences();

      //  AddAudioSources();
        ButtonAddListeners();

        GetCurrentSoundStateFromPrefs();

        SetMaterialShaders();
        Application.targetFrameRate = 60;
        /* if (isGooglePlayStoreVersion)
         {
             LoadLocalGameValues();
         }*/
        /*  if (mainJsonData != null)
          {
              print("paytm Version");
              StartCoroutine(GetGameValues());
              yield return new WaitForSeconds(1f);

          }*/
        yield return new WaitForSeconds(8f);
        StartGame();
        string loadingBarAmountPrefsText = "LoadingBarFillAmount";
        PlayerPrefs.SetFloat(devGameName + loadingBarAmountPrefsText, 1f);
        if (gameDownloader != null)
        {
            gameDownloader.SetTargetLoadingBarImageAmount(1);
        }

        if (isGooglePlayStoreVersion)
        {
           InitializeMobileAds();
        }
        yield return null;
    }

    void AddAudioSources()
    {
        GameObject AudioSourceParent = new GameObject();
        AudioSourceParent.name = "AudioSourceParent";
        for (int count = 0; count < 10; count++)
        {
            GameObject audioSource = new GameObject("audioSource" + count);
            audioSources.Add(audioSource.AddComponent<AudioSource>());
            audioSource.transform.SetParent(AudioSourceParent.transform);
            audioSources.Add(audioSource.GetComponent<AudioSource>());
        }

    }



    public void SetScreenOrientation()
    {
        Screen.orientation = ScreenOrientation.Portrait;
    }


    void GetReferences()
    {
        analyticsSetupDone = false;
        bool useLocalGamePrefab = false;
        if (gameDownloader != null)
        {
            useLocalGamePrefab = gameDownloader.useLocalGamePrefab;
        }

        if (mainAssetBundle != null)
        {
            print("assets loaded from bundle");
            if (isGooglePlayStoreVersion)
            {
                TapTapKnifePrefab = this.gameObject.transform.gameObject;

            }
            else
            {
                TapTapKnifePrefab = Instantiate(mainAssetBundle.LoadAsset<GameObject>("TapTapKnifeGame"));

            }


            #region related to prefab and art assets loading from assetbundle
         
          
            if (mainAssetBundle.LoadAsset<Sprite>("v2_01") != null)
            {
                print("assetbundle contains sprites");
            }
            boardSkins = new Sprite[10];
            boardSkins[0] = mainAssetBundle.LoadAsset<Sprite>("v2_01");
            boardSkins[1] = mainAssetBundle.LoadAsset<Sprite>("v2_02");
            boardSkins[2] = mainAssetBundle.LoadAsset<Sprite>("v2_03");
            boardSkins[3] = mainAssetBundle.LoadAsset<Sprite>("v2_04");
            boardSkins[4] = mainAssetBundle.LoadAsset<Sprite>("v2_05");
            boardSkins[5] = mainAssetBundle.LoadAsset<Sprite>("v2_06");
            boardSkins[6] = mainAssetBundle.LoadAsset<Sprite>("v2_07");
            boardSkins[7] = mainAssetBundle.LoadAsset<Sprite>("v2_08");
            boardSkins[8] = mainAssetBundle.LoadAsset<Sprite>("v2_09");
            boardSkins[9] = mainAssetBundle.LoadAsset<Sprite>("v2_10");

            bonusBoardSkins = new Sprite[3];
            bonusBoardSkins[0] = mainAssetBundle.LoadAsset<Sprite>("burger");
            bonusBoardSkins[1] = mainAssetBundle.LoadAsset<Sprite>("Doughnut");
            bonusBoardSkins[2] = mainAssetBundle.LoadAsset<Sprite>("pizza");

            brokenBoards = new GameObject[10];
            brokenBoards[0] = mainAssetBundle.LoadAsset<GameObject>("V201");
            brokenBoards[1] = mainAssetBundle.LoadAsset<GameObject>("V202");
            brokenBoards[2] = mainAssetBundle.LoadAsset<GameObject>("V203");
            brokenBoards[3] = mainAssetBundle.LoadAsset<GameObject>("V204");
            brokenBoards[4] = mainAssetBundle.LoadAsset<GameObject>("V205");
            brokenBoards[5] = mainAssetBundle.LoadAsset<GameObject>("V206");
            brokenBoards[6] = mainAssetBundle.LoadAsset<GameObject>("V207");
            brokenBoards[7] = mainAssetBundle.LoadAsset<GameObject>("V208");
            brokenBoards[8] = mainAssetBundle.LoadAsset<GameObject>("V209");
            brokenBoards[9] = mainAssetBundle.LoadAsset<GameObject>("V210");
            // brokenBoards[10] = mainAssetBundle.LoadAsset<GameObject>("v2_01");

            bonusBrokenBoards = new GameObject[3];
            bonusBrokenBoards[0] = mainAssetBundle.LoadAsset<GameObject>("burger");
            bonusBrokenBoards[1] = mainAssetBundle.LoadAsset<GameObject>("Doughnut");
            bonusBrokenBoards[2] = mainAssetBundle.LoadAsset<GameObject>("pizza");

            knifePrefab = mainAssetBundle.LoadAsset<GameObject>("Knife");

            easyBoards = new GameObject[5];
            easyBoards[0] = mainAssetBundle.LoadAsset<GameObject>("Easy_Board_1");
            easyBoards[1] = mainAssetBundle.LoadAsset<GameObject>("Easy_Board_2");
            easyBoards[2] = mainAssetBundle.LoadAsset<GameObject>("Easy_Board_3");
            easyBoards[3] = mainAssetBundle.LoadAsset<GameObject>("Easy_Board_4");
            easyBoards[4] = mainAssetBundle.LoadAsset<GameObject>("Easy_Board_5");

            easyMediumBoards = new GameObject[12];
            easyMediumBoards[0] = mainAssetBundle.LoadAsset<GameObject>("Easy_Board_1");
            easyMediumBoards[1] = mainAssetBundle.LoadAsset<GameObject>("Easy_Board_2");
            easyMediumBoards[2] = mainAssetBundle.LoadAsset<GameObject>("Easy_Board_3");
            easyMediumBoards[3] = mainAssetBundle.LoadAsset<GameObject>("Easy_Board_4");
            easyMediumBoards[4] = mainAssetBundle.LoadAsset<GameObject>("Easy_Board_5");
            easyMediumBoards[5] = mainAssetBundle.LoadAsset<GameObject>("Medium_Board_1");
            easyMediumBoards[6] = mainAssetBundle.LoadAsset<GameObject>("Medium_Board_2");
            easyMediumBoards[7] = mainAssetBundle.LoadAsset<GameObject>("Medium_Board_3");
            easyMediumBoards[8] = mainAssetBundle.LoadAsset<GameObject>("Medium_Board_4");
            easyMediumBoards[9] = mainAssetBundle.LoadAsset<GameObject>("Medium_Board_5");
            easyMediumBoards[10] = mainAssetBundle.LoadAsset<GameObject>("Medium_Board_6");
            easyMediumBoards[11] = mainAssetBundle.LoadAsset<GameObject>("Medium_Board_7");


            mediumBoards = new GameObject[7];
            mediumBoards[0] = mainAssetBundle.LoadAsset<GameObject>("Medium_Board_1");
            mediumBoards[1] = mainAssetBundle.LoadAsset<GameObject>("Medium_Board_2");
            mediumBoards[2] = mainAssetBundle.LoadAsset<GameObject>("Medium_Board_3");
            mediumBoards[3] = mainAssetBundle.LoadAsset<GameObject>("Medium_Board_4");
            mediumBoards[4] = mainAssetBundle.LoadAsset<GameObject>("Medium_Board_5");
            mediumBoards[5] = mainAssetBundle.LoadAsset<GameObject>("Medium_Board_6");
            mediumBoards[6] = mainAssetBundle.LoadAsset<GameObject>("Medium_Board_7");

            mediumHardBoards = new GameObject[12];
            mediumHardBoards[0] = mainAssetBundle.LoadAsset<GameObject>("Hard_Board_1");
            mediumHardBoards[1] = mainAssetBundle.LoadAsset<GameObject>("Hard_Board_2");
            mediumHardBoards[2] = mainAssetBundle.LoadAsset<GameObject>("Hard_Board_3");
            mediumHardBoards[3] = mainAssetBundle.LoadAsset<GameObject>("Hard_Board_4");
            mediumHardBoards[4] = mainAssetBundle.LoadAsset<GameObject>("Hard_Board_5");
            mediumHardBoards[5] = mainAssetBundle.LoadAsset<GameObject>("Medium_Board_1");
            mediumHardBoards[6] = mainAssetBundle.LoadAsset<GameObject>("Medium_Board_2");
            mediumHardBoards[7] = mainAssetBundle.LoadAsset<GameObject>("Medium_Board_3");
            mediumHardBoards[8] = mainAssetBundle.LoadAsset<GameObject>("Medium_Board_4");
            mediumHardBoards[9] = mainAssetBundle.LoadAsset<GameObject>("Medium_Board_5");
            mediumHardBoards[10] = mainAssetBundle.LoadAsset<GameObject>("Medium_Board_6");
            mediumHardBoards[11] = mainAssetBundle.LoadAsset<GameObject>("Medium_Board_7");

            hardBoards = new GameObject[5];
            hardBoards[0] = mainAssetBundle.LoadAsset<GameObject>("Hard_Board_1");
            hardBoards[1] = mainAssetBundle.LoadAsset<GameObject>("Hard_Board_2");
            hardBoards[2] = mainAssetBundle.LoadAsset<GameObject>("Hard_Board_3");
            hardBoards[3] = mainAssetBundle.LoadAsset<GameObject>("Hard_Board_4");
            hardBoards[4] = mainAssetBundle.LoadAsset<GameObject>("Hard_Board_5");

            easyHardBoards = new GameObject[10];
            easyHardBoards[0] = mainAssetBundle.LoadAsset<GameObject>("Hard_Board_1");
            easyHardBoards[1] = mainAssetBundle.LoadAsset<GameObject>("Hard_Board_2");
            easyHardBoards[2] = mainAssetBundle.LoadAsset<GameObject>("Hard_Board_3");
            easyHardBoards[3] = mainAssetBundle.LoadAsset<GameObject>("Hard_Board_4");
            easyHardBoards[4] = mainAssetBundle.LoadAsset<GameObject>("Hard_Board_5");
            easyHardBoards[5] = mainAssetBundle.LoadAsset<GameObject>("Easy_Board_1");
            easyHardBoards[6] = mainAssetBundle.LoadAsset<GameObject>("Easy_Board_2");
            easyHardBoards[7] = mainAssetBundle.LoadAsset<GameObject>("Easy_Board_3");
            easyHardBoards[8] = mainAssetBundle.LoadAsset<GameObject>("Easy_Board_4");
            easyHardBoards[9] = mainAssetBundle.LoadAsset<GameObject>("Easy_Board_5");

            bonusBoard = mainAssetBundle.LoadAsset<GameObject>("Bonus_Board_5");
            knifeUIObject= mainAssetBundle.LoadAsset<GameObject>("KnifeUI");
            plusFifty = mainAssetBundle.LoadAsset<GameObject>("Plus50");
            plusFiftyText = plusFifty.GetComponent<TextMeshProUGUI>();

            ActiveStageDot = mainAssetBundle.LoadAsset<Sprite>("dot fill");
            InActiveStageDots = mainAssetBundle.LoadAsset<Sprite>("dot");
            ActiveBonusDots = mainAssetBundle.LoadAsset<Sprite>("dot 01");
            InActiveBonusDots = mainAssetBundle.LoadAsset<Sprite>("dots 0");
            VolumeOFF = mainAssetBundle.LoadAsset<Sprite>("No sound");
            VolumeOn = mainAssetBundle.LoadAsset<Sprite>("sound");
            //testing
            collectibleVFX = mainAssetBundle.LoadAsset<GameObject>("VFX_JellyBlast");
             knifeHitKnifeVfx = mainAssetBundle.LoadAsset<GameObject>("VFX_OnKnifeHit");
             boardHitVFX = mainAssetBundle.LoadAsset<GameObject>("VFX_KnifeHit");
             boardDestroyVFX = mainAssetBundle.LoadAsset<GameObject>("VFX_WoodenBoard Explosion V2");
             knifeUIObject = mainAssetBundle.LoadAsset<GameObject>("KnifeUI");
             circleMat = mainAssetBundle.LoadAsset<Material>("circle 1"); 
             glowOneMat = mainAssetBundle.LoadAsset<Material>("glow 1");
             glowMat = mainAssetBundle.LoadAsset<Material>("Glow_add_mat");
             hitABMat = mainAssetBundle.LoadAsset<Material>("Glow_add_mat");
             kHitABMat = mainAssetBundle.LoadAsset<Material>("Khit_AB_mat");
             rayABMat = mainAssetBundle.LoadAsset<Material>("Ray_AB_mat");
             ringABMat = mainAssetBundle.LoadAsset<Material>("Ring_AB_mat");
              triangleSoftOne = mainAssetBundle.LoadAsset<Material>("triangle_soft 1");
              woodABMat = mainAssetBundle.LoadAsset<Material>("Wood_AB_mat");
              woodABMatOne = mainAssetBundle.LoadAsset<Material>("Wood_AB_mat 1");
              woodABMatTwo = mainAssetBundle.LoadAsset<Material>("Wood_AB_mat 2");

             
            #endregion

            #region Related to Child gameobjects
            if (!isGooglePlayStoreVersion)
            {
                stageText = TapTapKnifePrefab.transform.GetChild(1).transform.GetChild(4).GetComponent<TextMeshProUGUI>();
                scoreText = TapTapKnifePrefab.transform.GetChild(1).transform.GetChild(3).GetComponent<TextMeshProUGUI>();
                StageDots = new Image[5];
                StageDots[0] = TapTapKnifePrefab.transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
                StageDots[1] = TapTapKnifePrefab.transform.GetChild(1).transform.GetChild(0).transform.GetChild(1).GetComponent<Image>();
                StageDots[2] = TapTapKnifePrefab.transform.GetChild(1).transform.GetChild(0).transform.GetChild(2).GetComponent<Image>();
                StageDots[3] = TapTapKnifePrefab.transform.GetChild(1).transform.GetChild(0).transform.GetChild(3).GetComponent<Image>();
                StageDots[4] = TapTapKnifePrefab.transform.GetChild(1).transform.GetChild(0).transform.GetChild(4).GetComponent<Image>();

                knifeUIParent = TapTapKnifePrefab.transform.GetChild(1).transform.GetChild(1).gameObject;
                homeScreenPanel = TapTapKnifePrefab.transform.GetChild(1).transform.GetChild(6).gameObject;
                playButton = homeScreenPanel.transform.GetChild(1).GetComponent<Button>();
                volumeButton = homeScreenPanel.transform.GetChild(3).GetComponent<Button>();
                infoButton = homeScreenPanel.transform.GetChild(2).GetComponent<Button>();
                volumeImage = homeScreenPanel.transform.GetChild(3).GetComponent<Image>();

                gameOverPanel = TapTapKnifePrefab.transform.GetChild(1).transform.GetChild(8).gameObject;
                restartButton = gameOverPanel.transform.GetChild(1).GetComponent<Button>();
                homeButton = gameOverPanel.transform.GetChild(0).GetComponent<Button>();


                tutorialPanel = TapTapKnifePrefab.transform.GetChild(1).transform.GetChild(10).gameObject;
                tutoralButton = tutorialPanel.GetComponent<Button>();
                redPanel = TapTapKnifePrefab.transform.GetChild(1).transform.GetChild(9).gameObject;
                canvas = TapTapKnifePrefab.transform.GetChild(1).GetComponent<Canvas>();
                float posY = canvas.GetComponent<RectTransform>().sizeDelta.y / 2;
                print(posY - 10f);
                plusFiftyFinalPos = new Vector3(0f, posY - 50f, 0f);

                collectibleSFX = TapTapKnifePrefab.transform.GetChild(1).transform.GetChild(5).transform.GetChild(0).GetComponent<AudioSource>();
                knifeHitBoardSFX = TapTapKnifePrefab.transform.GetChild(1).transform.GetChild(5).transform.GetChild(1).GetComponent<AudioSource>(); ;
                knifeHitKnifeSFX = TapTapKnifePrefab.transform.GetChild(1).transform.GetChild(5).transform.GetChild(2).GetComponent<AudioSource>(); ;
                boardBreakSFX = TapTapKnifePrefab.transform.GetChild(1).transform.GetChild(5).transform.GetChild(3).GetComponent<AudioSource>(); ;
                buttonSFX = TapTapKnifePrefab.transform.GetChild(1).transform.GetChild(5).transform.GetChild(6).GetComponent<AudioSource>(); ;


                 homeFromPauseButton = PauseScreenObject.transform.GetChild(2).transform.GetChild(0).GetComponent<Button>();
               

                 resumeButton = PauseScreenObject.transform.GetChild(2).transform.GetChild(1).GetComponent<Button>();
                

                /**/
            }

            #endregion

            #region Related to Loading Numerical Values
            /* float posY = canvas.GetComponent<RectTransform>().sizeDelta.y / 2;
             print(posY - 10f);
             plusFiftyFinalPos = new Vector3(0f, posY - 50f, 0f);*/

            if (isGooglePlayStoreVersion == false)
              {
                if (File.Exists(scriptFilePath))
                 {

                      assembly = System.Reflection.Assembly.LoadFrom(scriptFilePath);
                      BreakingForce = assembly.GetType("BreakingForce");
                      Knife = assembly.GetType("Knife");
                      KnifeForce = assembly.GetType("KnifeForce");

                    //  slotPrefab.AddComponent<Slot>();
                    knifePrefab.AddComponent(Knife);
                    knifePrefab.AddComponent(KnifeForce);
                    knifePrefab.GetComponent<KnifeForce>().enabled = false;


                }
              }


        }
       // BgRenderer = TapTapKnifePrefab.transform.GetChild(0).GetComponent<SpriteRenderer>();
      //  BgImageScaler();




        

        if (analyticsGameobject == null)
        {
            if (isGooglePlayStoreVersion)
            {
                if (analyticsGameobject == null)
                {
                    analyticsGameobject = new GameObject("Analytics");
                    analyticsGameobject.SetActive(false);
                    analyticsGameobject.AddComponent<Analytics>().enabled = false;
                }
                if (mainJsonData != null)
                {
                    string trackingId = "";

                    if (mainJsonData["mainData"]["analyticsTrackingID"])
                    {
                        trackingId = mainJsonData["mainData"]["analyticsTrackingID"];
                    }


                    string appName = gameTitle;

                    if (mainJsonData["mainData"]["analyticsAppName"])
                    {
                        appName = mainJsonData["mainData"]["analyticsAppName"];
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
        


    }

    IEnumerator GameValuesJsonLoader()
    {
        
        string gameFolderName = devGameName + "Data";
        string JSONFileName = devGameName + "GameValues";
        string folderPath = Application.persistentDataPath + "/" + gameFolderName;
        string filePath = folderPath + "/" + JSONFileName;
        string gameValuesURL = mainJsonData["mainData"]["gameValuesUrl"];
       
        WWW JsonRequest = new WWW(gameValuesURL);
        yield return JsonRequest;
        if (JsonRequest.error == null)
        {
            print("gamevaluesLoaded");
            print(JsonRequest.text);
            gameValuesJsonData = SimpleJSON.JSON.Parse(JsonRequest.text);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
                File.WriteAllBytes(filePath, JsonRequest.bytes);
            }
            else
            {
                File.WriteAllBytes(filePath, JsonRequest.bytes);
            }

        }
        LoadValuesFromJSON();
    }

    private void LoadValuesFromJSON()
    {
        if (gameValuesJsonData == null&&isGooglePlayStoreVersion)
        {
            print("Loaded locally");
            gameValuesJsonData = SimpleJSON.JSON.Parse(localJsonGameValuesText.text);

        }





        #region Related to Loading Numerical Values
      /*  float posY = canvas.GetComponent<RectTransform>().sizeDelta.y / 2;
        print(posY - 10f);
        plusFiftyFinalPos = new Vector3(0f, posY - 50f, 0f);
        */
        knifeInitialPos = new Vector3(0f, -3, 0f);
        knifeThrownPos = new Vector3(0f, -0.2f, 0f);
        particlePos = new Vector3(0f, -0.5f, 0f);
        KnifehitparticlePos = new Vector3(0f, -.1f, 0f);
        knifeFadeLerpSpeed =20;
      
        print(gameValuesJsonData["mainData"]["knifeThrowSpeed"]);
        knifeThrowSpeed = gameValuesJsonData["mainData"]["knifeThrowSpeed"]; 

        rotationSpeed =new float[] { 4, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };
        numOfKnives = new Vector2[] { new Vector2(6f, 12f), new Vector2(6, 16), new Vector2(6, 16), new Vector2(6, 16), new Vector2(12, 18), new Vector2(6, 18) };
        stopWaitTime = gameValuesJsonData["mainData"]["stopWaitTime"];
        knifeHitPoints = gameValuesJsonData["mainData"]["knifePoints"];
        collectiblesPoints = gameValuesJsonData["mainData"]["collectiblePoints"];
        forwardBackwardAngles= new Vector3[]{ new Vector3(0f, 0f,3f), new Vector3(0f, 0f, 3f), new Vector3(0f, 0f, 3f), new Vector3(0f, 0f, 3f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(180f, 360f, 5f), new Vector3(180f, 0f, 0f), new Vector3(90f, 180f, 9f), new Vector3(90f, 180f, 9f), new Vector3(45f, 90f, 17f), new Vector3(45f, 90f, 17f), new Vector3(30f, 150f, 7f) };
        #endregion

       
                   

    }
    public void BgImageScaler()
    {
        Camera.main.transform.position = camerPos;
        Camera.main.orthographicSize = 5f;
        /*   float screenRatio = (float)Screen.width / (float)Screen.height;
          float targetRatio = BgRenderer.bounds.size.x / BgRenderer.bounds.size.y;

          if (screenRatio >= targetRatio)
           {
               Camera.main.orthographicSize = BgRenderer.bounds.size.y / 2;
           }
           else
           {
               float differenceInSize = targetRatio / screenRatio;
               Camera.main.orthographicSize = BgRenderer.bounds.size.y / 2 * differenceInSize;
           }

        float screenHeight = Camera.main.orthographicSize * 2f;
        float screenWidth = screenHeight / Screen.height * Screen.width;
        float width = screenWidth / BgRenderer.sprite.bounds.size.x;
        float height = screenHeight / BgRenderer.sprite.bounds.size.y;
        BgRenderer.transform.localScale = new Vector3(width, height, 1f);

        */


    }

    public void GetMainAssetBundleAndMainJsonData()
    {
        mainJsonData = null;
        isGooglePlayStoreVersion = false;

        gameDownloader = GameDownloader.instance;
        GameObject abdObj = null;
        if (gameDownloader == null)
        {
            string abdString = "GameDownloader";
            abdObj = GameObject.Find(abdString);
        }
        else
        {
            abdObj = gameDownloader.gameObject;
        }
        if (abdObj != null)
        {
            if (abdObj.GetComponent<GameDownloader>())
            {
                if (gameDownloader == null)
                {
                    gameDownloader = abdObj.GetComponent<GameDownloader>();
                }
            }
            if (gameDownloader == null)
            {
                Debug.Log("GameDownloader component not found");
            }
            else
            {
                mainAssetBundle = gameDownloader.GetMainAssetBundle();
                mainJsonData = gameDownloader.GetMainJsonData();
                if (gameDownloader.isTargetGooglePlayStore())
                {
                    isGooglePlayStoreVersion = true;
                }
            }
        }
        else
        {
            Debug.Log("GameDownloader Gameobject not found");
        }

        if (mainAssetBundle == null)
        {
            string assetBundleFilePath = PlayerPrefs.GetString(devGameName + "assetBundleFilePath");
            Debug.Log("mainAssetBundle " + assetBundleFilePath);
            if (File.Exists(assetBundleFilePath))
            {
              //  mainAssetBundle = AssetBundle.LoadFromFile(assetBundleFilePath);
            }
        }

        if (mainJsonData == null)
        {
            mainJsonDataText = PlayerPrefs.GetString(devGameName + "mainJsonDataText");
            print("mainJsonDataText");
            print(mainJsonDataText);
            if (mainJsonDataText.Length > 10)
            {
                mainJsonData = SimpleJSON.JSON.Parse(mainJsonDataText);
            }
            print("mainJsonData");
            print(mainJsonData);
        }

        scriptFilePath = PlayerPrefs.GetString(devGameName + "scriptFilePath");
    }

    public void SetStringsFromMainJsonAndLoadPrefs()
    {
        if (mainJsonData != null)
        {
            gameTitle = mainJsonData["mainData"]["gameTitle"];
            devGameName = mainJsonData["mainData"]["devGameName"];
        }

        gameFolderName = devGameName + "Data";
        gameValuesFileName = devGameName + "GameValues.json";


        soundStringPrefs = devGameName + "Sounds";
        gameValuesVersionPrefs = devGameName + "GameValuesVersion";
    }

    public void GetCurrentSoundStateFromPrefs()
    {
        currentSoundState = PlayerPrefs.GetInt(soundStringPrefs);
    }
    Shader textShader;
    void SetMaterialShaders()
    {
     
        {
            List<TextMeshProUGUI> allTexts = new List<TextMeshProUGUI>();
            allTexts.Add(scoreText);
            allTexts.Add(stageText);
            allTexts.Add(plusFiftyText);

             textShader = Shader.Find("TextMeshPro/Distance Field");
           // plusFifty.materialForRendering.shader = textShader;
            for (int count = 0; count < allTexts.Count; count++)
            {
                allTexts[count].materialForRendering.shader = textShader;
            }
             circleMat.shader= Shader.Find("Legacy Shaders/Particles/Alpha Blended");          
             glowOneMat.shader = Shader.Find("Legacy Shaders/Particles/Alpha Blended"); ;
             glowMat.shader = Shader.Find("Legacy Shaders/Particles/Additive"); ;
             hitABMat.shader = Shader.Find("Legacy Shaders/Particles/Alpha Blended"); ;
             kHitABMat.shader = Shader.Find("Legacy Shaders/Particles/Additive"); ;
             rayABMat.shader = Shader.Find("Legacy Shaders/Particles/Alpha Blended"); ;
             ringABMat.shader = Shader.Find("Legacy Shaders/Particles/Alpha Blended"); ;
             triangleSoftOne.shader = Shader.Find("Legacy Shaders/Particles/Alpha Blended"); ;
             woodABMat.shader = Shader.Find("Legacy Shaders/Particles/Alpha Blended"); ;
             woodABMatOne.shader = Shader.Find("Legacy Shaders/Particles/Alpha Blended"); ;
             woodABMatTwo.shader = Shader.Find("Legacy Shaders/Particles/Alpha Blended"); ;
            
        }

    }

     public void InitializeMobileAds()
     {
         string appId = "ca-app-pub-3940256099942544~3347511713";
         if (mainJsonData != null)
         {
             if (mainJsonData["mainData"]["admobAppId"])
             {
                 appId = mainJsonData["mainData"]["admobAppId"];
                 print(appId);
             }
         }
         MobileAds.Initialize(appId);
         this.rewardBasedVideo = RewardBasedVideoAd.Instance;
         rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
         RequestRewardBasedVideo();

     }
     RewardBasedVideoAd rewardBasedVideo;


     private void RequestRewardBasedVideo()
     {
         string adUnitId = "ca-app-pub-3940256099942544/5224354917";
         if (mainJsonData != null)
         {
             if (mainJsonData["mainData"]["admobAdId"])
             {
                 adUnitId = mainJsonData["mainData"]["admobAdId"];
             }
             if (mainJsonData["mainData"]["deathAdInterval"])
             {
                 deathAdInterval = mainJsonData["mainData"]["deathAdInterval"];
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

     private void ShowVideoAd()
     {
         if (rewardBasedVideo.IsLoaded())
         {

             rewardBasedVideo.Show();
         }
     }
    /*       private void Start()
       {        
           ButtonAddListeners();
           StartGame();
       }*/
    int deathAdInterval = 3;
    int numOfDeaths = 0;
    public void HandleDeathVideoAd()
    {
        numOfDeaths++;
        if (numOfDeaths % deathAdInterval == 0)
        {
            if (isGooglePlayStoreVersion)
            {
                ShowVideoAd();
            }
        }
    }

        private void Update()
    {



        if (Input.GetMouseButtonDown(0) && canThrow && gameStarted && knivesLeft >= 0&&PauseScreenObject.activeInHierarchy==false)
        {

            canThrow = false;
            isAttacking = true;
            KnifeUIUpdater();
            if (knivesLeft != 0)
            {
                StartCoroutine(Shake(boardShakeDuration, shakeMagnitude));
                StartCoroutine(ThrowKnife(knifeObjects[knivesLeft], false));
                knivesLeft -= 1;
                knifeObjects[knivesLeft].SetActive(true);
                StartCoroutine(coKnifeFadeIn(knifeObjects[knivesLeft]));
            }
            else
            {
                StartCoroutine(ThrowKnife(knifeObjects[knivesLeft], true));
            }
        }

        if (Application.platform == RuntimePlatform.Android && isGooglePlayStoreVersion)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && homeScreenPanel.activeInHierarchy)
            {
                HandlleBackButton(true);
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && gameOverPanel.activeInHierarchy)
            {
                HandlleBackButton(true);
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && !homeScreenPanel.activeInHierarchy && !gameOverPanel.activeInHierarchy)
            {
                HandlleBackButton(false);
            }
        }

    }

    void HandlleBackButton(bool canQuit)
    {
        if (!canQuit)
        {
            DisplayPauseScreen();
        }
        else
        {
            print("Game Quit");
            Application.Quit();
        }
    }

    void DisplayPauseScreen()
    {
        PauseScreenObject.SetActive(true);
        Time.timeScale = 0;
        
    }

    

 
    void HomeFromPauseScreen()
    {
        Time.timeScale = 1;
        PauseScreenObject.SetActive(false);
     

        StopAllCoroutines();
        gameStarted = false;
        foreach (GameObject plusFiftyUnit in plusFifties)
        {
            if (plusFiftyUnit != null)
            {
                Destroy(plusFiftyUnit);
            }
        }
        plusFifties = new List<GameObject>();
        Camera.main.transform.position = new Vector3(0f, 0f, -10f);
        Destroy(currentBoard);
        for (int i = 0; i < knifeObjects.Count; i++)
        {
            Destroy(knifeObjects[i]);

        }
        for (int i = 0; i < UIKnives.Count; i++)
        {
            Destroy(UIKnives[i]);
        }

        knivesLeft = 0;
        UIKnives.Clear();
        isAttacking = false;
        score = 0;
        UpdateScore(0);
        StageDotsUI();
        OnHomeButtonClicked();
       
    }

    void ResumeGame()
    {
        Time.timeScale = 1;
        PauseScreenObject.SetActive(false);
        print("resumed");
       
    }

    private void OnApplicationPause(bool pause)
    {
        if (!homeScreenPanel.activeInHierarchy && !gameOverPanel.activeInHierarchy)
        {
            HandlleBackButton(false);
        }
    }
    #region Related to Rotation Based on Difficulty

    enum RotationEasy
    {
        Continuous_1,
        Continuous_2,
        SlowInSlowOut,
        SlowInSlowOut_3,
        ReverseSlowInSlowOut,
        Max
    }

    enum RotationEasyMedium
    {
        Continuous_1,
        Continuous_2,
        SlowInSlowOut,
        SlowInSlowOut_3,
        ReverseSlowInSlowOut,

        StopAndGo,
        StopAndReverse,
        Rotate_180_Reverse,
        Rotate_180_StopAndGo,
        Rotate_90_Reverse_180_SlowInSlowOut,
        Max
    }

    enum RotationMedium
    {
        StopAndGo,
        StopAndReverse,
        Rotate_180_Reverse,
        Rotate_180_StopAndGo,
        Rotate_90_Reverse_180_SlowInSlowOut,
        Max
    }

    enum RotationMediumHard
    {
        StopAndGo,
        StopAndReverse,
        Rotate_180_Reverse,
        Rotate_180_StopAndGo,
        Rotate_90_Reverse_180_SlowInSlowOut,

        Rotate_90_Reverse_180,
        Rotate_45_Reverse_90,
        Rotate_45_Reverse_90_SlowInSlowOut,
        Rotate_30_Reverse_150,
        Max
    }

    enum RotationHard
    {
        Rotate_90_Reverse_180,
        Rotate_45_Reverse_90,
        Rotate_45_Reverse_90_SlowInSlowOut,
        Rotate_30_Reverse_150,
        Max
    }

    enum RotationEasyHard
    {
        Continuous_1,
        Continuous_2,
        SlowInSlowOut,
        SlowInSlowOut_3,
        ReverseSlowInSlowOut,
        Rotate_90_Reverse_180,
        Rotate_45_Reverse_90,
        Rotate_45_Reverse_90_SlowInSlowOut,
        Rotate_30_Reverse_150,
        Max
    }

    void EasyRotation(RotationEasy easy)
    {
        switch (easy)
        {
            case RotationEasy.Continuous_1:
                StartCoroutine(coContinuous(rotationSpeed[0]));
                break;

            case RotationEasy.Continuous_2:
                StartCoroutine(coContinuous(rotationSpeed[1]));
                break;

            case RotationEasy.SlowInSlowOut:
                StartCoroutine(coSlowInSlowOut(rotationSpeed[2], 0, true, false));
                break;

            case RotationEasy.SlowInSlowOut_3:
                StartCoroutine(coSlowInSlowOut(rotationSpeed[3], 3, true, false));
                break;

            case RotationEasy.ReverseSlowInSlowOut:
                StartCoroutine(coSlowInSlowOut(rotationSpeed[4], 0, true, true));
                break;

            default:
                break;
        }
    }

    void EasyMediumRotation(RotationEasyMedium easyMedium)
    {
        switch (easyMedium)
        {
            case RotationEasyMedium.Continuous_1:
                StartCoroutine(coContinuous(rotationSpeed[0]));
                break;

            case RotationEasyMedium.Continuous_2:
                StartCoroutine(coContinuous(rotationSpeed[1]));
                break;

            case RotationEasyMedium.SlowInSlowOut:
                StartCoroutine(coSlowInSlowOut(rotationSpeed[2], 0, true, false));
                break;

            case RotationEasyMedium.SlowInSlowOut_3:
                StartCoroutine(coSlowInSlowOut(rotationSpeed[3], 3, true, false));
                break;

            case RotationEasyMedium.ReverseSlowInSlowOut:
                StartCoroutine(coSlowInSlowOut(rotationSpeed[4], 0, true, true));
                break;

            case RotationEasyMedium.StopAndGo:
                StartCoroutine(coSlowInSlowOut(rotationSpeed[5], 0, false, false));
                break;

            case RotationEasyMedium.StopAndReverse:
                StartCoroutine(coSlowInSlowOut(rotationSpeed[6], 0, false, true));
                break;

            case RotationEasyMedium.Rotate_180_Reverse:
                StartCoroutine(coReverseAtAngle(rotationSpeed[7], forwardBackwardAngles[7].x, forwardBackwardAngles[7].y, (int)forwardBackwardAngles[7].z, false, false));
                break;

            case RotationEasyMedium.Rotate_180_StopAndGo:
                StartCoroutine(coReverseAtAngle(rotationSpeed[8], forwardBackwardAngles[8].x, forwardBackwardAngles[8].y, (int)forwardBackwardAngles[8].z, false, true));
                break;

            case RotationEasyMedium.Rotate_90_Reverse_180_SlowInSlowOut:
                StartCoroutine(coReverseAtAngle(rotationSpeed[9], forwardBackwardAngles[9].x, forwardBackwardAngles[9].y, (int)forwardBackwardAngles[9].z, true, false));
                break;

            default:
                break;
        }
    }

    void MediumRotaion(RotationMedium medium)
    {
        switch (medium)
        {
            case RotationMedium.StopAndGo:
                StartCoroutine(coSlowInSlowOut(rotationSpeed[5], 0, false, false));
                break;

            case RotationMedium.StopAndReverse:
                StartCoroutine(coSlowInSlowOut(rotationSpeed[6], 0, false, true));
                break;

            case RotationMedium.Rotate_180_Reverse:
                StartCoroutine(coReverseAtAngle(rotationSpeed[7], forwardBackwardAngles[7].x, forwardBackwardAngles[7].y, (int)forwardBackwardAngles[7].z, false, false));
                break;

            case RotationMedium.Rotate_180_StopAndGo:
                StartCoroutine(coReverseAtAngle(rotationSpeed[8], forwardBackwardAngles[8].x, forwardBackwardAngles[8].y, (int)forwardBackwardAngles[8].z, false, true));
                break;

            case RotationMedium.Rotate_90_Reverse_180_SlowInSlowOut:
                StartCoroutine(coReverseAtAngle(rotationSpeed[9], forwardBackwardAngles[9].x, forwardBackwardAngles[9].y, (int)forwardBackwardAngles[9].z, true, false));
                break;

            default:
                break;
        }
    }

    void MediumHardRotation(RotationMediumHard mediumHard)
    {
        switch (mediumHard)
        {
            case RotationMediumHard.StopAndGo:
                StartCoroutine(coSlowInSlowOut(rotationSpeed[5], 0, false, false));
                break;

            case RotationMediumHard.StopAndReverse:
                StartCoroutine(coSlowInSlowOut(rotationSpeed[6], 0, false, true));
                break;

            case RotationMediumHard.Rotate_180_Reverse:
                StartCoroutine(coReverseAtAngle(rotationSpeed[7], forwardBackwardAngles[7].x, forwardBackwardAngles[7].y, (int)forwardBackwardAngles[7].z, false, false));
                break;

            case RotationMediumHard.Rotate_180_StopAndGo:
                StartCoroutine(coReverseAtAngle(rotationSpeed[8], forwardBackwardAngles[8].x, forwardBackwardAngles[8].y, (int)forwardBackwardAngles[8].z, false, true));
                break;

            case RotationMediumHard.Rotate_90_Reverse_180_SlowInSlowOut:
                StartCoroutine(coReverseAtAngle(rotationSpeed[9], forwardBackwardAngles[9].x, forwardBackwardAngles[9].y, (int)forwardBackwardAngles[9].z, true, false));
                break;

            case RotationMediumHard.Rotate_90_Reverse_180:
                StartCoroutine(coReverseAtAngle(rotationSpeed[10], forwardBackwardAngles[10].x, forwardBackwardAngles[10].y, (int)forwardBackwardAngles[10].z, false, false));
                break;

            case RotationMediumHard.Rotate_45_Reverse_90:
                StartCoroutine(coReverseAtAngle(rotationSpeed[11], forwardBackwardAngles[11].x, forwardBackwardAngles[11].y, (int)forwardBackwardAngles[11].z, false, false));
                break;

            case RotationMediumHard.Rotate_45_Reverse_90_SlowInSlowOut:
                StartCoroutine(coReverseAtAngle(rotationSpeed[12], forwardBackwardAngles[12].x, forwardBackwardAngles[12].y, (int)forwardBackwardAngles[12].z, true, false));
                break;

            case RotationMediumHard.Rotate_30_Reverse_150:
                StartCoroutine(coReverseAtAngle(rotationSpeed[13], forwardBackwardAngles[13].x, forwardBackwardAngles[13].y, (int)forwardBackwardAngles[13].z, false, false));
                break;

            default:
                break;
        }
    }

    void HardRotation(RotationHard hard)
    {
        switch (hard)
        {
            case RotationHard.Rotate_90_Reverse_180:
                StartCoroutine(coReverseAtAngle(rotationSpeed[10], forwardBackwardAngles[10].x, forwardBackwardAngles[10].y, (int)forwardBackwardAngles[10].z, false, false));
                break;

            case RotationHard.Rotate_45_Reverse_90:
                StartCoroutine(coReverseAtAngle(rotationSpeed[11], forwardBackwardAngles[11].x, forwardBackwardAngles[11].y, (int)forwardBackwardAngles[11].z, false, false));
                break;

            case RotationHard.Rotate_45_Reverse_90_SlowInSlowOut:
                StartCoroutine(coReverseAtAngle(rotationSpeed[12], forwardBackwardAngles[12].x, forwardBackwardAngles[12].y, (int)forwardBackwardAngles[12].z, true, false));
                break;

            case RotationHard.Rotate_30_Reverse_150:
                StartCoroutine(coReverseAtAngle(rotationSpeed[13], forwardBackwardAngles[13].x, forwardBackwardAngles[13].y, (int)forwardBackwardAngles[13].z, false, false));
                break;

            default:
                break;
        }
    }

    void EasyHardRotation(RotationEasyHard easyHard)
    {
        switch (easyHard)
        {
            case RotationEasyHard.Continuous_1:
                StartCoroutine(coContinuous(rotationSpeed[0]));
                break;

            case RotationEasyHard.Continuous_2:
                StartCoroutine(coContinuous(rotationSpeed[1]));
                break;

            case RotationEasyHard.SlowInSlowOut:
                StartCoroutine(coSlowInSlowOut(rotationSpeed[2], 0, true, false));
                break;

            case RotationEasyHard.SlowInSlowOut_3:
                StartCoroutine(coSlowInSlowOut(rotationSpeed[3], 3, true, false));
                break;

            case RotationEasyHard.ReverseSlowInSlowOut:
                StartCoroutine(coSlowInSlowOut(rotationSpeed[4], 0, true, true));
                break;

            case RotationEasyHard.Rotate_90_Reverse_180:
                StartCoroutine(coReverseAtAngle(rotationSpeed[10], forwardBackwardAngles[10].x, forwardBackwardAngles[10].y, (int)forwardBackwardAngles[10].z, false, false));
                break;

            case RotationEasyHard.Rotate_45_Reverse_90:
                StartCoroutine(coReverseAtAngle(rotationSpeed[11], forwardBackwardAngles[11].x, forwardBackwardAngles[11].y, (int)forwardBackwardAngles[11].z, false, false));
                break;

            case RotationEasyHard.Rotate_45_Reverse_90_SlowInSlowOut:
                StartCoroutine(coReverseAtAngle(rotationSpeed[12], forwardBackwardAngles[12].x, forwardBackwardAngles[12].y, (int)forwardBackwardAngles[12].z, true, false));
                break;

            case RotationEasyHard.Rotate_30_Reverse_150:
                StartCoroutine(coReverseAtAngle(rotationSpeed[13], forwardBackwardAngles[13].x, forwardBackwardAngles[13].y, (int)forwardBackwardAngles[13].z, false, false));
                break;

            default:
                break;
        }
    }

    #endregion

    #region Related to coRoutines of Rotations

    IEnumerator coContinuous(float seconds)
    {
        while (currentBoard != null)
        {
            rotationType = new AnimationCurve(new Keyframe(0, 0), new Keyframe(seconds, 360f));
            rotationType.preWrapMode = WrapMode.Loop;
            rotationType.postWrapMode = WrapMode.Loop;

            SetCurveLinear(rotationType);

            currentBoard.transform.eulerAngles = new Vector3(currentBoard.transform.eulerAngles.x, currentBoard.transform.eulerAngles.y, rotationType.Evaluate(Time.time));
            yield return null;
        }

    }

    IEnumerator coSlowInSlowOut(float seconds, int numOfRotations, bool isSlowInSlowOut, bool isReverse)
    {
        while (currentBoard != null)
        {
            if (numOfRotations > 0)
            {
                rotationType = new AnimationCurve(new Keyframe(0, 0), new Keyframe(seconds, 360f),
                                                  new Keyframe(seconds * 2, 360f * 2), new Keyframe(seconds * 3, 360f * 3),
                                                  new Keyframe((seconds * 3) + stopWaitTime, 360f * 3));
                rotationType.preWrapMode = WrapMode.Loop;
                rotationType.postWrapMode = WrapMode.Loop;

                for (int i = 0; i < rotationType.keys.Length; i++)
                {
                    rotationType.SmoothTangents(i, 0);
                }

                currentBoard.transform.eulerAngles = new Vector3(currentBoard.transform.eulerAngles.x, currentBoard.transform.eulerAngles.y, rotationType.Evaluate(Time.time));
                yield return null;
            }
            else
            {
                if (isReverse)
                {
                    if (isSlowInSlowOut)
                    {
                        rotationType = new AnimationCurve(new Keyframe(0, 0), new Keyframe(stopWaitTime, 0), new Keyframe(seconds, 360f), new Keyframe(seconds + stopWaitTime, 360f));
                        rotationType.preWrapMode = WrapMode.PingPong;
                        rotationType.postWrapMode = WrapMode.PingPong;

                        currentBoard.transform.eulerAngles = new Vector3(currentBoard.transform.eulerAngles.x, currentBoard.transform.eulerAngles.y, rotationType.Evaluate(Time.time));
                        yield return null;
                    }
                    else
                    {
                        rotationType = new AnimationCurve(new Keyframe(0, 0), new Keyframe(stopWaitTime, 0), new Keyframe(seconds, 360f), new Keyframe(seconds + stopWaitTime, 360f));
                        rotationType.preWrapMode = WrapMode.PingPong;
                        rotationType.postWrapMode = WrapMode.PingPong;

                        SetCurveLinear(rotationType);

                        currentBoard.transform.eulerAngles = new Vector3(currentBoard.transform.eulerAngles.x, currentBoard.transform.eulerAngles.y, rotationType.Evaluate(Time.time));
                        yield return null;
                    }
                }
                else
                {
                    if (isSlowInSlowOut)
                    {
                        rotationType = new AnimationCurve(new Keyframe(0, 0), new Keyframe(seconds, 360f), new Keyframe(seconds + stopWaitTime, 360f));
                        rotationType.preWrapMode = WrapMode.Loop;
                        rotationType.postWrapMode = WrapMode.Loop;

                        currentBoard.transform.eulerAngles = new Vector3(currentBoard.transform.eulerAngles.x, currentBoard.transform.eulerAngles.y, rotationType.Evaluate(Time.time));
                        yield return null;
                    }
                    else
                    {
                        rotationType = new AnimationCurve(new Keyframe(0, 0), new Keyframe(seconds, 360f), new Keyframe(seconds + stopWaitTime, 360f));
                        rotationType.preWrapMode = WrapMode.Loop;
                        rotationType.postWrapMode = WrapMode.Loop;

                        SetCurveLinear(rotationType);

                        currentBoard.transform.eulerAngles = new Vector3(currentBoard.transform.eulerAngles.x, currentBoard.transform.eulerAngles.y, rotationType.Evaluate(Time.time));
                        yield return null;
                    }
                }
            }

        }
    }

    IEnumerator coReverseAtAngle(float seconds, float forwardAngle, float backwardAngle, int fames, bool isSlowInSlowOut, bool isStopAndGo)
    {
        while (currentBoard != null)
        {
            if (isStopAndGo)
            {
                rotationType = new AnimationCurve(new Keyframe(0, 0), new Keyframe(seconds, 180f), new Keyframe(seconds + stopWaitTime, 180f),
                    new Keyframe(seconds * 2, 360f), new Keyframe((seconds * 2) + stopWaitTime, 360f));
                rotationType.preWrapMode = WrapMode.Loop;
                rotationType.postWrapMode = WrapMode.Loop;

                SetCurveLinear(rotationType);

                currentBoard.transform.eulerAngles = new Vector3(currentBoard.transform.eulerAngles.x, currentBoard.transform.eulerAngles.y, rotationType.Evaluate(Time.time));
                yield return null;
            }
            else
            {
                rotationType = new AnimationCurve();
                Keyframe[] newKeys = new Keyframe[fames];
                int multiplier = 0;
                float currentAngle = 0;

                for (int i = 0; i < newKeys.Length; i++)
                {
                    if (i == 0)
                    {
                        newKeys[i].time = 0;
                        newKeys[i].value = 0;
                    }
                    else if (i % 2 != 0)
                    {
                        newKeys[i].time = seconds * multiplier;
                        currentAngle += forwardAngle;
                        newKeys[i].value = currentAngle;
                    }
                    else
                    {
                        newKeys[i].time = seconds * multiplier;
                        currentAngle -= backwardAngle;
                        newKeys[i].value = currentAngle;
                    }

                    multiplier++;

                    rotationType.AddKey(newKeys[i]);
                }

                rotationType.preWrapMode = WrapMode.Loop;
                rotationType.postWrapMode = WrapMode.Loop;

                if (isSlowInSlowOut)
                {
                    currentBoard.transform.eulerAngles = new Vector3(currentBoard.transform.eulerAngles.x, currentBoard.transform.eulerAngles.y, rotationType.Evaluate(Time.time));
                    yield return null;
                }
                else
                {
                    SetCurveLinear(rotationType);

                    currentBoard.transform.eulerAngles = new Vector3(currentBoard.transform.eulerAngles.x, currentBoard.transform.eulerAngles.y, rotationType.Evaluate(Time.time));
                    yield return null;
                }
            }
        }

    }

    void SetCurveLinear(AnimationCurve curve)
    {
        for (int i = 0; i < curve.keys.Length; ++i)
        {
            float intangent = 0;
            float outtangent = 0;
            bool intangent_set = false;
            bool outtangent_set = false;
            Vector2 point1;
            Vector2 point2;
            Vector2 deltapoint;
            Keyframe key = curve[i];

            if (i == 0)
            {
                intangent = 0; intangent_set = true;
            }

            if (i == curve.keys.Length - 1)
            {
                outtangent = 0; outtangent_set = true;
            }

            if (!intangent_set)
            {
                point1.x = curve.keys[i - 1].time;
                point1.y = curve.keys[i - 1].value;
                point2.x = curve.keys[i].time;
                point2.y = curve.keys[i].value;

                deltapoint = point2 - point1;

                intangent = deltapoint.y / deltapoint.x;
            }
            if (!outtangent_set)
            {
                point1.x = curve.keys[i].time;
                point1.y = curve.keys[i].value;
                point2.x = curve.keys[i + 1].time;
                point2.y = curve.keys[i + 1].value;

                deltapoint = point2 - point1;

                outtangent = deltapoint.y / deltapoint.x;
            }

            key.inTangent = intangent;
            key.outTangent = outtangent;
            curve.MoveKey(i, key);
        }
    }

    #endregion

    #region Related to Gameplay

    enum Stages
    {
        Stage_1,
        Stage_2,
        Stage_3,
        Stage_4,
        Bonus,
        Max
    }

    enum Difficulty
    {
        Easy,
        EasyMedium,
        Medium,
        MediumHard,
        Hard,
        EasyHard,
        Bonus, //TODO: think rotation
        Max
    }

    Difficulty GetCurrentDifficulty(Stages stage, int level)
    {
        Difficulty difficulty = Difficulty.Max;

        if (level == 0)
        {
            switch (stage)
            {
                case Stages.Stage_1:
                    difficulty = Difficulty.Easy;
                    stageCount = 0;
                    return difficulty;

                case Stages.Stage_2:
                    difficulty = Difficulty.Easy;
                    stageCount = 1;
                    return difficulty;

                case Stages.Stage_3:
                    difficulty = Difficulty.EasyMedium;
                    stageCount = 2;
                    return difficulty;

                case Stages.Stage_4:
                    difficulty = Difficulty.EasyMedium;
                    stageCount = 3;
                    return difficulty;

                case Stages.Bonus:
                    difficulty = Difficulty.Bonus;
                    stageCount = 4;
                    return difficulty;

                case Stages.Max:
                    break;

                default:
                    break;
            }
        }
        else if (level == 1)
        {
            switch (stage)
            {
                case Stages.Stage_1:
                    difficulty = Difficulty.Easy;
                    stageCount = 0;
                    break;

                case Stages.Stage_2:
                    difficulty = Difficulty.Easy;
                    stageCount = 1;
                    break;

                case Stages.Stage_3:
                    difficulty = Difficulty.EasyMedium;
                    stageCount = 2;
                    break;

                case Stages.Stage_4:
                    difficulty = Difficulty.EasyMedium;
                    stageCount = 3;
                    break;

                case Stages.Bonus:
                    difficulty = Difficulty.Bonus;
                    stageCount = 4;
                    break;

                case Stages.Max:
                    break;

                default:
                    break;
            }
        }
        else if (level == 2)
        {
            switch (stage)
            {
                case Stages.Stage_1:
                    difficulty = Difficulty.Easy;
                    stageCount = 0;
                    break;

                case Stages.Stage_2:
                    difficulty = Difficulty.EasyMedium;
                    stageCount = 1;
                    break;

                case Stages.Stage_3:
                    difficulty = RandomSelection(Difficulty.EasyMedium, Difficulty.EasyHard);
                    stageCount = 2;
                    break;

                case Stages.Stage_4:
                    difficulty = Difficulty.EasyHard;
                    stageCount = 3;
                    break;

                case Stages.Bonus:
                    difficulty = Difficulty.Bonus;
                    stageCount = 4;
                    break;

                case Stages.Max:
                    break;

                default:
                    break;
            }
        }
        else if (level == 3)
        {
            switch (stage)
            {
                case Stages.Stage_1:
                    difficulty = RandomSelection(Difficulty.EasyMedium, Difficulty.Easy); ;
                    stageCount = 0;
                    break;

                case Stages.Stage_2:
                    difficulty = RandomSelection(Difficulty.EasyMedium, Difficulty.EasyHard);
                    stageCount = 1;
                    break;

                case Stages.Stage_3:
                    difficulty = RandomSelection(Difficulty.EasyMedium, Difficulty.EasyHard);
                    stageCount = 2;
                    break;

                case Stages.Stage_4:
                    difficulty = Difficulty.Medium;
                    stageCount = 3;
                    break;

                case Stages.Bonus:
                    difficulty = Difficulty.Bonus;
                    stageCount = 4;
                    break;

                case Stages.Max:
                    break;

                default:
                    break;
            }
        }
        else if (level == 4)
        {
            switch (stage)
            {
                case Stages.Stage_1:
                    difficulty = RandomSelection(Difficulty.Easy, Difficulty.EasyMedium);
                    stageCount = 0;
                    break;

                case Stages.Stage_2:
                    difficulty = RandomSelection(Difficulty.EasyMedium, Difficulty.EasyHard);
                    stageCount = 1;
                    break;

                case Stages.Stage_3:
                    difficulty = RandomSelection(Difficulty.Medium, Difficulty.EasyHard);
                    stageCount = 2;
                    break;

                case Stages.Stage_4:
                    difficulty = Difficulty.Hard;
                    stageCount = 3;
                    break;

                case Stages.Bonus:
                    difficulty = Difficulty.Bonus;
                    stageCount = 4;
                    break;

                case Stages.Max:
                    break;

                default:
                    break;
            }
        }
        else if (level == 5)
        {
            switch (stage)
            {
                case Stages.Stage_1:
                    difficulty = RandomSelection(Difficulty.EasyMedium, Difficulty.Easy);
                    stageCount = 0;
                    break;

                case Stages.Stage_2:
                    difficulty = RandomSelection(Difficulty.Medium, Difficulty.EasyHard);
                    stageCount = 1;
                    break;

                case Stages.Stage_3:
                    difficulty = Difficulty.Medium;
                    stageCount = 2;
                    break;

                case Stages.Stage_4:
                    difficulty = Difficulty.Hard;
                    stageCount = 3;
                    break;

                case Stages.Bonus:
                    difficulty = Difficulty.Bonus;
                    stageCount = 4;
                    break;

                case Stages.Max:
                    break;

                default:
                    break;
            }
        }
        return difficulty;
    }

    void StartGame()
    {
        NextTurn(Stages.Stage_1);
    }

    void NextTurn(Stages stage)
    {
        currentStage = stage;
        currentDifficulty = GetCurrentDifficulty(currentStage, currentLevel);
        print("currentDifficulty" + currentDifficulty);
        int knives = GetKnivesToThrow(currentDifficulty);
        // testing

        knivesLeft = knives - 1;
        SpawnKnivesToThrow(knives);
        UIKnifeIndicator(knives);
        SpawnBoard();
        if (currentStage == Stages.Bonus)
        {

            foreach (Image sprite in StageDots)
            {
                sprite.gameObject.SetActive(false);
            }
            StageDots[stageCount].gameObject.SetActive(true);
            StageDots[stageCount].sprite = ActiveBonusDots;
            RectTransform rectTransform = StageDots[stageCount].gameObject.GetComponent<RectTransform>();
            StartCoroutine(MoveObject(rectTransform, new Vector3(0f, -46f),700f,false));
            stageText.text = "Bonus Stage";
            if (currentLevel != 5)
            {
                currentLevel += 1;
            }
        }
        else
        {
            StageDots[stageCount].sprite = ActiveStageDot;
            stageText.text = "Stage " + (stageCount + 1).ToString();
        }


    }

    IEnumerator MoveObject(RectTransform MovingObject, Vector2 desiredPos,float speed,bool destroyObject)
    {
        
            while (MovingObject.anchoredPosition != desiredPos)
            {
              
                    {
                    MovingObject.anchoredPosition = Vector3.MoveTowards(MovingObject.anchoredPosition, desiredPos, speed * Time.deltaTime);
                    yield return new WaitForSeconds(Time.deltaTime);
                }
            }

        if (destroyObject)
             {
            Destroy(MovingObject.gameObject);
             }
    }

    Difficulty RandomSelection(Difficulty diffOne, Difficulty diffTwo)
    {
        Difficulty difficulty;
        int value = Random.Range(0, 2);
        if (value == 1)
        {
            difficulty = diffOne;
        }
        else
        {
            difficulty = diffTwo;
        }
        return difficulty;

    }


    void UIKnifeIndicator(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject UIKnife = Instantiate(knifeUIObject, knifeUIParent.transform);
            UIKnife.GetComponent<RectTransform>().anchoredPosition = new Vector2(UIKnife.transform.position.x, UIKnife.transform.position.y + (i * 35));
            UIKnives.Add(UIKnife);

        }

    }


    void KnifeUIUpdater()
    {
        Color colour = UIKnives[knivesLeft].GetComponent<Image>().color;
        colour.a = .42f;
        UIKnives[knivesLeft].GetComponent<Image>().color = colour;
    }



    void SpawnBoard()
    {
        GetBoardWithKnivesAndCollectibles(currentDifficulty);
        if (currentDifficulty == Difficulty.Bonus)
        {
            brokenboardIndex = Random.Range(0, bonusBoardSkins.Length);
            currentBoard.GetComponent<SpriteRenderer>().sprite = bonusBoardSkins[brokenboardIndex];
        }
        else
        {
            brokenboardIndex = Random.Range(0, boardSkins.Length);
            currentBoard.GetComponent<SpriteRenderer>().sprite = boardSkins[brokenboardIndex];
        }
        SetRotationOfBoard(currentDifficulty);
        currentBoardSprite = currentBoard.GetComponent<SpriteRenderer>();
    }

    void ButtonAddListeners()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
        restartButton.onClick.AddListener(OnRestartButtonClicked);
        homeButton.onClick.AddListener(OnHomeButtonClicked);
        tutoralButton.onClick.AddListener(TutorialButtonClicked);
        infoButton.onClick.AddListener(InfoButtonClicked);
        volumeButton.onClick.AddListener(VolumeButtonClicked);
        resumeButton.onClick.AddListener(ResumeGame);
        homeFromPauseButton.onClick.AddListener(HomeFromPauseScreen);

    }


    void InfoButtonClicked()
    {
        buttonSFX.Play();
        tutorialPanel.SetActive(true);

    }

    void VolumeButtonClicked()
    {
        buttonSFX.Play();
        if (volume)
        {
            volume = false;
            volumeImage.sprite = VolumeOFF;
            for (int i = 0; i < audioSources.Count; i++)
            {
                audioSources[i].mute = true;
            }
        }
        else
        {
            volume = true;
            volumeImage.sprite = VolumeOn;
            for (int i = 0; i < audioSources.Count; i++)
            {
                audioSources[i].mute = false;
            }
        }
    }





    void TutorialButtonClicked()
    {
        tutorialPanel.SetActive(false);
    }


    void OnPlayButtonClicked()
    {
        buttonSFX.Play();
        homeScreenPanel.SetActive(false);
        gameStarted = true;
    }

    void OnRestartButtonClicked()
    {
       // print("called");
        buttonSFX.Play();
        StartCoroutine(RestartButtonDelay());
        gameOverPanel.SetActive(false);
        isGameOver = false;
        StartGame();
    }

    IEnumerator RestartButtonDelay()
    {
        yield return new WaitForSeconds(.5f);
        
        gameStarted = true;
        canThrow = true;
        
    }

        void OnHomeButtonClicked()
    {
        buttonSFX.Play();
        //  OnRestartButtonClicked();
        isGameOver = false;
        canThrow = true;
        StartGame();
        gameOverPanel.SetActive(false);
        homeScreenPanel.SetActive(true);
        gameStarted = false;

    }



    void SetRotationOfBoard(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                RotationEasy rotationE = (RotationEasy)Random.Range(0, (int)RotationEasy.Max);
                EasyRotation(rotationE);
                break;

            case Difficulty.EasyMedium:
                RotationEasyMedium rotationEM = (RotationEasyMedium)Random.Range(0, (int)RotationEasyMedium.Max);
                EasyMediumRotation(rotationEM);
                break;

            case Difficulty.Medium:
                RotationMedium rotationM = (RotationMedium)Random.Range(0, (int)RotationMedium.Max);
                MediumRotaion(rotationM);
                break;

            case Difficulty.MediumHard:
                RotationMediumHard rotationMH = (RotationMediumHard)Random.Range(0, (int)RotationMediumHard.Max);
                MediumHardRotation(rotationMH);
                break;

            case Difficulty.Hard:
                RotationHard rotationH = (RotationHard)Random.Range(0, (int)RotationHard.Max);
                HardRotation(rotationH);
                break;

            case Difficulty.EasyHard:
                RotationEasyHard rotationEH = (RotationEasyHard)Random.Range(0, (int)RotationEasyHard.Max);
                EasyHardRotation(rotationEH);
                break;

            case Difficulty.Bonus:
                //Test easy rotTION
                RotationEasy rotationB = RotationEasy.Continuous_1;
                EasyRotation(rotationB);
                break;

            default:
                break;
        }
    }

    void GetBoardWithKnivesAndCollectibles(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                currentBoard = Instantiate(easyBoards[Random.Range(0, easyBoards.Length)]);
                break;

            case Difficulty.EasyMedium:
                currentBoard = Instantiate(easyMediumBoards[Random.Range(0, easyMediumBoards.Length)]);
                break;

            case Difficulty.Medium:
                currentBoard = Instantiate(mediumBoards[Random.Range(0, mediumBoards.Length)]);
                break;

            case Difficulty.MediumHard:
                currentBoard = Instantiate(mediumHardBoards[Random.Range(0, mediumHardBoards.Length)]);
                break;

            case Difficulty.Hard:
                currentBoard = Instantiate(hardBoards[Random.Range(0, hardBoards.Length)]);
                break;

            case Difficulty.EasyHard:
                currentBoard = Instantiate(easyHardBoards[Random.Range(0, easyHardBoards.Length)]);
                break;

            case Difficulty.Bonus:
                currentBoard = Instantiate(bonusBoard);
                break;

            default:
                break;
        }
    }

    int GetKnivesToThrow(Difficulty difficulty)
    {
        int randomValue = 0;
        switch (difficulty)
        {
            case Difficulty.Easy:
                print(numOfKnives.Length);
                randomValue = (int)Random.Range(numOfKnives[0].x, numOfKnives[0].y);
                print("randomvalue  "+randomValue);
                return randomValue;

            case Difficulty.EasyMedium:
                randomValue = (int)Random.Range(numOfKnives[1].x, numOfKnives[1].y);
                return randomValue;

            case Difficulty.Medium:
                randomValue = (int)Random.Range(numOfKnives[2].x, numOfKnives[2].y);
                return randomValue;

            case Difficulty.MediumHard:
                randomValue = (int)Random.Range(numOfKnives[3].x, numOfKnives[3].y);
                return randomValue;

            case Difficulty.Hard:
                randomValue = (int)Random.Range(numOfKnives[4].x, numOfKnives[4].y);
                return randomValue;

            case Difficulty.EasyHard:
                randomValue = (int)Random.Range(numOfKnives[5].x, numOfKnives[5].y);
                return randomValue;
                break;

            case Difficulty.Bonus:
                
                randomValue = 12;//number of jellys in bonus stage
                return randomValue;
                break;

            default:
                return -1;
                break;
        }

        return -1;
    }

    void SpawnKnivesToThrow(int numOfKnives)
    {
        //     knivesToThrowText.text = "Knives to Throw: " + numOfKnives.ToString();
        knifeObjects = new List<GameObject>(numOfKnives);
        knives = new List<GameObject>(numOfKnives);
        if (knifeParent == null)
        {
            knifeParent = new GameObject("Knife Parent");
        }

        for (int i = 0; i < numOfKnives; i++)
        {
            GameObject knife = Instantiate(knifePrefab, knifeParent.transform);
            knifeObjects.Add(knife);
            knives.Add(knife);
        }

        for (int i = 0; i < knifeObjects.Count; i++)
        {

            if (i == knifeObjects.Count - 1)
            {
                StartCoroutine(coKnifeFadeIn(knifeObjects[i]));
            }
            else
            {
                knifeObjects[i].SetActive(false);
            }
        }
    }

    IEnumerator coKnifeFadeIn(GameObject knife)
    {
        Color fullApha = new Color(1, 1, 1, 1);
        while (knife.transform.position != knifeInitialPos)
        {
            knife.transform.position = Vector3.MoveTowards(knife.transform.position, knifeInitialPos, knifeFadeLerpSpeed * Time.deltaTime);
            knife.GetComponent<SpriteRenderer>().color = Color.Lerp(knife.GetComponent<SpriteRenderer>().color, fullApha, knifeFadeLerpSpeed * Time.deltaTime);
            yield return null;
        }
        knife.GetComponent<SpriteRenderer>().color = fullApha;
        if (!isGameOver)
        {
            canThrow = true;
        }
    }


    IEnumerator ThrowKnife(GameObject currentKnife, bool lastKnife)
    {

        while (currentKnife.transform.position != knifeThrownPos)
        {

            currentKnife.transform.position = Vector3.MoveTowards(currentKnife.transform.position, knifeThrownPos, knifeThrowSpeed * Time.deltaTime);
            yield return new WaitForSeconds((knifeThrowSpeed * Time.deltaTime) / knifeThrowSpeed);
        }

        knifeHitBoardSFX.Play();
        currentKnife.transform.parent = currentBoard.transform;
        if (lastKnife)
        {
            BoardBreakDelay();
        }
    }


    void BoardBreakDelay()
    {
        if (!isGameOver)
        {
            for (int i = 0; i < knives.Count; i++)
            {
                knives[i].transform.parent = null;
            }
            GameObject temp = currentBoard;
            currentBoard = null;


            for (int i = 0; i < knives.Count; i++)
            {

                knives[i].GetComponent<KnifeForce>().enabled = true;
                Destroy(knives[i], 1f);
                boardBreakSFX.Play();
            }

            for (int i = 0; i < UIKnives.Count; i++)
            {
                Destroy(UIKnives[i]);
            }
            knivesLeft = 0;
            UIKnives.Clear();
            GameObject tempBoard;
            if (currentDifficulty == Difficulty.Bonus)
            {
                tempBoard = Instantiate(bonusBrokenBoards[brokenboardIndex], new Vector3(-0.5f, 1.7f, 0f), Quaternion.identity);
            }
            else
            {
                tempBoard = Instantiate(brokenBoards[brokenboardIndex], new Vector3(-0.5f, 1.7f, 0f), Quaternion.identity);
            }
            StartCoroutine(coDestroyBoard(temp));
            Destroy(tempBoard, 1f);


        }

    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        if (currentBoard != null)
        {
            yield return new WaitForSeconds(.1f);
            Vector3 orignalPosition = currentBoard.transform.position;
            float elapsed = 0f;
           // Color color = currentBoardSprite.color;
          //  color.a = .95f;
         //   currentBoardSprite.color = color;
            while (elapsed < duration)
            {
                float x = 0f;
                float y = Random.Range(1f, 1.07f) * magnitude;
                if (currentBoard != null)
                {
                    currentBoard.transform.position = new Vector3(x, y, 0f);
                }
                else
                {
                    yield break;
                }
                elapsed += Time.deltaTime;
                yield return 0;
            }
            if (currentBoard != null)
            {

                currentBoard.transform.position = orignalPosition;
              //  color.a = 1f;
              //  currentBoardSprite.color = color;
            }
        }
    }

    IEnumerator coDestroyBoard(GameObject board)
    {
        Destroy(board);
        GameObject clone = Instantiate(boardDestroyVFX);
        yield return new WaitForSeconds(1f);
        Destroy(clone);
        currentStage++;
        if (currentStage == Stages.Max)
        {
            currentStage = Stages.Stage_1;
            StageDotsUI();
        }
        NextTurn(currentStage);
    }

    public void UpdateScore(int value)
    {
        score += value;
        scoreText.text = score.ToString();
    }

    public void GameOverDelay()
    {
        StartCoroutine(GameOver());



    }

  //  GameObject plusFifties;
  List<GameObject> plusFifties=new List<GameObject>();

    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(.25f);
        StopAllCoroutines();
        gameStarted = false;
        foreach (GameObject plusFiftyUnit in plusFifties)
        {
            if (plusFiftyUnit != null)
            {
                Destroy(plusFiftyUnit);
            }
        }
        plusFifties = new List<GameObject>();
        gameOverPanel.SetActive(true);
        Camera.main.transform.position = new Vector3(0f, 0f, -10f);
        Destroy(currentBoard);
        for (int i = 0; i < knifeObjects.Count; i++)
        {
            Destroy(knifeObjects[i]);

        }
        for (int i = 0; i < UIKnives.Count; i++)
        {
            Destroy(UIKnives[i]);
        }

        knivesLeft = 0;
        UIKnives.Clear();
        isAttacking = false;
        score = 0;
        UpdateScore(0);
        StageDotsUI();
        HandleDeathVideoAd();
    }

    private void StageDotsUI()
    {

        for (int j = 0; j < StageDots.Length; j++)
        {
            StageDots[j].gameObject.SetActive(true);
            if (j < StageDots.Length - 1)
            {
                StageDots[j].sprite = InActiveStageDots;
            }
            else
            {
                StageDots[j].sprite = InActiveBonusDots;
                StopAllCoroutines();
                StageDots[j].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(100f, -38.7f);

            }

        }
    }

    public IEnumerator CameraShake()
    {
        StopCoroutine(Shake(.1f, shakeMagnitude));
        Vector3 orignalPosition = Camera.main.transform.position;
        float elapsed = 0f;
        float duration = .1f;
        while (elapsed < duration)
        {
            float x = Random.Range(-.5f, .25f) * shakeMagnitude;
            float y = Random.Range(0f, .25f) * shakeMagnitude;
            Camera.main.transform.position = new Vector3(x, y, Camera.main.transform.position.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Camera.main.transform.position = orignalPosition;
        GameOverDelay();
    }

    public void redPanelEnabler()
    {
        StartCoroutine(RedPanelEnabler());

    }


    IEnumerator RedPanelEnabler()
    {

        knifeHitBoardSFX.Stop();
        knifeHitKnifeSFX.Play();
        redPanel.SetActive(true);
        StartCoroutine(CameraShake());
        yield return new WaitForSeconds(.15f);
        redPanel.SetActive(false);
        StartCoroutine(GameOver());

    }

    public void UIEnabler(Vector2 vector2)
    {
        StartCoroutine(plusFiftyEnabler(vector2));
    }

        IEnumerator plusFiftyEnabler(Vector2 Pos)
    {
        GameObject plusFiftyClone = Instantiate(plusFifty,canvas.transform);
        plusFifties.Add(plusFiftyClone);
        //Destroy(plusFiftyClone, .65f);
        TextMeshProUGUI plusFiftyTextClone = plusFiftyClone.GetComponent<TextMeshProUGUI>();
        plusFiftyTextClone.materialForRendering.shader = Shader.Find("TextMeshPro/Distance Field"); 
        plusFiftyClone.SetActive(true);
         RectTransform newRect= plusFiftyClone.GetComponent<RectTransform>();
        newRect.anchoredPosition = Pos;
        while (plusFiftyTextClone.fontSize < 60)

        {
            plusFiftyTextClone.fontSize += 5;
            yield return null;
        }
        yield return new WaitForSeconds(.025f);
        StartCoroutine(MoveObject(newRect, plusFiftyFinalPos, 2500f,true));
       // yield return new WaitForSeconds(.5f);
     //   Destroy(plusFiftyTextClone, .3f);
      //  plusFifty.SetActive(false);
        //  plusFiftyText.fontSize = 55;
        //  yield return new WaitForEndOfFrame();
        //  plusFiftyText.fontSize = 60


    }
  
}
#endregion
#endregion

