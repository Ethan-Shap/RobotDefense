using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public Transform fingers;

    private SceneManagement sceneManagment;
    private TouchInput touchInput;

    private void Start()
    {
        sceneManagment = FindObjectOfType<SceneManagement>();
        touchInput = FindObjectOfType<TouchInput>();

        Player.instance.Coins = 25;

        Debug.Log(sceneManagment.CurrentLevelName());
        //Checks to see if the current scene is the tutorial level
        if(sceneManagment.CurrentLevelName() == "Tutorial")
        {
            StartCoroutine(PlayTutorial());
        }
    }

    private IEnumerator PlayTutorial()
    {
        DataHelper.instance.ShowData("Welcome to the Tutorial Level!", DataHelper.Position.CENTER);

        yield return new WaitForSeconds(1f);

        DataHelper.instance.ShowData("To exit the Tutorial Level, press the button in the top right-", DataHelper.Position.MIDDLE_RIGHT);
        DataHelper.instance.PingObject("Pause Button");

        yield return new WaitForSeconds(4f);

        DataHelper.instance.ShowData("Then click the Home Button to return to the main menu.", DataHelper.Position.MIDDLE_RIGHT);

        yield return new WaitForSeconds(3f);

        DataHelper.instance.CancelPing(true, false);

        DataHelper.instance.ShowData("Now, let's move the camera.", DataHelper.Position.BOTTOM_RIGHT);

        yield return new WaitForSeconds(3f);

#if unity_android
        DataHelper.instance.ShowData("Use one finger to move the camera side to side.", DataHelper.Position.BOTTOM_RIGHT);
        fingers.GetComponent<Animator>().SetBool("Move Camera", true);


        float touchTime = 0;
        while(touchTime < 5)
        {
            if (touchInput.movingCamera)
            {
                touchTime += Time.smoothDeltaTime;
            }
            yield return null;
        }
#endif

#if unity_android
        fingers.GetComponent<Animator>().SetBool("Move Camera", false);

        DataHelper.instance.ShowData("Use two fingers to zoom in and out.", DataHelper.Position.BOTTOM_RIGHT);

        fingers.GetComponent<Animator>().SetBool("Zoom", true);

        touchTime = 0;
        while (touchTime < 5)
        {
            if (touchInput.zooming)
            {
                fingers.GetComponent<Animator>().SetBool("Zoom", false);
                touchTime += Time.smoothDeltaTime;
            }
            yield return null;
        }
#endif
        DataHelper.instance.ShowData("Let's place an Arrow Tower!", DataHelper.Position.MIDDLE_TOP);

        yield return new WaitForSeconds(3f);

        DataHelper.instance.ShowData("Click on the Arrow Tower icon to preview it!", DataHelper.Position.MIDDLE_TOP);

        DataHelper.instance.PingObjectCopy("Shop Button/Shop/ShopTowerItem (1)");

        while (TowerManager.instance.selectedTower == null)
        {
            yield return null;
        }

        DataHelper.instance.CancelPing(true, true);

        yield return new WaitForSeconds(2f);

        DataHelper.instance.ShowData("Towers can only be placed on flat tiles shown by the blue icons.", DataHelper.Position.BOTTOM_RIGHT);
        SnapToGrid.instance.PingSnapPositions();

        yield return new WaitForSeconds(4f);

        DataHelper.instance.ShowData("Press the boxes in the top right to comfirm or cancel a tower purchase.", DataHelper.Position.BOTTOM_RIGHT);

        while (!TowerManager.instance.selectedTower.IsPurchased())
        {
            yield return null;
        }

        SnapToGrid.instance.StopPingSnapPositions();

        DataHelper.instance.ShowData("Now, press the Start Round button!", DataHelper.Position.BOTTOM_RIGHT);

        while (!GameManager.instance.RoundStarted)
        {
            yield return null;
        }

        GameManager.instance.ResumeRound();

        DataHelper.instance.ShowData("Now finish off the round!", DataHelper.Position.BOTTOM_RIGHT);

        int currentEnemyType = -1;

        while (!GameManager.instance.GameEnded)
        {
            if (currentEnemyType < EnemyManager.instance.GetGreatestEnemyType())
            {
                currentEnemyType++;
                yield return new WaitForSeconds(1f);
                GameManager.instance.PauseRound(false);
                switch (currentEnemyType)
                {
                    // White
                    case 0:                      
                        DataHelper.instance.ShowData("White enemies only take one hit to kill.", DataHelper.Position.MIDDLE_BOTTOM);
                        break;
                    // Red
                    case 1:
                        DataHelper.instance.ShowData("Red enemies can't be affected by fire towers. They take 2 shots to kill.", DataHelper.Position.MIDDLE_BOTTOM);
                        break;
                    // Blue
                    case 2:
                        DataHelper.instance.ShowData("Blue enemies are not affected by ice towers. They take 3 shots to kill.", DataHelper.Position.MIDDLE_BOTTOM);
                        break;
                    // Grey
                    case 3:
                        DataHelper.instance.ShowData("Grey enemies are reinforced. They take 6 shots to kill.", DataHelper.Position.MIDDLE_BOTTOM);
                        break;
                    // BOSS
                    case 4:
                        DataHelper.instance.ShowData("Bosses take 100 shots to kill. In the last 5 rounds of every level, bosses will spawn.", DataHelper.Position.MIDDLE_BOTTOM);
                        break;
                }
                yield return new WaitForSeconds(6f);
                DataHelper.instance.CloseCurrentData();
                GameManager.instance.ResumeRound();
            }
            yield return null;
        }

        DataHelper.instance.ShowData("Good job! You finished the game! Continue on to the main levels!", DataHelper.Position.BOTTOM_RIGHT);

    }

}
