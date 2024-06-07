using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRenderer
{
    GameObject RenderBox { get;}

    public void OnRenderBox();
    public void OffRenderBox();


}
