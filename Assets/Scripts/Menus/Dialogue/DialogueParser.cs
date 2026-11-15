using UnityEngine;

public class DialogueParser
{
    public static string[] GetDialogueLines(TextAsset textAsset)
    {
        return textAsset.ToString().Split(";");
    }
}
