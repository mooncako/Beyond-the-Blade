using UnityEngine;

[CreateAssetMenu(fileName = "GameDifficultyDataSO", menuName = "BytheBlade/GameDifficultyDataSO")]
public class GameDifficultyDataSO : ScriptableObject
{
    [field: SerializeField] public AnimationCurve DifficultyCurve { get; private set; }
}
