using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpText : MonoBehaviour
{
    private float speed = 1.5f;
    private float timer = 0f;
    private float time = 0.5f;
 
    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
        timer += Time.deltaTime;
        GetComponent<Text>().color = new Color(1,0,0,1 - timer);
        Destroy(gameObject,time);
    }
}
