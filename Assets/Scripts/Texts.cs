﻿using System.Collections;
using UnityEngine;

public class Texts : Singleton<Texts> {
    // don't forget to init it !

    public static string[] Characters = new string[GameManager.nbCharacters];
    public static string[,] Actions = new string[GameManager.nbCharacters, GameManager.nbActionsPerCharacter];
    public static string[] CharactersSucess = new string[GameManager.nbCharacters];
    public static string[] CharactersFailure = new string[GameManager.nbCharacters];

    public static string CriticalSucess = "C'est une réussite totale !";
    public static string CriticalFailure = "C'est un échec complet !";

    public static string[] Threats = new string[GameManager.nbThreats];
    public static string[] ThreatPlus = new string[GameManager.nbThreats];
    public static string[] ThreatMinus = new string[GameManager.nbThreats];

    public void Init()
    {
        Characters[0] = "Joe le chauffeur";
        Characters[1] = "Nathan le mécano";
        Characters[2] = "Lara la gachette";

        Actions[0, 0] = "donne un bon coup de volaut.";
        Actions[0, 1] = "freine d'un coup sec.";
        Actions[0, 2] = "enfonce la pédale d'accélération.";
        Actions[0, 3] = "appuie sur un bouton mystérieux du tableau de bord ...";
        Actions[0, 4] = "suis des yeux la mouche qui s'est posé sur le pare-brise...";

        Actions[1, 0] = "donne un coup de lasso aux poursuivants !";
        Actions[1, 1] = "verse un peu de nitro dans le moteur de la jeep ...";
        Actions[1, 2] = "dégaine sa trousse à outils et tente de réparer se qu'il peut de la Jeep.";
        Actions[1, 3] = "brandit l'amulette secrette haut dans le ciel !";
        Actions[1, 4] = "trie les boulons et les vis éparpillés dans sa trousse à outils...";

        Actions[2, 0] = "tire un coup avec son fusil de chasse !";
        Actions[2, 1] = "jette une bouteille de nitro sur les poursuivants !";
        Actions[2, 2] = "tente de calmer la nitro...";
        Actions[2, 3] = "dégaine une énorme mitrailleuse qui se trouvait dans la Jeep !";
        Actions[2, 4] = "sort un mouchoir de sa poche et se met à nettoyer son fusil...";

        CharactersSucess[0] = "Un bon coup de volant de Joe sur ce coup !";
        CharactersSucess[1] = "Nathan manie la clef à molette comme un pro !";
        CharactersSucess[2] = "Lara a réussi à intimider le Caporal avec cette action !";

        CharactersFailure[0] = "Joe est complètement perdu, il ferait mieux de regarder la route !";
        CharactersFailure[1] = "Nathan s'emmèle dans ses outils, il ne sait plus quoi faire...";
        CharactersFailure[2] = "Lara glisse sur un fond d'huile dans la remorque, c'est pas son moment...";


        Threats[0] = "état de la Jeep";
        Threats[1] = "stabilité de la nitro";
        Threats[2] = "distance au tank";

        ThreatPlus[0] = "Le moteur de notre Jeep a pris un sale coup !";
        ThreatPlus[1] = "Le bidon de nitro s'agite de plus en plus... J'espère qu'il ne va pas exploser !";
        ThreatPlus[2] = "Vite, le tank se rapproche !";

        ThreatMinus[0] = "La jeep roule mieux tout d'un coup. Elle a l'air de reprendre du poil de la bête !";
        ThreatMinus[1] = "La nitro a l'air d'avoir reposer un peu.. On a la paix pour un petit moment !";
        ThreatMinus[2] = "On a repris pris un peu de distance sur le tank, continuez comme ça !";


    }
}