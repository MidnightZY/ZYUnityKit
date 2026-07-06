using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string targetScene = "target_scene";
    [SerializeField] private GameObject loading;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private bool useAsyncLoad = true;

    private bool isLoading;

    private void Awake()
    {
        if (loading != null)
        {
            loading.SetActive(false);
        }

        UpdateProgressUI(0f);
    }

    public void LoadGameSync()
    {
        LoadScene(targetScene, false);
    }

    public void LoadGameASync()
    {
        LoadScene(targetScene, true);
    }

    public void LoadGame()
    {
        LoadScene(targetScene, useAsyncLoad);
    }

    public void LoadScene(string sceneName)
    {
        LoadScene(sceneName, useAsyncLoad);
    }

    public void LoadScene(string sceneName, bool async)
    {
        if (isLoading)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("Scene name is empty.");
            return;
        }

        if (async)
        {
            StartCoroutine(LoadAsync(sceneName));
            return;
        }

        isLoading = true;
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator LoadAsync(string sceneName)
    {
        isLoading = true;

        if (loading != null)
        {
            loading.SetActive(true);
        }

        UpdateProgressUI(0f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        if (asyncLoad == null)
        {
            Debug.LogWarning($"Scene failed to start loading: {sceneName}");
            isLoading = false;
            yield break;
        }

        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            UpdateProgressUI(progress);
            yield return null;
        }

        UpdateProgressUI(1f);
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private void UpdateProgressUI(float progress)
    {
        if (progressBar != null)
        {
            progressBar.value = progress;
        }

        if (progressText != null)
        {
            progressText.text = progress.ToString("P0");
        }
    }
}
