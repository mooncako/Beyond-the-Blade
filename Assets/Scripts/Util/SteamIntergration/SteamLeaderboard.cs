using UnityEngine;
using Steamworks;
using UnityEngine.SocialPlatforms.Impl;

public class SteamLeaderBoard : MonoBehaviour
{
    private SteamLeaderboard_t _leaderboard;
    private CallResult<LeaderboardFindResult_t> _leaderboardFindResult;
    private CallResult<LeaderboardScoreUploaded_t> _scoreUploadResult;

    public const string ClearTimeLeaderboard = "Clear Time";


    void Start()
    {
        if (SteamManager.Initialized)
        {
            _leaderboardFindResult = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFindResult);
            SteamAPICall_t handle = SteamUserStats.FindOrCreateLeaderboard(ClearTimeLeaderboard, ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);

            _leaderboardFindResult.Set(handle);
        }
    }

    void Update()
    {
        SteamAPI.RunCallbacks();
    }

    private void OnLeaderboardFindResult(LeaderboardFindResult_t pCallback, bool bIOFailure)
    {
        if (bIOFailure || pCallback.m_bLeaderboardFound == 0)
        {
            Debug.LogError("Leaderboard not found or failed to retrieve.");
            return;
        }

        _leaderboard = pCallback.m_hSteamLeaderboard;
        Debug.Log("Leaderboard found!");
    }

    public void UploadScore(int score)
    {
        _scoreUploadResult = CallResult<LeaderboardScoreUploaded_t>.Create(OnScoreUploaded);

        SteamAPICall_t handle = SteamUserStats.UploadLeaderboardScore(
            _leaderboard,
            ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodNone,
            score,
            null,
            0
        );
    }

    private void OnScoreUploaded(LeaderboardScoreUploaded_t pCallback, bool bIOFailure)
    {
        if (bIOFailure || pCallback.m_bSuccess == 0)
        {
            Debug.LogError("Score upload failed.");
            return;
        }

        Debug.Log($"Score uploaded, New rank: {pCallback.m_nGlobalRankNew}");
    }
}
