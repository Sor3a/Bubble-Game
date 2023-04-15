using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Announcer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI AnnouncerText;

    public static Announcer Instance;

    int CurrentCombot;
    [SerializeField] Color textColor;
    Color alpha;

    private void Awake()
    {
        Instance = this;
        CurrentCombot = 0;
        alpha = new Color(0, 0, 0, 1);
    }
    IEnumerator disableText(float time)
    {
        yield return new WaitForSeconds(time);
        AnnouncerText.text = "";
    }
    public void Announce(string text,float timeToDisable=1f)
    {
        StopAllCoroutines();
        AnnouncerText.text = text;
        AnnouncerText.color = textColor;
        StartCoroutine(disableText(timeToDisable));
    }

    IEnumerator disableCombot()
    {
        yield return new WaitWhile(() =>
        {
            while (AnnouncerText.color.a > 0)
            {
                AnnouncerText.color -= alpha*Time.deltaTime;
                return true;
            }
            return false;
        });

        AnnouncerText.text = "";
        CurrentCombot = 0;
    }
    public void DoCombot()
    {
        StopAllCoroutines();
        CurrentCombot++;
        AnnouncerText.text = "X" + CurrentCombot.ToString();
        AnnouncerText.color = textColor;
        StartCoroutine(disableCombot());
    }
}
