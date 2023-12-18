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

    // change scene
    public void changeScene()
    {
        // play canvasStartAnimation
        canvasStart.GetComponent<Animator>().SetTrigger("start");
        // set canvasStory to active
        canvasStory.gameObject.SetActive(true);
    }

    public void PlayStory(string story,TextMeshProUGUI textStory)
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator PlayText(string story,TextMeshProUGUI textStory)
    {
        textStory.text = "";
        foreach (char c in story)
        {
            textStory.text += c;
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(targetSceneName);
    }
}
