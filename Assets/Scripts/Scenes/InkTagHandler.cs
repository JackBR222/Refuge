using Ink.Runtime;
using System.Collections.Generic;
using UnityEngine;

public class InkTagHandler : MonoBehaviour
{
    public void HandleTags(List<string> currentTags)
    {
        if (currentTags == null || currentTags.Count == 0)
            return;

        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2) continue;

            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            // Verifica se a tag é do tipo "DeadWoods" ou "DriedRiver"
            if (tagKey.Equals("DeadWoods", System.StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(tagValue, out int newValue))
                {
                    if (GameStateManager.Instance != null)
                        GameStateManager.Instance.UpdateDeadWoodsValue(newValue);
                    else
                        Debug.LogWarning("GameStateManager.Instance está nulo ao tentar atualizar DeadWoods.");
                }
            }
            else if (tagKey.Equals("DriedRiver", System.StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(tagValue, out int newValue))
                {
                    if (GameStateManager.Instance != null)
                        GameStateManager.Instance.UpdateDriedRiverValue(newValue);
                    else
                        Debug.LogWarning("GameStateManager.Instance está nulo ao tentar atualizar DriedRiver.");
                }
            }
        }
    }
}