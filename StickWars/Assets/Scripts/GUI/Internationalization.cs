using System;
using System.Collections.Generic;

public class Internationalization
{
	public static Dictionary<string,string> CurrentLanguage;

	static Dictionary<string,string> en_US = new Dictionary<string, string>();
	static Dictionary<string,string> pt_BR = new Dictionary<string, string>();

	public static void Initialize ()
	{
		en_US.Add ("new_game", "New Game");
		en_US.Add ("options", "Options");
		en_US.Add ("language", "Language");
		en_US.Add ("difficulty", "Difficulty");
		en_US.Add ("player", "Player");
		en_US.Add ("exit", "Exit");

		pt_BR.Add ("new_game", "Novo Jogo");
		pt_BR.Add ("options", "Opçoes");
		pt_BR.Add ("language", "Linguagem");
		pt_BR.Add ("difficulty", "Dificuldade");
		pt_BR.Add ("player", "Jogador");
		pt_BR.Add ("exit", "Sair");

		CurrentLanguage = en_US;
	}

	public static void ChangeLanguage(string language){
		if (language.Equals ("en-US"))
			CurrentLanguage = en_US;
		else
			CurrentLanguage = pt_BR;
	}

	public static string Get(string key)
	{
		return CurrentLanguage[key];
	}
}

