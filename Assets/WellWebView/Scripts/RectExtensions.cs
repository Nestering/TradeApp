using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectExtensions
{
    public static bool IsEmpty(this RectInt lhs)
    {
        // Returns true in the presence of NaN values.
        return (lhs.x == 0 && lhs.y == 0 && lhs.width == 0 && lhs.height == 0);
    }

    public static bool Equal(this RectInt lhs , RectInt equRct)
    {
        return (lhs.x == equRct.x && lhs.y == equRct.y && lhs.width == equRct.width &&
            lhs.height == equRct.height);
    }
}
