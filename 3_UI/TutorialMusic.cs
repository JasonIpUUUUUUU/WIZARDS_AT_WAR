using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialMusic : MonoBehaviour
{
    private void Awake()
    {
        // Make it so the music stays throughout the tutorial
        DontDestroyOnLoad(gameObject);
    }
}
