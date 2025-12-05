using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    [Tooltip("Valor de DeadWoods: 1 = Ajudou Sarue, 2 = Ajudou Jabuti, 3 = Ajudou ambos, 4 = Ajudou ninguém")]
    public int deadWoodsValue = 0;

    [Tooltip("Valor de DriedRiver: 1 = 'Ajudou' Beija-Flor, 2 = Ajudou Anta, 3 = Ajudou ambos, 4 = Ajudou ninguém")]
    public int driedRiverValue = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void UpdateDeadWoodsValue(int newValue)
    {
        deadWoodsValue = newValue;
        Debug.Log($"DeadWoods Value Updated: {deadWoodsValue}");
    }

    public void UpdateDriedRiverValue(int newValue)
    {
        driedRiverValue = newValue;
        Debug.Log($"DriedRiver Value Updated: {driedRiverValue}");
    }
}
