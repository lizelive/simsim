using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorDecorator : MonoBehaviour
{
    public List<FurnatureData> Furnature;
    public double TotalBudget = 1000;
    public int MaxTries = 1000;
    public Transform furnatureTransform;
    public Vector3 HeadPos => transform.position + Vector3.up;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var furnature in Furnature)
        {
            furnature.CalcBounds();
        }
        WorkMagic();
    }

    void DebugDrawCube(Bounds box, Vector3 pos, Quaternion rotation)
    {

        var transform = Matrix4x4.TRS(pos, rotation, Vector3.one);


        void DrawLine(Vector3 a, Vector3 b)
        {
            Debug.DrawLine(b, a, Color.red);

            Debug.DrawLine(transform.MultiplyPoint(a), transform.MultiplyPoint(b), Color.red);
        }
        var max = box.max;
        var min = box.min;
        DrawLine(box.Vert(1, 1, 1), box.Vert(0, 1, 1));
        DrawLine(box.Vert(1, 1, 1), box.Vert(1, 1, 0));
        DrawLine(box.Vert(0, 1, 0), box.Vert(0, 1, 1));
        DrawLine(box.Vert(0, 1, 0), box.Vert(1, 1, 0));
        DrawLine(box.Vert(1, 0, 1), box.Vert(0, 0, 1));
        DrawLine(box.Vert(1, 0, 1), box.Vert(1, 0, 0));
        DrawLine(box.Vert(0, 0, 0), box.Vert(0, 0, 1));
        DrawLine(box.Vert(0, 0, 0), box.Vert(1, 0, 0));
    }



    bool TryPlace(FurnatureData furnature)
    {
        var wallDir = Random.insideUnitCircle.normalized;
        var randomDir = Random.onUnitSphere;
        var floorDir = randomDir;
        floorDir.y = -1;
        floorDir.Normalize();
        var ceilDir = -floorDir;
        var wallRayDir = new Vector3(wallDir.x, 0, wallDir.y);

        var wantedKind = furnature.PlacementKind;
        var rayDir = wantedKind switch
        {
            PlacementKind.Wall => randomDir,
            PlacementKind.Ceiling => -floorDir,
            PlacementKind.FloorEdge => wallRayDir,
            PlacementKind.Floor => floorDir,
            _ => randomDir,
        };

        if (Physics.Raycast(HeadPos, rayDir, out var hit, 100f))
        {
            var spaceing = 0.01f;

            var hitNormal = hit.normal;

            var hitPos = hit.point + hitNormal * spaceing;
            var hitNormalFlat = hitNormal;
            
            hitNormalFlat.y = 0;
            hitNormalFlat.Normalize();
            var rotation = Quaternion.LookRotation(hitNormalFlat);

            var kind = FigureOutHitKind(hitNormal);

            if(kind == PlacementKind.Wall && wantedKind == PlacementKind.FloorEdge)
            {
                hitNormal = hitNormalFlat;
                rotation = Quaternion.LookRotation(hitNormalFlat);
                kind = PlacementKind.FloorEdge;

                var placePostion = hit.point - (furnature.Bounds.min.z - spaceing) * hitNormalFlat;
                placePostion.y = furnature.Bounds.min.y + spaceing;

                hitPos = placePostion;
            }

            if (kind != wantedKind)
                return false;


            // assume the rotaion is going to be something


            
            Debug.DrawLine(HeadPos, hit.point, Color.green);
            Debug.DrawLine(hit.point, hitPos, Color.green);
            DebugDrawCube(furnature.Bounds, hitPos, rotation);

            var spaceOpen = !Physics.CheckBox(furnature.Bounds.center + hitPos, furnature.Bounds.size / 2, rotation);

            if (spaceOpen)
            {
                Instantiate(furnature.Prefab, hitPos, rotation, furnatureTransform);
            }

            return spaceOpen;
        }
        return false;
    }

    private static PlacementKind FigureOutHitKind(Vector3 hitNormal)
    {
        PlacementKind hitKind = PlacementKind.Wall;
        var roundedHitNormal = new Vector3(Mathf.Round(hitNormal.x), Mathf.Round(hitNormal.y), Mathf.Round(hitNormal.z));
        if (roundedHitNormal.y == -1)
            hitKind = PlacementKind.Ceiling;
        if (roundedHitNormal.y == 1)
            hitKind = PlacementKind.Floor;
        return hitKind;
    }

    void WorkMagic()
    {
        foreach (Transform child in furnatureTransform)
        {
            Destroy(child);
        }

        int tries = 0;
        var budget = TotalBudget;
        while (budget > 0 && tries++ <= MaxTries)
        {
            var furnature = Furnature[Random.Range(0, Furnature.Count)];
            var match = TryPlace(furnature);
            if (match)
                budget -= furnature.Cost;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
