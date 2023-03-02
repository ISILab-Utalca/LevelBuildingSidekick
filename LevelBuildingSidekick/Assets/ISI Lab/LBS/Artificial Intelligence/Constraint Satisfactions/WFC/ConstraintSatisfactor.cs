using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConstraintSatisfactor<T> //where T is lbsAlgo
{
    private List<Rule> rules = new List<Rule>();

    public List<Rule> Rules => new List<Rule>(rules);

    public abstract bool Verify(T target);

    public abstract T Execute(T target = default(T)); // esto puede ser generar o algo asi pero en abstracto
}
