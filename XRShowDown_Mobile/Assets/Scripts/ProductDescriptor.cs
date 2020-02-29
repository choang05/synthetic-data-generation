using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ProductDescriptor : MonoBehaviour
{
    public Transform productNode;

    private GameObject currentDisplayingObject;

    public void removeObjects()
    {
        Destroy(currentDisplayingObject);
        currentDisplayingObject = null;
    }

    public void showProduct(string productKey)
    {
        //  if provided key is empty... randomly grab a database key and display that object??
        if (string.IsNullOrEmpty(productKey))
        {
            int randomIndex = Random.Range(0, ProductManager.instance.products.Count-1);
            productKey = ProductManager.instance.products.ElementAt(randomIndex).Key;
        }

        //  Destroy any existing product
        if (currentDisplayingObject)
        {
            removeObjects();
        }

        //  Spawn new object
        GameObject product;
        if (ProductManager.instance.products.TryGetValue(productKey, out product))
        {
            currentDisplayingObject = Instantiate(product, productNode.position, Quaternion.identity, productNode);

            //  correct position for objects that have center/pivot NOT in center of object
            Renderer rend = currentDisplayingObject.GetComponent<Renderer>();
            if (rend)
            {
                Bounds bounds = rend.bounds;
                float yOffset = -bounds.extents.y;
                currentDisplayingObject.transform.position = transform.position + transform.position + new Vector3(0, yOffset, 0);
            }
        }
        else
        {
            Debug.LogWarning(productKey + " does not exist in database!");
        }
    }
}
