using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Strobotnik.GUA;
using GoogleMobileAds.Api;

public class TapTapKnife : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] TextMeshProUGUI stageText;
    [SerializeField] TextMeshProUGUI scoreText;
  

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
    List <GameObject> UIKnives=new List<GameObject>();


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
     AudioSource[] audioSources;



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

    private void Awake()
    {
        instance = this;
        audioSources = GetComponentsInChildren<AudioSource>();
      
    }

    private void Start()
    {
        
        ButtonAddListeners();
        StartGame();
    }

    private void Update()
    {
      
       
        
        if (Input.GetMouseButtonDown(0) && canThrow && gameStarted && knivesLeft>=0)
        {

            canThrow = false;
            isAttacking = true;
            KnifeUIUpdater();          
            if (knivesLeft != 0)
            {
                StartCoroutine(Shake(boardShakeDuration, shakeMagnitude));
                StartCoroutine(ThrowKnife(knifeObjects[knivesLeft],false));            
                knivesLeft -= 1;
                knifeObjects[knivesLeft].SetActive(true);
                StartCoroutine(coKnifeFadeIn(knifeObjects[knivesLeft]));
            }
            else
            {
                StartCoroutine(ThrowKnife(knifeObjects[knivesLeft],true));
            }
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
        else if (level==2)
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
                    difficulty = RandomSelection(Difficulty.EasyMedium,Difficulty.EasyHard);
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
        else if (level ==3)
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
        int knives = GetKnivesToThrow(currentDifficulty);    
        knivesLeft = knives-1;
        SpawnKnivesToThrow(knives);
        UIKnifeIndicator(knives);
        SpawnBoard();
        if(currentStage==Stages.Bonus)
        {
           
            foreach(Image sprite in StageDots)
            {
                sprite.gameObject.SetActive(false);
            }
            StageDots[stageCount].gameObject.SetActive(true);
            StageDots[stageCount].sprite = ActiveBonusDots;
            RectTransform rectTransform= StageDots[stageCount].gameObject.GetComponent<RectTransform>();
            StartCoroutine(MoveObject(rectTransform, new Vector3(0f,-46f)));
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

    IEnumerator MoveObject(RectTransform MovingObject,Vector2 desiredPos)
    {
        while(MovingObject.anchoredPosition != desiredPos)
        {
            MovingObject.anchoredPosition = Vector3.MoveTowards(MovingObject.anchoredPosition, desiredPos, 700f*Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
       
    }

    Difficulty RandomSelection(Difficulty diffOne, Difficulty diffTwo)
    {
        Difficulty difficulty;
        int value=Random.Range(0, 2);
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
            GameObject UIKnife= Instantiate(knifeUIObject,knifeUIParent.transform);
            UIKnife.GetComponent<RectTransform>().anchoredPosition = new Vector2(UIKnife.transform.position.x, UIKnife.transform.position.y + (i * 35));
            UIKnives.Add(UIKnife);
            
        }

    }


    void KnifeUIUpdater()
    {    
        Color colour = UIKnives[knivesLeft].GetComponent<Image>().color;
        colour.a = .42f;     
        UIKnives[knivesLeft].GetComponent<Image>().color= colour;
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
            for(int i=0;i<audioSources.Length;i++)
            {
                audioSources[i].mute = true;
            }
        }
        else
        {
            volume = true;
            volumeImage.sprite = VolumeOn;
            for (int i = 0; i < audioSources.Length; i++)
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
        buttonSFX.Play();
        isGameOver = false;
        gameStarted = true;
        canThrow = true;      
        StartGame();
        gameOverPanel.SetActive(false);
    }

    void OnHomeButtonClicked()
    {
        buttonSFX.Play();
        OnRestartButtonClicked();
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
                randomValue = (int)Random.Range(numOfKnives[0].x, numOfKnives[0].y);
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
                randomValue = (int)Random.Range(numOfKnives[6].x, numOfKnives[6].y);
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
           
            if(i == knifeObjects.Count-1)
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
            boardBreakSFX.Play(1);
        }

        for (int i = 0; i < UIKnives.Count; i++)
        {
            Destroy(UIKnives[i]);
        }
        knivesLeft = 0;
        UIKnives.Clear();
        GameObject tempBoard;
       if (currentDifficulty==Difficulty.Bonus)
        {
             tempBoard = Instantiate(bonusBrokenBoards[brokenboardIndex], new Vector3(-1.15f,1f, 0f), Quaternion.identity);
        }
        else
        {
             tempBoard = Instantiate(brokenBoards[brokenboardIndex], new Vector3(-1.15f, 1f, 0f), Quaternion.identity);
        }
        StartCoroutine(coDestroyBoard(temp));


    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        if (currentBoard != null)
        {
            yield return new WaitForSeconds(.1f);
            Vector3 orignalPosition = currentBoard.transform.position;
            float elapsed = 0f;
            Color color = currentBoardSprite.color;
            color.a = .95f;
            currentBoardSprite.color = color;
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
                color.a = 1f;
                currentBoardSprite.color = color;
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
        if(currentStage==Stages.Max)
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


    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(.25f);
        StopAllCoroutines();
        gameStarted = false;
        gameOverPanel.SetActive(true);
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

  
    #endregion

}
