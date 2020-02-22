using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductDescriptor : MonoBehaviour
{

    public GameObject voltmeter;
    public GameObject cup;
    public GameObject lamp;
    public GameObject wrench;
    public GameObject hammer;


    void Start()
    {
        voltmeter = Instantiate(voltmeter, new Vector3(0, 0, 0), Quaternion.identity);
        cup = Instantiate(cup, new Vector3(0, 0, 0), Quaternion.identity);
        lamp = Instantiate(lamp, new Vector3(0, 0, 0), Quaternion.identity);
        wrench = Instantiate(wrench, new Vector3(0, 0, 0), Quaternion.identity);
        hammer = Instantiate(hammer, new Vector3(0, 0, 0), Quaternion.identity);

        removeObjects();
    }

    public void removeObjects()
    {
        voltmeter.gameObject.SetActive(false);
        cup.gameObject.SetActive(false);
        lamp.gameObject.SetActive(false);
        wrench.gameObject.SetActive(false);
        hammer.gameObject.SetActive(false);

    }

    public void showProduct(string productName)
    {
        removeObjects();
        switch (productName)
        {
            case "":
                Debug.Log("Stating");
                break;
            case "voltmeter":
                voltmeter.gameObject.SetActive(true);
                break;
            case "cup":
                cup.gameObject.SetActive(true);
                break;
            case "lamp":
                lamp.gameObject.SetActive(true);
                break;
            case "wrench":
                wrench.gameObject.SetActive(true);
                break;
            case "hammer":
                hammer.gameObject.SetActive(true);
                break;
        }
    }
}
