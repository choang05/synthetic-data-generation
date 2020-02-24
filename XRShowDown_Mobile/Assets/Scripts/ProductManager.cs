using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

public class ProductManager : MonoBehaviour
{
    public static ProductManager instance = null;              //Static instance which allows it to be accessed by any other script.

    //  dictionary to hold all our objects
    [Serializable] public class ProductDictionary : SerializableDictionaryBase<string, GameObject> { }
    public ProductDictionary products = new ProductDictionary();

    #region [TODO] use data structure instead of just gameobject for the dictionary
    //  data of our object
    public struct ObjectData
    {
        public string name;
        public string description;
        public GameObject modelPrefab;
    }
    #endregion

    private void Awake()
    {
        #region Instance Check
        //Check if instance already exists
        if (instance == null)
        {
            //if not, set instance to this
            instance = this;
        }
        //If instance already exists and it's not this:
        else if (instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        }

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
        #endregion
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
