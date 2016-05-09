using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Path : ScriptableObject {

    protected List<Vector2> path;

    public static int VERTICAL = 0;
    public static int HORIZONTAL = 1;

    protected int pathStep;

    public Path ()
    {
        path = new List<Vector2>();
        pathStep = 0;
    }

    public void addStep(int orientation, int numTiles)
    {
        Vector2 pathNode = new Vector2(orientation, numTiles);
        path.Add(pathNode);
    }

    public Vector2 getStep()
    {
        return path[pathStep];
    }

    public int getNumSteps()
    {
        return path.Count;
    }

    override public string ToString()
    {
        string returnVal = "";
        for (int i = 0; i < path.Count; i++)
        {
            returnVal += "{" + path[i].x + ", " + path[i].y + "}";
        }
        return returnVal;
    }

    public int getPathStep()
    {
        return pathStep;
    }

    public void incrementPathStep()
    {
        pathStep++;
    }

    // Create a clone of this Path
    public Path clone()
    {
        Path p = (Path)ScriptableObject.CreateInstance(typeof(Path));
        Vector2[] pA = path.ToArray();
        p.path = new List<Vector2>(pA);

        return p;
    }
}
