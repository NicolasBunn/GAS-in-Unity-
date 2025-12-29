using UnityEngine;

[CreateAssetMenu(fileName = "GameplayData", menuName = "GAS/GameplayData")]
public class GameplayData : ScriptableObject
{
    public int integer_data_1;
    public int integer_data_2;

    public float float_data_1;
    public float float_data_2;

    public string string_data_1;
    public string string_data_2;

    public GameplayTag gameplay_tag_1;
    public GameplayTag gampelay_tag_2;
}
