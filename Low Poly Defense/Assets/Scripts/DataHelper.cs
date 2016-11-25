using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DataHelper : MonoBehaviour {

    public static DataHelper instance;

    public Transform pingPanel;
    public Transform pingParent;
    private Transform pingedObject;

    public Canvas currentCanvas;
    public GameObject dataNotification;
    public GameObject dataNotificationResponse;
    public EventSystem eventSystem;
    private Transform currentNotification;

    public enum Position { MIDDLE_RIGHT, MIDDLE_LEFT, MIDDLE_TOP, MIDDLE_BOTTOM, TOP_LEFT, TOP_RIGHT, BOTTOM_LEFT, BOTTOM_RIGHT, CENTER };
    [Tooltip("In order of MIDDLE_RIGHT, MIDDLE_LEFT, MIDDLE_TOP, MIDDLE_BOTTOM, TOP_LEFT, TOP_RIGHT, BOTTOM_LEFT, BOTTOM_RIGHT,  CENTER")]
    public Transform[] positions;

    private void Awake()
	{
        instance = this;
	}

    public void ShowData(string data, Position position)
    {
        if (!currentNotification)
        {
            GameObject newDataNotificaiton = Instantiate(dataNotification, currentCanvas.transform) as GameObject;
            currentNotification = newDataNotificaiton.transform;
        }

        if(Position.CENTER == position)
            currentNotification.transform.position = currentCanvas.transform.position;
        else
            currentNotification.transform.position = positions[(int)position].position;

        currentNotification.transform.localScale = Vector3.one;
        currentNotification.GetComponentInChildren<Text>().text = data;
    }

    public void ShowDataWithResponse(string data, Action comfirmAction, Action cancelAction)
    {
        GameObject newDataNotificationResponse = Instantiate(dataNotificationResponse, currentCanvas.transform) as GameObject;
        newDataNotificationResponse.GetComponentInChildren<Text>().text = data;
        GameObject acceptButton = newDataNotificationResponse.transform.GetChild(0).gameObject;
        GameObject cancelButton = newDataNotificationResponse.transform.GetChild(1).gameObject;

        currentNotification = newDataNotificationResponse.transform;

        StartCoroutine(CheckForResponse(acceptButton, cancelButton, comfirmAction, cancelAction));
    }

    public void PingObject(GameObject obj)
    {
        obj.transform.SetParent(pingPanel);
        pingPanel.gameObject.SetActive(true);

        pingParent = obj.transform.parent;
        pingedObject = obj.transform;
    }

    public void PingObject(string objName)
    {
        Transform obj = currentCanvas.transform.FindChild("Gameplay/" + objName);

        Debug.Log(obj);
        if (obj)
        {
            pingParent = obj.parent;
            Debug.Log(pingParent);
            StartCoroutine(FadeIn());

            obj.SetParent(pingPanel);
            pingPanel.gameObject.SetActive(true);

            pingedObject = obj.transform;
        }
    }

    public void PingObjectCopy(string objName)
    {
        Transform obj = currentCanvas.transform.FindChild("Gameplay/"  + objName);

        Debug.Log(obj);
        if (obj)
        {
            pingParent = obj;
            Debug.Log(pingParent);
            StartCoroutine(FadeIn());

            GameObject objCopy = (GameObject)Instantiate(obj.gameObject, obj.transform.position, Quaternion.identity);
            objCopy.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 300);

            DisableAllImageTextAndRawImageComponents(pingParent);
            objCopy.transform.SetParent(pingPanel);
            objCopy.GetComponent<RectTransform>().localScale = Vector3.one;
            pingPanel.gameObject.SetActive(true);

            pingedObject = objCopy.transform;
        }
    }

    public void CancelPing(bool fadeOut, bool isCopy)
    {
        if (pingedObject)
        {
            if (fadeOut)
            {
                StartCoroutine(FadeOut());
            }

            if (!isCopy)
            {
                pingedObject.SetParent(pingParent);

                Debug.Log(pingedObject.parent);
                pingParent = null;
                pingedObject = null;
            } else
            {
                Destroy(pingedObject.gameObject);
                EnableAllImageTextAndRawImageComponents(pingParent);
            }
        } 
    }

    private void DisableAllImageTextAndRawImageComponents(Transform parent)
    {
        Text[] textComponents = parent.GetComponentsInChildren<Text>();
        Image[] imageComponents = parent.GetComponentsInChildren<Image>();
        RawImage[] rawImageComponents = parent.GetComponentsInChildren<RawImage>();

        for(int i = 0; i < textComponents.Length; i++)
        {
            textComponents[i].enabled = false;
        }

        for (int i = 0; i < imageComponents.Length; i++)
        {
            imageComponents[i].enabled = false;
        }

        for (int i = 0; i < rawImageComponents.Length; i++)
        {
            rawImageComponents[i].enabled = false;
        }
    }

    private void EnableAllImageTextAndRawImageComponents(Transform parent)
    {
        Text[] textComponents = parent.GetComponentsInChildren<Text>();
        Image[] imageComponents = parent.GetComponentsInChildren<Image>();
        RawImage[] rawImageComponents = parent.GetComponentsInChildren<RawImage>();

        for (int i = 0; i < textComponents.Length; i++)
        {
            textComponents[i].enabled = true;
        }

        for (int i = 0; i < imageComponents.Length; i++)
        {
            imageComponents[i].enabled = true;
        }

        for (int i = 0; i < rawImageComponents.Length; i++)
        {
            rawImageComponents[i].enabled = true;
        }
    }

    private IEnumerator FadeOut()
    {
        Color c = pingPanel.GetComponent<Image>().color;

        while (c.a > 0)
        {
            c.a -= Time.smoothDeltaTime * 0.5f;
            pingPanel.GetComponent<Image>().color = c;
            yield return new WaitForSeconds(Time.smoothDeltaTime);
        }
        pingPanel.gameObject.SetActive(false);
    }

    private IEnumerator FadeIn()
    {
        Color c = pingPanel.GetComponent<Image>().color;
        float currentAlpha = 0.615f;

        c.a = 0;
        pingPanel.GetComponent<Image>().color = c;

        while (c.a < currentAlpha)
        {
            c.a += Time.smoothDeltaTime * 0.5f;
            pingPanel.GetComponent<Image>().color = c;
            yield return new WaitForSeconds(Time.smoothDeltaTime);
        }
    }

    private IEnumerator CheckForResponse(GameObject acceptButton, GameObject cancelButton, Action comfirmAction, Action cancelAction)
    {
        while (eventSystem.currentSelectedGameObject != acceptButton && eventSystem.currentSelectedGameObject != cancelButton)
        {
            Debug.Log(eventSystem.currentSelectedGameObject);
            yield return new WaitForSeconds(0.1f);
        }

        if (eventSystem.currentSelectedGameObject == acceptButton)
        {
            if (comfirmAction != null)
            {
                comfirmAction.Invoke();
            }
            else
            {
                Debug.Log("Trying to close from comfirm button");
            }
        }
        if (eventSystem.currentSelectedGameObject == cancelButton)
        {
            if (cancelAction != null)
            {
                cancelAction.Invoke();
            }
            else
            {
                Debug.Log("Trying to close from cancel button");
            }
        }
        yield break;
    }

    public void CloseCurrentData()
    {
        Destroy(currentNotification.gameObject);
    }

}