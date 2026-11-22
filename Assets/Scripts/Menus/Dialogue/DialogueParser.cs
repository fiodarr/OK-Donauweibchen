using UnityEngine;
using System;

[Serializable]
public class DialogueLine
{
    public string name;
    public string line;
}

public class DialogueParser
{
    public static DialogueLine[] GetDialogueLines(TextAsset textAsset)
    {
        string[] entries = textAsset.text.Split(
            ';',
            StringSplitOptions.RemoveEmptyEntries
        );

        DialogueLine[] result = new DialogueLine[entries.Length];

        for (int i = 0; i < entries.Length; i++)
        {
            int separator = entries[i].IndexOf(':');

            result[i] = new DialogueLine
            {
                name = entries[i].Substring(0, separator).Trim(),
                line = entries[i].Substring(separator + 1).Trim()
            };
        }

        return result;
    }
}