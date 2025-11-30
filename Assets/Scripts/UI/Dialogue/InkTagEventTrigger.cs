using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class TagMatch
{
    [Tooltip("Valor esperado da tag (ex: acalmou_sarue)")]
    public string expectedValue;

    [Tooltip("Evento a ser executado quando a tag combinar")]
    public UnityEvent onMatch;
}

public class InkTagEventTrigger : MonoBehaviour
{
    [Header("Tipo de tag para escutar (ex: outcome, resultado, etc.)")]
    public string tagType = "outcome";

    [Header("Lista de respostas possíveis")]
    public List<TagMatch> matches = new();

    /// <summary>
    /// Chamado pelo InkDialogueManager ao detectar uma tag.
    /// </summary>
    public void CheckTag(string tag)
    {
        var parts = tag.Split(':');
        if (parts.Length != 2) return;

        string tagKey = parts[0].Trim();
        string tagValue = parts[1].Trim();

        if (!tagKey.Equals(tagType, StringComparison.OrdinalIgnoreCase))
            return;

        foreach (var match in matches)
        {
            if (tagValue.Equals(match.expectedValue, StringComparison.OrdinalIgnoreCase))
            {
                match.onMatch.Invoke();
            }
        }
    }
}
