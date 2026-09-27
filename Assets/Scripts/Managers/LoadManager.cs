using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    public static LoadManager Instance { get; private set;}
    [SerializeField]private GameObject loadUi;
    [SerializeField]private CanvasGroup loadUiGroup;

    void Awake()
    {
       if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void StartLoad(int levelIndex)
    {
        StartCoroutine(LoadRoutine(levelIndex));
    }

    private IEnumerator LoadRoutine(int levelIndex)
    {
        loadUi.SetActive(true);
        loadUiGroup.alpha = 1.0f;
        loadUiGroup.blocksRaycasts = true;

        //Application.backgroundLoadingPriority = ThreadPriority.Low;
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelIndex);

        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            yield return null; 
        }

        yield return new WaitForSecondsRealtime(0.2f);
        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            yield return null;
        }

        float timer = 0;
        while (timer < 0.2f)
        {
            timer += Time.deltaTime;
            loadUiGroup.alpha = Mathf.Lerp(1f, 0f, timer/0.2f);
            yield return null;
        }
        loadUiGroup.blocksRaycasts = false;
        loadUi.SetActive(false);
    }
}
