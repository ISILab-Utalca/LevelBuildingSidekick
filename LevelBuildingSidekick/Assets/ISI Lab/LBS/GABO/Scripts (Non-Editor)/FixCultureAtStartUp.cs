using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class FixCultureAtStartUp
{
    static FixCultureAtStartUp()
    {
        // Busca arreglar problemas de parseo, como floats usando coma en vez de punto.
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
    }
}
