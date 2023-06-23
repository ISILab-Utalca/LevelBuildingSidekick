using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LBSEvents
{
    public static Action<object> OnSelectElementInWorkSpace;
}
// poner esto en un namespace  y que sea internal para que solo cosas de la
// herramienta puedan invokar este evento, ademas tiene que ser editor (!)
// 
// tambien podemos poner una clase que quede no tnega la restriccion de
// namespace asi cosas que no son sep ueden suscribir pero no pueden invocar
// el evento (!)