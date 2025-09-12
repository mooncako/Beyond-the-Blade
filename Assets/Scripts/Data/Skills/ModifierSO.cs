using UnityEngine;

public class ModifierSO : ScriptableObject
{
    public string Name;
    public Rarity Rarity;
    public string Description;


    public virtual void Perform(in ModifierContext context)
    {

    }
    
}
