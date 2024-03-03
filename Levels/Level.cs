using System;
using System.Collections.Generic;

using GameObjectsComponentsLevels.Helpers;
using GameObjectsComponentsLevels.GameObjects;

namespace GameObjectsComponentsLevels.Levels;
public class Level
{
	public virtual List<GameObject> GameObjects { get; set; }
	public virtual List<object> UIElements { get; set; }

	public virtual string tiledMap { get; set; }
	public GameManager parent;
	public int index;

	public Level(GameManager parent, bool respawn)
	{
		GameObjects = new();
		this.parent = parent;

		if (!respawn)
		{
			parent.levels.Add(this);
			index = parent.levels.IndexOf(this);
		}
	}

	public virtual void Update(){ }
	public virtual void Initialize() { }

	public virtual void playerRespawn() { }

	public void Remove()
	{
		parent.levels.Remove(this);
	}
}