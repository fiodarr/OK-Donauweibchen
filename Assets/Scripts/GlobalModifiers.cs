using System.Collections.Generic;
using UnityEngine;

public class GlobalModifiers : MonoBehaviour
{
    public static GlobalModifiers Instance;

    [SerializeField] private List<ModifierEntry> modifierEntries;

    private Dictionary<string, Modifier> modifiers;

    private void Awake()
    {
        Instance = this;

        modifiers = new Dictionary<string, Modifier>();

        foreach (var entry in modifierEntries)
            modifiers.Add(entry.name, entry.modifier);
    }

    public Modifier Get(string name)
    {
        return modifiers.GetValueOrDefault(name);
    }
}