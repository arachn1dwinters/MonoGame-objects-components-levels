﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GameObjectsComponentsLevels.Components;
using GameObjectsComponentsLevels.Helpers;
using GameObjectsComponentsLevels.Levels;

namespace GameObjectsComponentsLevels.GameObjects;
public class GameObject
{
    // Components
    public List<Component> attributes;

    // Sprite and rect
    public Rectangle rect;
    public Texture2D sprite;
    public Vector2Int position;
    public Vector2Int sizeMultiplier;
    public Rectangle cropRect;
    public int height;
    public int width;
    public bool cropped;
    public int rotation;
    public Vector2Int topLeftCorner = new(0, 0);
    public bool dontRender;

    // Parents
    public Level parent;

    // Name
    public string type;

    public bool collidable = true;

    // Regular GameObject
    public GameObject(Texture2D sprite, Vector2Int position, Vector2Int sizeMultiplier, string type, Level parent, List<string> attributesToAdd = null)
    {
        // Creating attributes, position, size, and sprite
        this.attributes = new();
        this.sprite = sprite;
        this.position = position;
        this.sizeMultiplier = sizeMultiplier;

        // Creating the rectangle
        rect = new(position.x, position.y, sprite.Width * sizeMultiplier.x, sprite.Height * sizeMultiplier.y);
        this.width = sprite.Width;
        this.height = sprite.Height;

        // Create attributes
        if (attributesToAdd != null)
        {
            foreach (string i in attributesToAdd)
            {
                AddAttribute(i);
            }
        }

        // Set parent
        this.parent = parent;

        // Set name
        this.type = type;

        // Add this to the parent's list of Objects
        parent.GameObjects.Add(this);

        // Check if its a tile and add it to the parents list of tiles
        if (type == "tile")
        {
            parent.parent.tiles.Add(this);
        }
    }

    // GameObject without a sprite
    public GameObject(Vector2Int size, Vector2Int topLeftCorner, Vector2Int position, Vector2Int sizeMultiplier, string type, Level parent, List<string> attributesToAdd = null)
    {
        // Creating attributes, position, size, and sprite
        this.attributes = new();
        this.position = position;
        this.sizeMultiplier = sizeMultiplier;

        // Creating the rectangle
        rect = new(position.x, position.y, size.x * sizeMultiplier.x, size.y * sizeMultiplier.y);
        width = size.x;
        height = size.y;
        this.topLeftCorner = topLeftCorner;

        // Create attributes
        if (attributesToAdd != null)
        {
            foreach (string i in attributesToAdd)
            {
                AddAttribute(i);
            }
        }

        // Set parent
        this.parent = parent;

        // Set name
        this.type = type;

        // Add this to the parent's list of Objects
        parent.GameObjects.Add(this);
    }

    // For a cropped object with a different size
    public GameObject(Texture2D sprite, Rectangle cropRect, Vector2Int size, Vector2Int topLeftCorner, Vector2Int position, Vector2Int sizeMultiplier, string type, Level parent, List<string> attributesToAdd = null)
    {
        // Creating attributes, position, size, and sprite
        this.attributes = new();
        this.sprite = sprite;
        this.position = position;
        this.sizeMultiplier = sizeMultiplier;

        // Creating the rectangle
        rect = new(position.x, position.y, sprite.Width * sizeMultiplier.x, sprite.Height * sizeMultiplier.y);
        cropped = true;
        this.cropRect = cropRect;
        width = size.x;
        height = size.y;
        this.topLeftCorner = topLeftCorner;

        // Create attributes
        if (attributesToAdd != null)
        {
            foreach (string i in attributesToAdd)
            {
                AddAttribute(i);
            }
        }

        // Set parent
        this.parent = parent;

        // Set name
        this.type = type;

        // Add this to the parent's list of Objects
        parent.GameObjects.Add(this);
    }

    // For a cropped object
    public GameObject(Texture2D sprite, Rectangle cropRect, Vector2Int position, Vector2Int sizeMultiplier, string type, Level parent, List<string> attributesToAdd = null)
    {
        // Creating attributes, position, size, and sprite
        this.attributes = new();
        this.sprite = sprite;
        this.position = position;
        this.sizeMultiplier = sizeMultiplier;

        // Creating the rectangle
        rect = new(position.x, position.y, sprite.Width * sizeMultiplier.x, sprite.Height * sizeMultiplier.y);
        cropped = true;
        this.cropRect = cropRect;
        this.width = cropRect.Width;
        this.height = cropRect.Height;

        // Create attributes
        if (attributesToAdd != null)
        {
            foreach (string i in attributesToAdd)
            {
                AddAttribute(i);
            }
        }

        // Set parent
        this.parent = parent;

        // Set name
        this.type = type;

        // Add this to the parent's list of Objects
        parent.GameObjects.Add(this);

        // Check if its a tile and add it to the parents list of tiles
        if (type == "tile")
        {
            parent.parent.tiles.Add(this);
        }
    }

    public void AddAttribute(string componentName)
    {
        /*Here we use the assembly qualified name of the desired attribute, something like
        MirrorImage.GameObject.MovementAttribute, MirrorImage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null*/
        try {
            string attributeToInstantiate = $"GameObjectsComponentsLevels.Components.{componentName}, GameObjectsComponentsLevels, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            var attributeType = Type.GetType(attributeToInstantiate);
            var instantiatedAttribute = Activator.CreateInstance(attributeType, this);
            Component newAttribute = (Component)instantiatedAttribute;

            // Make sure that the Actor doesn't already have an attribute of this type
            foreach (Component attribute in attributes)
            {
                if (attribute.type == newAttribute.type)
                {
                    Console.WriteLine("You can't add an attribute that has already been added to this object!");
                    break;
                }

            }

            // Add the attribute which we just created
            attributes.Add(newAttribute);
        }
        catch {
            Console.WriteLine($"We couldn't find the attribute {componentName}! Sorry. :(");
        }
    }

    // Call a specific method of an attribute
    public void CallAttributeMethod(string attributeType, string method, object[] parameters = null)
    {
        // Loop through all attributes
        foreach (Component attribute in attributes)
        {
            // Check if the looped attribute matches the attribute passed in the parameter. Remember, there can be only one of each type of attribute per GameObject.
            if (attribute.type == attributeType)
            {
                // Invoke the method that the user passed
                try
                {
                    attribute.GetType().GetMethod(method).Invoke(attribute, parameters);
                }
                catch
                {
                    Console.WriteLine("We couldn't find the method " + method + " in the " + attributeType + " type.");
                }
                break;
            }
        }
    }

    // Edit a variable of an attribute
    public void SetAttributeVariable(string attributeType, string variable, object newVariable)
    {
        // Loop through all attributes
        foreach (Component attribute in attributes)
        {
            // Check if the looped attribute matches the attribute passed in the parameter. Remember, there can be only one of each type of attribute per GameObject.
            if (attribute.type == attributeType)
            {
                // Set the variable
                try
                {
                    attribute.GetType().GetField(variable).SetValue(attribute, newVariable);
                }
                catch
                {
                    Console.WriteLine("We couldn't find the variable " + variable + " in the " + attributeType + " type.");
                }
                break;
            }
        }
    }

    public object GetAttributeVariable(string attributeType, string variable)
    {
        foreach (Component attribute in attributes)
        {
            if (attribute.type == attributeType)
            {
                try
                {
                    return attribute.GetType().GetField(variable).GetValue(attribute);
                }
                catch
                {
                    Console.WriteLine("We couldn't find the variable " + variable + " in the " + attributeType + " type.");
                }
            }
        }
        return null;
    }

    public void Remove()
    {
        parent.GameObjects.Remove(this);
    }
}