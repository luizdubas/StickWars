using UnityEngine;
using System.Collections;

/*
 * Interfaces.cs
 * 
 * Tentar usar esse arquivo para criar as interfaces que serao comuns em jogos que fizermos
 * desse mesmo tipo
 * 
 */


/// <summary>
///Tipos de animaçao possiveis para os materiais
///Chop seria a animaçao para 'arvores'
///Mine e a animaçao para coletar ouro ou os circulos para criar os personagens
///Collect seria para coletar agua, que nao tera nesse
/// </summary>
public enum AnimationType { Chop, Mine, Collect }

/// <summary>
///Tipos de materiais possiveis no jogo
///Stick seriam as arvores, nela se coleta os pauzinhos para criar personagens e construçoes
///Circle e como o Gold no Age of Empires, so que nele se coleta os circulos necessarios para criar os personagens
/// </summary>
public enum MaterialType { Stick, Circle, Gold }

public interface IMaterialSource
{
	/// <summary>
	/// Coleta o material dessa fonte
	/// </summary>
	/// <param name='quantity'>
	/// Quantidade de material que sera coletado
	/// </param>
	void CollectSource(ref int quantity);
}

public interface IUnit
{
	int ID { get; }
	Player Owner { get; set; }
	bool Selected { get; set; }
	GameObject ParentObject { get; }
	int HP { get; set; }
	IUnitClass UnitClass { get; set; }
	void MoveTo(Vector3 point);
	void StopMovement();
	void AttackMode();
	void DefenseMode();
	void AttackUnit(IUnit unit);
	void AttackBuilding(IBuilding building);
	void Build(IBuilding building);
	void Destroy(IBuilding building);
	void SetColor(Color playerColor);
}

public interface IUnitClass
{
	string Name { get; }
	int HP { get; }
	int Attack { get; }
	int Defense { get; }
	int AttackRange { get; }
	int AttackSpeed { get; }
	int MovementSpeed { get; }
	int SecondsToCreate { get; }
	bool CanBuild { get; }
	int MaterialCost(MaterialType material);
	bool CanCollect(MaterialType material);
}

public interface IBuilding
{
	string Name { get; }
	int HP { get; }
	float SecondsToCreate { get; }
	Player Owner { get; set; }
	Vector3 BuildingPosition { get; }
	GameObject ParentObject { get; }
	IUnit CreateUnit();
	IUnit UpgradeUnit();
	void DrawGUI();
}
