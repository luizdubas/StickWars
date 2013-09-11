using System;

public enum LayerConstants{
	BUILDINGS = 8,
	GROUND = 9,
	UNITS = 10,
	RESOURCES = 11
}

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


public static class ConstantProperties{
	public const float GROUND_POSITION = 6.019565f;
}