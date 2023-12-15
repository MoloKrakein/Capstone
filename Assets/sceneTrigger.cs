using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneTrigger : MonoBehaviour
{ // Nama scene yang akan dipindahkan
    public string targetSceneName;

    // Method ini akan dipanggil ketika objek diklik
    private void OnMouseDown()
    {
        // Pindah ke scene lain
        SceneManager.LoadScene(targetSceneName);
    }
}
