using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static partial class U
{
    public static Vector3 Vert(this ref Bounds box, int x, int y, int z)
    {
        return new Vector3(
            box.min.x + box.size.x * x,
            box.min.y + box.size.y * y,
            box.min.z + box.size.z * z);
    }

    public static IEnumerable<Vector3> Verts(this Bounds box)
    {
        for (int x = 0; x <= 1; x++)
        {
            for (int y = 0; y <= 1; y ++)
            {
                for (int z = 0;z  <= 1; z ++)
                {
                    yield return box.Vert(x,y,z);
                }
            }
        }
    }
}
