using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class sceneTrigger : MonoBehaviour
{ // Nama scene yang akan dipindahkan
    public string targetSceneName;
    public Canvas canvasStart;
    public Canvas canvasStory;
    public TextMeshProUGUI textStory;
    // text area for story
    [TextArea(10,10)]
    public string story;
    private float delayDuration;

    // change scene
    public void changeScene()
    {
        // play canvasStartAnimation
        canvasStart.GetComponent<Animator>().SetTrigger("start");
        // set canvasStory to active
        canvasStory.gameObject.SetActive(true);
        PlayStory(story,textStory);
    }

    public void PlayStory(string story,TextMeshProUGUI textStory)
    {
        delayDuration = story.Length * 0.05f;
        // calculate how many enters in story
        int enterCount = 0;
        foreach(char letter in story.ToCharArray())
        {
            if(letter == '\n')
            {
                enterCount++;
            }
        }
        delayDuration = delayDuration+enterCount * 1.5f;
        StartCoroutine(PlayText(story,textStory));
        StartCoroutine(delay());
        // StartCoroutine(LoadScene());
    }
    IEnumerator delay()
    {
        yield return new WaitForSeconds(delayDuration);
        StartCoroutine(LoadScene());
    }

    // split text char into 2 parts and play it, give delay between each story part
    IEnumerator PlayText(string story,TextMeshProUGUI textStory)
    {
        yield return new WaitForSeconds(3f);
        string[] storyParts = story.Split('\n');
        foreach(string storyPart in storyParts)
        {
            textStory.text = "";
            foreach(char letter in storyPart.ToCharArray())
            {
                textStory.text += letter;
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(3f);
        
        SceneManager.LoadScene(targetSceneName);
    }
}
