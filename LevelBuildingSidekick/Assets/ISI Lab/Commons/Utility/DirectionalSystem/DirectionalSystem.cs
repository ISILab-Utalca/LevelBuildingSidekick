using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface DirectionalSystem
{
    public List<Vector2> Directions { get;}

    public abstract Vector2 ConnectsTo(Vector2 dir);

    public List<T> Rotate<T>(List<T> items, int rotations)
    {
        if(items.Count != Directions.Count)
        {
            Debug.LogError("Collection size differs from directions count");
            return items;
        }
        rotations %= Directions.Count;
        return ApplyRotations(items, rotations);
    }

    abstract List<T> ApplyRotations<T>(List<T> items, int rotations);
}
