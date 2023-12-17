using UnityEngine;
using System.Collections;

public class AutoGetComponent : PropertyAttribute
{

    public From from;
    public string gameObjectName;

    public AutoGetComponent(From value) { from = value; }
    public AutoGetComponent(string gameObjectName) { this.gameObjectName = gameObjectName; }
    public AutoGetComponent(string objName, From related) { gameObjectName = objName;
                                                                      from = related; }
}


public class AutoGetAssets : PropertyAttribute
{
    public string path;
}


public enum From
{
    self,
    parent,
    children,
}