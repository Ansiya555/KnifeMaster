using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Reflection;
using System;
using Strobotnik.GUA;

public class GameDownloader : MonoBehaviour
{
    public bool forceLoadNewScriptAssetBundle = false;
    public bool showDebug = true;

    public enum StorePlatform
    {
        GooglePlayStore,
        PaytmFirstGamesStore
    }
    [Header("Target Store Platform")]
    public StorePlatform storePlatform;

    [Header("Game string and URL")]
    public string devGameName;
    public string gameMainAssetUrl = "";

    [Header("Game Prefab")]
    public GameObject GamePrefab;
    [HideInInspector]
    public bool useLocalGamePrefab;

    [Header("Loading Canvas Prefab")]
    public GameObject LoadingCanvasPrefab;
    public GameObject EventSystemPrefab;


    public static GameDownloader instance = null;
    GameObject loadingCanvasObject;
    CanvasGroup loadingCanvasGroup;

    Image loadingBarFillAmountImage;


    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SetupGameDownloaderValues()
    {// Fill main json url and devGameName for paytm at runtime and then call StartLoadingGame()
        StartLoadingGame();
    }

    GameObject eventSystemObject;
    public void Start()
    {
        SetupGameDownloaderValues();
    }

    public void StartLoadingGame()
    {
        if (isTargetGooglePlayStore())
        {
            useLocalGamePrefab = true;
        }
        else if (isTargetPaytmFirstGamesStore())
        {
            useLocalGamePrefab = false;
        }

        this.StopAllCoroutines();
        if (loadingCanvasObject == null)
        {
            loadingCanvasObject = Instantiate(LoadingCanvasPrefab, this.transform);
            eventSystemObject = Instantiate(EventSystemPrefab, this.transform);
            loadingCanvasObject.transform.localPosition = Vector3.zero;
            loadingBarFillAmountImage = loadingCanvasObject.transform.GetChild(1).transform.GetChild(0).transform.GetChild(1).GetComponent<Image>();
            loadingCanvasGroup = loadingCanvasObject.GetComponent<CanvasGroup>();
        }
        if (loadingCanvasGroup != null)
        {
            ShowLoadingCanvas();
        }

        if (GamePrefab == null)
        {
            if (showDebug)
            {
                print("GamePrefab is null");
            }
            useLocalGamePrefab = false;
        }

        SetTargetLoadingBarImageAmount(0);
        StartCoroutine(FillLoadingBarImageTo(loadingBarFillAmountImage, 5));

        StartCoroutine(CheckForInternetConnection());
    }

    IEnumerator CheckForInternetConnection()
    {
        if (isTargetGooglePlayStore())
        {
            /*const string echoServer = "http://google.com";

            bool result = false;
            using (var request = UnityWebRequest.Head(echoServer))
            {
                request.timeout = 5;
                yield return request.SendWebRequest();
                result = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;
            }
            isInternetConnectionAvailable = result;*/

            if (useLocalGamePrefab)
            {
                isInternetConnectionAvailable = true;
                yield return new WaitForEndOfFrame();
                StartCoroutine(LoadMainJsonData(gameMainAssetUrl));
                //SetupLocalGamePrefab();
            }
        }
        else
        {
            isInternetConnectionAvailable = true;
            DownloadMainJsonData();
        }
    }

    public void DownloadMainJsonData()
    {
        StartCoroutine(LoadAssetBundleAndDLL(gameMainAssetUrl));
    }

    public void ShowLoadingCanvas()
    {
        loadingCanvasGroup.alpha = 0f;
        StartCoroutine(FadeCanvasGroupAndToggleObject(loadingCanvasGroup, 1, 0, null, true, 5));
    }

    public void HideLoadingCanvas()
    {
        StartCoroutine(FadeCanvasGroupAndToggleObject(loadingCanvasGroup, 0, 0, loadingCanvasObject, false, 5));
    }

    public void SetTargetLoadingBarImageAmount(float val)
    {
        PlayerPrefs.SetFloat(devGameName + loadingBarAmountPrefsText, val);
        loadingBarFillTargetAmount = val;
    }

    public IEnumerator FadeCanvasGroupAndToggleObject(CanvasGroup cg, float targetAlpha, float delay, GameObject obj, bool target, float fadeSpeed)
    {
        yield return new WaitForSeconds(delay);
        while (cg.alpha != targetAlpha)
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }
        if (obj != null)
        {
            obj.SetActive(target);
        }
        yield return null;
    }

    string loadingBarAmountPrefsText = "LoadingBarFillAmount";
    float loadingBarFillTargetAmount;
    public IEnumerator FillLoadingBarImageTo(Image img, float speed)
    {
        float checkTime = 0f;
        bool hideCanvasStarted = false;
        if (img != null)
        {
            while (img.enabled)
            {
                checkTime += Time.deltaTime;
                if (checkTime > 1f && hideCanvasStarted == false)
                {
                    img.fillAmount = PlayerPrefs.GetFloat(devGameName + loadingBarAmountPrefsText);
                    loadingBarFillTargetAmount = img.fillAmount;
                    checkTime = 0f;
                }
                img.fillAmount = Mathf.MoveTowards(img.fillAmount, loadingBarFillTargetAmount, speed * Time.deltaTime);
                if (img.fillAmount >= 1 && hideCanvasStarted == false)
                {
                    HideLoadingCanvas();
                    hideCanvasStarted = true;
                }
                yield return null;
            }
        }
    }

    public SimpleJSON.JSONNode mainJsonData = null;
    WWW localAssetBundleDownload;

    [HideInInspector]
    public bool isInternetConnectionAvailable = false;

    public AssetBundle assetBundle = null;

    public AssetBundle GetMainAssetBundle()
    {
        if (assetBundle == null)
        {
            if (showDebug)
                print("main assetBundle is null ");
        }
        return assetBundle;

    }
    string assetBundleURL;
    int newAssetBundleVersion;

    string scriptURL;
    int newScriptDLLVersion;

    bool loadGameFromLocalStoredAssetBundle = false;
    string folderPath;
    string gameFolderName;
    string assetBundleFileName;
    string scriptFileName;
    string assetBundleFilePath;
    string scriptFilePath;

    IEnumerator LoadAssetBundleAndDLL(string url)
    {
        int assetBundleVersion = 0;
        int scriptDLLVersion = 0;

        gameFolderName = devGameName + "Data";
        folderPath = Application.persistentDataPath + "/" + gameFolderName;

        assetBundleFileName = devGameName + "AssetBundleData";
        assetBundleFilePath = folderPath + "/" + assetBundleFileName;
        PlayerPrefs.SetString(devGameName + "assetBundleFilePath", assetBundleFilePath);

        scriptFileName = devGameName + "scriptData";
        scriptFilePath = folderPath + "/" + scriptFileName;
        PlayerPrefs.SetString(devGameName + "scriptFilePath", scriptFilePath);
        assembly = null;

        if (showDebug)
            print("isInternetConnectionAvailable " + isInternetConnectionAvailable);

        if (isInternetConnectionAvailable)
        {
            WWW www = new WWW(url);
            yield return www;
            if (www.error == null)
            {
                if (showDebug)
                    print(www.text);

                mainJsonData = SimpleJSON.JSON.Parse(www.text);

                PlayerPrefs.SetString(devGameName + "mainJsonDataText", www.text);

                if (showDebug)
                    print(mainJsonData);

                SetTargetLoadingBarImageAmount(0.25f);

                devGameName = mainJsonData["mainData"]["devGameName"];

                #region PlayerPrefsReset
                if (forceLoadNewScriptAssetBundle)
                {
                    PlayerPrefs.SetInt(devGameName + "scriptDLLVersion", 0);

                    PlayerPrefs.SetInt(devGameName + "assetBundleVersion", 0);
                }
                #endregion

                assetBundleURL = mainJsonData["mainData"]["assetbundleUrl"];

                assetBundleVersion = PlayerPrefs.GetInt(devGameName + "assetBundleVersion");
                newAssetBundleVersion = mainJsonData["mainData"]["assetbundleversion"];

                #region Related to AssetBundle                

                bool isNewAssetBundleVersionAvailable = false;
                if (assetBundleVersion < newAssetBundleVersion) { isNewAssetBundleVersionAvailable = true; }

                if (showDebug)
                    print("isNewAssetBundleVersionAvailable " + isNewAssetBundleVersionAvailable);

                if (isNewAssetBundleVersionAvailable == false)
                {
                    if (File.Exists(assetBundleFilePath))
                    {
                        assetBundle = AssetBundle.LoadFromFile(assetBundleFilePath);
                    }

                    if (showDebug)
                        print(assetBundle);

                    if (assetBundle == null)
                    {
                        isNewAssetBundleVersionAvailable = true;
                    }
                    else if (assetBundle.Contains(devGameName + "Game") == false)
                    {
                        loadGameFromLocalStoredAssetBundle = true;
                    }
                }

                if (isNewAssetBundleVersionAvailable)
                {
                    if (showDebug)
                        print("downloading new asset bundle " + newAssetBundleVersion);

                    yield return StartCoroutine(LoadAssetBundle());
                }

                #endregion


                #region Related to script DLL Only For Paytm Store
                if (isTargetPaytmFirstGamesStore())
                {
                    scriptURL = mainJsonData["mainData"]["scriptDLLUrl"];

                    scriptDLLVersion = PlayerPrefs.GetInt(devGameName + "scriptDLLVersion");
                    newScriptDLLVersion = mainJsonData["mainData"]["scriptDLLVersion"];
                    bool isNewScriptDLLAvailable = false;
                    if (scriptDLLVersion < newScriptDLLVersion) { isNewScriptDLLAvailable = true; }

                    if (isNewScriptDLLAvailable == false)
                    {
                        if (File.Exists(scriptFilePath))
                        {
                            assembly = System.Reflection.Assembly.LoadFrom(scriptFilePath);
                        }

                        if (assembly == null)
                        {
                            isNewScriptDLLAvailable = true;
                        }
                    }

                    if (isNewScriptDLLAvailable)
                    {
                        if (showDebug)
                            print("downloading new script DLL " + newScriptDLLVersion);

                        yield return StartCoroutine(LoadScript());
                        PlayerPrefs.SetInt(devGameName + "scriptDLLVersion", newScriptDLLVersion);
                    }
                }
                #endregion
            }
            else
            {
                loadGameFromLocalStoredAssetBundle = true;
                if (showDebug)
                {
                    Debug.LogError("url");
                    Debug.LogError(url);
                    Debug.LogError("Could not load main json data");
                }
            }
        }
        else
        {
            loadGameFromLocalStoredAssetBundle = true;
        }

        if (isTargetPaytmFirstGamesStore())
        {
            if (assetBundle != null)
            {
                assetBundle.Unload(false);
            }
            GameObject tempObject = new GameObject(devGameName);
            assembly = Assembly.LoadFrom(scriptFilePath);
            Type mainScript = assembly.GetType(devGameName);
            tempObject.AddComponent(mainScript);
        }

        if (loadGameFromLocalStoredAssetBundle)
        {
            if (showDebug)
                print("checking loading locally stored asset bundle");

            if (File.Exists(assetBundleFilePath))
            {
                if (assetBundle == null)
                {
                    assetBundle = AssetBundle.LoadFromFile(assetBundleFilePath);
                }
            }

            if (showDebug)
                print(assetBundle);

            if (assetBundle != null)
            {
                if (assetBundle.Contains(devGameName + "Game"))
                {
                    //var loadAsset = (assetBundle.LoadAssetAsync<GameObject>(devGameName + "Game"));
                    //yield return loadAsset;
                    //Instantiate(loadAsset.asset);
                    SetTargetLoadingBarImageAmount(0.75f);
                }
                else
                {
                    useLocalGamePrefab = true;
                }
            }
            else
            {
                useLocalGamePrefab = true;
            }
        }

        if (useLocalGamePrefab)
        {
            SetTargetLoadingBarImageAmount(0.75f);
            yield return new WaitForSeconds(1f);
            SetupLocalGamePrefab();
        }
    }

    IEnumerator LoadMainJsonData(string url)
    {
        gameFolderName = devGameName + "Data";
        folderPath = Application.persistentDataPath + "/" + gameFolderName;

        if (showDebug)
            print("isInternetConnectionAvailable " + isInternetConnectionAvailable);

        if (isInternetConnectionAvailable)
        {
            WWW www = new WWW(url);
            yield return www;
            if (www.error == null)
            {
                if (showDebug)
                    print(www.text);

                mainJsonData = SimpleJSON.JSON.Parse(www.text);

                PlayerPrefs.SetString(devGameName + "mainJsonDataText", www.text);

                if (showDebug)
                    print(mainJsonData);

                SetTargetLoadingBarImageAmount(0.25f);

                devGameName = mainJsonData["mainData"]["devGameName"];
            }
            else
            {
                loadGameFromLocalStoredAssetBundle = true;
                if (showDebug)
                {
                    Debug.LogError("url");
                    Debug.LogError(url);
                    Debug.LogError("Could not load main json data");
                }
            }
        }
        else
        {
            loadGameFromLocalStoredAssetBundle = true;
        }

        if (loadGameFromLocalStoredAssetBundle)
        {
            if (showDebug)
                print("checking loading locally stored asset bundle");

            if (File.Exists(assetBundleFilePath))
            {
                if (assetBundle == null)
                {
                    assetBundle = AssetBundle.LoadFromFile(assetBundleFilePath);
                }
            }

            if (showDebug)
                print(assetBundle);

            if (assetBundle != null)
            {
                if (assetBundle.Contains(devGameName + "Game"))
                {
                    //var loadAsset = (assetBundle.LoadAssetAsync<GameObject>(devGameName + "Game"));
                    //yield return loadAsset;
                    //Instantiate(loadAsset.asset);
                    SetTargetLoadingBarImageAmount(0.75f);
                }
                else
                {
                    useLocalGamePrefab = true;
                }
            }
            else
            {
                useLocalGamePrefab = true;
            }
        }

        if (useLocalGamePrefab)
        {
            SetTargetLoadingBarImageAmount(0.75f);
            yield return new WaitForEndOfFrame();
            SetupLocalGamePrefab();
        }
    }

    Assembly assembly;
    IEnumerator LoadScript()
    {
        UnityWebRequest www = UnityWebRequest.Get(scriptURL);
        yield return www.SendWebRequest();

        DownloadHandler fileHandler = www.downloadHandler;
        print(fileHandler.data);
        byte[] bytes = fileHandler.data;
        print(bytes.Length);

        if (bytes.Length > 0)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            if (!File.Exists(scriptFilePath))
            {
                File.Create(scriptFilePath).Close();
            }

            File.WriteAllBytes(scriptFilePath, bytes);
            print(devGameName);
        }
    }

    IEnumerator LoadAssetBundle()
    {
        WWW bundleRequest = new WWW(assetBundleURL);
        yield return bundleRequest;
        if (bundleRequest.bytesDownloaded > 0)
        {
            AssetBundleCreateRequest myRequest = AssetBundle.LoadFromMemoryAsync(bundleRequest.bytes);
            yield return myRequest;
            AssetBundle bundle = myRequest.assetBundle;
            if (bundle.Contains(devGameName + "Game"))
            {
                assetBundle = bundle;
                var loadAsset = (bundle.LoadAssetAsync<GameObject>(devGameName + "Game"));
                yield return loadAsset;
                SetTargetLoadingBarImageAmount(0.75f);

                //Instantiate(loadAsset.asset);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                if (!File.Exists(assetBundleFilePath))
                {
                    File.Create(assetBundleFilePath).Close();
                }
                File.WriteAllBytes(assetBundleFilePath, bundleRequest.bytes);

                PlayerPrefs.SetInt(devGameName + "assetBundleVersion", newAssetBundleVersion);
                loadGameFromLocalStoredAssetBundle = false;
            }
            else
            {
                useLocalGamePrefab = true;
            }
        }
    }

    public void SetupLocalGamePrefab()
    {
        if (GamePrefab != null)
        {
            Instantiate(GamePrefab);
        }
        else if (showDebug)
        {
            print("GamePrefab is null");
        }
    }

    public bool isTargetGooglePlayStore()
    {
        return storePlatform == StorePlatform.GooglePlayStore;
    }

    public bool isTargetPaytmFirstGamesStore()
    {
        return storePlatform == StorePlatform.PaytmFirstGamesStore;
    }

    public SimpleJSON.JSONNode GetMainJsonData()
    {
        if (mainJsonData == null)
        {
            if (showDebug)
                Debug.LogError("mainJsonData is null");
        }
        return mainJsonData;
    }

    public void SetAsPaytmFirstGames()
    {
        storePlatform = StorePlatform.PaytmFirstGamesStore;
    }

    public void SetAsGooglePlayStore()
    {
        storePlatform = StorePlatform.GooglePlayStore;
    }
}
