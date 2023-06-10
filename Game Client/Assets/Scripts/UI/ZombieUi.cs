using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieUi : MonoBehaviour
{
    public GameObject hudText;
 
    private void Update()
    {
        this.transform.LookAt(Camera.main.transform);
    }

    public void Show(float damage)
    {
        GameObject hud = Instantiate(hudText, transform)as GameObject;
        hud.GetComponent<Text>().text = "-" + damage.ToString();
    }

}
