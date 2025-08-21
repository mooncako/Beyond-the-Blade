using UnityEngine;

[CreateAssetMenu(fileName = "GameDifficultyDataSO", menuName = "BytheBlade/GameDifficultyDataSO")]
public class GameDifficultyDataSO : ScriptableObject
{
    [SerializeField] public float TotalLevelCount = 15;
    [SerializeField] public int StartingWaveBudget = 10;
    [field: SerializeField] public float BudgetScale = 1.15f;
    [field: SerializeField] public AnimationCurve MinDifficultyCurve { get; private set; }
    [field: SerializeField] public AnimationCurve MaxDifficultyCurve { get; private set; }
}
