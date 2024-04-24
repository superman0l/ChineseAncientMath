using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstCover : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var flowchart = GameObject.Find("FlowChart").GetComponent<Flowchart>();
        if (!(bool)(flowchart.GetPublicVariables()[0])) gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        var flowchart = GameObject.Find("FlowChart").GetComponent<Flowchart>();
        if (!(bool)(flowchart.GetPublicVariables()[0])) Destroy(gameObject);
    }
}
