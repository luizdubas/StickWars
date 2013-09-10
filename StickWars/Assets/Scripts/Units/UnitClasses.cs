using System;

/// <summary>
/// Definiçao da classe campones, a classe mais basica do jogo
/// </summary>
public class PeasantClass : IUnitClass
{
	
	public string Name {
		get {
			return "Peasant";
		}
	}
	
	public int HP {
		get {
			return 10;
		}
	}

	public int Attack {
		get {
			return 1;
		}
	}

	public int Defense {
		get {
			return 1;
		}
	}

	public int AttackRange {
		get {
			return 1;
		}
	}
	
	public int AttackSpeed {
		get {
			return 2;
		}
	}

	public int MovementSpeed {
		get {
			return 2;
		}
	}

	public bool CanBuild {
		get {
			return true;
		}
	}

	public int SecondsToCreate {
		get {
			return 2;
		}
	}
	
	#region IUnitClass implementation
	
	public int MaterialCost (MaterialType material)
	{
		switch(material){
			case MaterialType.Circle:
				return 1;
			case MaterialType.Stick:
				return 5;
		}
		return 0;
	}

	public bool CanCollect ()
	{
		return true;
	}
	
	#endregion
}

