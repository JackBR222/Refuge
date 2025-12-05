using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class Door : MonoBehaviour
{
    [Header("Destino")]
    public string destinationSceneName; // nome da cena no Build Settings
    public string arrivalSpawnName = "default"; // nome do Empty na cena de destino

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Spawn.NextSpawnName = arrivalSpawnName;
        SceneManager.LoadScene(destinationSceneName);
    }
}
