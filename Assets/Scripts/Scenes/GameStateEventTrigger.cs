using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameStateEventTrigger : MonoBehaviour
{
    public enum VariableSource
    {
        DeadWoods,
        DriedRiver
    }

    [Header("Selecione qual variável será usada")]
    public VariableSource variableSource = VariableSource.DeadWoods;

    [Header("Eventos baseados no valor escolhido (1 a 4)")]
    public UnityEvent event_1;  // Quando valor for 1
    public UnityEvent event_2;  // Quando valor for 2
    public UnityEvent event_3;  // Quando valor for 3
    public UnityEvent event_4;  // Quando valor for 4

    private void OnEnable()
    {
        CheckValue();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckValue();
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void CheckValue()
    {
        int value = 0;

        switch (variableSource)
        {
            case VariableSource.DeadWoods:
                value = GameStateManager.Instance.deadWoodsValue;
                break;
            case VariableSource.DriedRiver:
                value = GameStateManager.Instance.driedRiverValue;
                break;
        }

        switch (value)
        {
            case 1:
                event_1.Invoke();
                break;
            case 2:
                event_2.Invoke();
                break;
            case 3:
                event_3.Invoke();
                break;
            case 4:
                event_4.Invoke();
                break;
            default:
                Debug.LogWarning("Valor não reconhecido: " + value);
                break;
        }
    }
}