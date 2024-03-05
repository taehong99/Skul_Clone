using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    // Return true of layerMask contains layer
    public static bool Contains(this LayerMask layerMask, int layer)
    {
        return ((1 << layer) & layerMask) != 0;
    }
}
