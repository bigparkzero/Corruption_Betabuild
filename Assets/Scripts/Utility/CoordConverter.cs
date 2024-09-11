using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoordConverter
{
    // µ¥Ä«¸£Æ® ÁÂÇ¥°è¸¦ ±¸¸é ÁÂÇ¥°è·Î º¯È¯
    public static Vector3 CartesianToSpherical(Vector3 coord)
    {
        float r = Mathf.Sqrt(coord.x * coord.x + coord.y * coord.y + coord.z * coord.z);
        return new Vector3(
            r,
            Mathf.Atan2(coord.x, coord.z), // ¼¼Å¸ (¥è)
            Mathf.Acos(coord.y / r)        // ÆÄÀÌ (¥õ)
        );
    }

    // ±¸¸é ÁÂÇ¥°è¸¦ µ¥Ä«¸£Æ® ÁÂÇ¥°è·Î º¯È¯
    public static Vector3 SphericalToCartesian(Vector3 coord)
    {
        return new Vector3(
            coord.x * Mathf.Sin(coord.z) * Mathf.Sin(coord.y), // x ÁÂÇ¥
            coord.x * Mathf.Cos(coord.z),                      // y ÁÂÇ¥
            coord.x * Mathf.Sin(coord.z) * Mathf.Cos(coord.y)  // z ÁÂÇ¥
        );
    }
}
