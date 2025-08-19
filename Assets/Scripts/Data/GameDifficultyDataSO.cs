using UnityEngine;

[CreateAssetMenu(fileName = "GameDifficultyDataSO", menuName = "BytheBlade/GameDifficultyDataSO")]
public class GameDifficultyDataSO : ScriptableObject
{
    [SerializeField] public float TotalLevelCount = 15;
    [field: SerializeField] public AnimationCurve MinDifficultyCurve { get; private set; }
    [field: SerializeField] public AnimationCurve MaxDifficultyCurve { get; private set; }
}
