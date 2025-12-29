using UnityEngine;

[CreateAssetMenu(fileName = "New Tag", menuName = "GAS/GameplayTag")]
public class GameplayTag : ScriptableObject
{
    [TextArea]
    public string description;
}
