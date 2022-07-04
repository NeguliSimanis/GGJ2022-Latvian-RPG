using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestButton : MonoBehaviour
{
    [SerializeField]
    Text testText;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(ButtonTest);
    }

   public void ButtonTest()
    {
        testText.text = "test successful";
    }
}
