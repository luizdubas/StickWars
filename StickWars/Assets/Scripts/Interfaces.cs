using UnityEngine;
using System.Collections;

//Tipos de animaçao possiveis para os materiais
//Chop seria a animaçao para 'arvores'
//Mine e a animaçao para coletar ouro ou os circulos para criar os personagens
//Collect seria para coletar agua, que nao tera nesse
public enum AnimationType { Chop, Mine, Collect }

//Tipos de materiais possiveis no jogo
//Stick seriam as arvores, nela se coleta os pauzinhos para criar personagens e construçoes
//Circle e como o Gold no Age of Empires, so que nele se coleta os circulos necessarios para criar os personagens
public enum MaterialType { Stick, Circle, Gold }

public interface IMaterialSource
{
	void CollectSource(ref int quantity);
}
