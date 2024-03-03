using System;
using System.Collections.Generic;
using GameObjectsComponentsLevels.GameObjects;
using Microsoft.Xna.Framework.Graphics;

namespace SSGMU.Components;
public class ComponentName : Component
{
    public ComponentName(GameObject parent) : base(parent)
    {
        type = "[ComponentName]";
    }
}