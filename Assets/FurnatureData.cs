using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(menuName = "Data/Furnature")]
public class FurnatureData : ScriptableObject
{
    public double Cost;
    public List<RoomKind> RoomKinds;
    public PlacementKind PlacementKind;
    public Bounds Bounds;

    public GameObject Prefab;
    // Start is called before the first frame update
    public void CalcBounds()
    {
        Debug.Log($"help im {name}");
        Collider[] colliders = Prefab.GetComponentsInChildren<Collider>();
        var bounds = Bounds;
        //bounds = new Bounds();
        foreach (Collider c in colliders)
        {
            var transform = c.transform.localToWorldMatrix * Prefab.transform.worldToLocalMatrix;
            var points = c.bounds.Verts().Select(c.transform.worldToLocalMatrix.MultiplyPoint);
                //.Select(Prefab.transform.worldToLocalMatrix.MultiplyPoint);
            foreach (var point in points)
            {
                bounds.Encapsulate(point);
            }
        }
        Debug.Log(bounds);
        Bounds = bounds;
    }
}
