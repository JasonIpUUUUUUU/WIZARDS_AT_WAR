using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlinkingUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(repeatBlink());
    }  

    IEnumerator repeatBlink()
    {
        yield return new WaitForSeconds(1);
        GetComponent<TextMeshProUGUI>().enabled = !GetComponent<TextMeshProUGUI>().enabled;
        StartCoroutine(repeatBlink());
    }
}
