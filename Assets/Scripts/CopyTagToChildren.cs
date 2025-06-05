/**************************************************************************
 * Filename: CopyTagToChildren.cs  (Not Used)
 * Author: Amir Tarbiyat B00882695
 * Description:
 *     This script recursively assigns the parent's tag to all of its child 
 *     objects in the hierarchy. It ensures that the tag of the parent is 
 *     consistently propagated to all descendants, including nested children.
 *     This is useful for organizing and managing objects with shared 
 *     behavior or properties based on tags.
 * 
 **************************************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyTagToChildren : MonoBehaviour
{
    // This function will copy the parent's tag to all child objects
    void Start()
    {
        CopyTagToAllChildren(transform);
    }

    private void CopyTagToAllChildren(Transform parent)
    {
        // Iterate through each child
        foreach (Transform child in parent)
        {
            // Assign the parent's tag to the child
            child.tag = parent.tag;

            // Recursively call this function if the child has more children
            if (child.childCount > 0)
            {
                CopyTagToAllChildren(child);
            }
        }
    }
}
