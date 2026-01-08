using Sirenix.OdinInspector;
using UnityEngine;

public class ModifierSO : ScriptableObject
{
    public string Name;
    public Rarity Rarity;
    public string Description;
    [PreviewField] public Sprite Icon;


    public virtual void Perform(in ModifierContext context)
    {

    }
    
}
