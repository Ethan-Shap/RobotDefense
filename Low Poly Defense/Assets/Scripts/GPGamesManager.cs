using UnityEngine;
using System.Collections;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;

public class GPGamesManager : MonoBehaviour {

    public static GPGamesManager instance;

	// Use this for initialization
	void Start ()
    {
        PlayGamesPlatform.Activate();
        Authenticate();
	}

    public static void SignOut()
    {
        ((PlayGamesPlatform)Social.Active).SignOut();
    }

    public void ShowAchievements()
    {
        Social.ShowAchievementsUI();
    }

    public void UpdateAchievement(string achievement, double progress)
    {

    }

    public void UpdateAllAchievements()
    {
        // Incremental enemy killer
        PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_crystal_guard, Player.instance.currentNumEnemiesKilled, (bool success) => { });
        PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_crystal_protector, Player.instance.currentNumEnemiesKilled, (bool success) => { });
        PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_crystal_guardian, Player.instance.currentNumEnemiesKilled, (bool success) =>
        { if (success) { Player.instance.currentNumEnemiesKilled = 0; } });

        //Incrememntal tower building
        PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_builder, Player.instance.currentNumTowers, (bool success) => { });
        PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_artisan, Player.instance.currentNumTowers, (bool success) => { });
        PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_craftsman, Player.instance.currentNumTowers, (bool success) => 
        { if (success) { Player.instance.currentNumTowers = 0; } });

        Social.ReportProgress(GPGSIds.achievement_completionist, Player.instance.data.numNormalLevelsCompleted / Constants.numberOfNormalLevels, (bool success) => { });
        Social.ReportProgress(GPGSIds.achievement_extreme_completionist, Player.instance.data.numHardLevelsCompleted / Constants.numberOfHardLevels, (bool success) => { });
    }

    public void ShowLeaderboards()
    {
        Social.ShowLeaderboardUI();
    }

    public void Authenticate()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Debug.Log("You've successfully logged in");
            }
            else
            {
                Debug.Log("Login failed for some reason");
            }
        });
    }

    private class Constants
    {
        //public const int crystal_guard_number = 1000;
        //public const int crystal_protector_number = 25000;
        //public const int crystal_guardian_number = 100000;
        //public const int builder_number = 500;
        //public const int artisan_number = 1000;
        //public const int craftsman_number = 5000;
        public const int numberOfNormalLevels = 6;
        public const int numberOfHardLevels = 6;
    }

}
