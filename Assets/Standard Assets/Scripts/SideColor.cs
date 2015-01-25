using UnityEngine;
using System.Collections;

public class SideColor
{
	public int id;
	public Material material = null;
	
	public SideColor (int id, Material material)
	{
		this.material = material;
		this.id = id;
	}
	
	public bool Equals (SideColor other)
	{
		return other != null && this.id == other.id;
	}
}
