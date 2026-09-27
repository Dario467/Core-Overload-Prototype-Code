using System.Collections.Generic;
using TarodevController;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Transform PlayerTransform;
    private int activeLevelIndex;
    public bool IsPlayerStunned;
    [SerializeField]private Vector3 playerSpawnPoint;

    void Awake()
    {
       if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        activeLevelIndex = scene.buildIndex;
    }

    public void RestartLevel()
    {
        LoadManager.Instance.StartLoad(activeLevelIndex);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SetSpawnPoint(Vector3 spawnPoint)
    {
        playerSpawnPoint = spawnPoint;
    }

    public Vector3 GetSpawnPoint()
    {
        return playerSpawnPoint;
    }
}
