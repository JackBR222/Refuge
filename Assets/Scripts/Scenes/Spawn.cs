using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawn : MonoBehaviour
{
    private static Spawn Instance;

    public static string NextSpawnName;

    [Header("Spawn do Player")]
    public string defaultSpawnName = "default";
    public bool makePersistent = false;

    [Header("Câmera 2D")]
    public Camera cameraToFollow;
    public Vector3 cameraOffset = Vector3.zero;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (makePersistent)
        {
            DontDestroyOnLoad(gameObject);
        }

        if (cameraToFollow == null)
        {
            cameraToFollow = Camera.main;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (Instance == this)
            Instance = null;
    }

    void Start()
    {
        TryApplySpawn();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (Instance == this)
            TryApplySpawn();
    }

    void TryApplySpawn()
    {
        string nameToUse = !string.IsNullOrEmpty(NextSpawnName) ? NextSpawnName : defaultSpawnName;

        GameObject target = GameObject.Find(nameToUse);
        if (target != null)
        {
            transform.position = target.transform.position;
            transform.rotation = target.transform.rotation;

            if (cameraToFollow != null)
            {
                Vector3 newCamPos = new Vector3(
                    target.transform.position.x + cameraOffset.x,
                    target.transform.position.y + cameraOffset.y,
                    cameraToFollow.transform.position.z
                );
                cameraToFollow.transform.position = newCamPos;
            }

            NextSpawnName = null;
        }
        else
        {
            Debug.LogWarning(
                $"[PlayerSpawn2D] Spawn '{nameToUse}' não encontrado na cena ({SceneManager.GetActiveScene().name}). " +
                $"Crie um Empty com esse NOME ou ajuste 'defaultSpawnName'."
            );
        }
    }
}
