using System.Collections;
using UnityEngine;

public class Texts : Singleton<Texts>
{
    // don't forget to init it !

    public static string[] Characters = new string[GameManager.nbCharacters];
    public static string[,] Actions = new string[GameManager.nbCharacters, GameManager.nbActionsPerCharacter];
    public static string[] Items = new string[GameManager.nbItems];

    public static string[] CharactersSucess = new string[GameManager.nbCharacters];
    public static string[] CharactersFailure = new string[GameManager.nbCharacters];

    public static string CriticalSucess = "C'est un succès total !";
    public static string CriticalFailure = "C'est un échec critique !";

    public static string[] Threats = new string[GameManager.nbThreats];
    public static string[] ThreatPlus = new string[GameManager.nbThreats];
    public static string[] ThreatMinus = new string[GameManager.nbThreats];

    public static string ThreatStay = "aucune menace n'a évolué";

    public static string Has = "a un";
    public static string HandicapPlayer = "Son inaction gêne ses coéquipiers -";
    public static string Handicap = "handicap de";
    public static string Bonus = "bonus de";
    public static string HandicapResult = "% sur leur action !";

    public static string ScenarioResult = "%)";

    public static string PositiveAction = "Grâce à ";

    public static string NeutralAction1 = "L'action de";
    public static string NeutralAction2 = "n'a pas été très utile,";

    public static string ScenarioCriticalSucess = "Les Aventuriers ont brillament passé cette épreuve !";
    public static string ScenarioSucess = "Les Aventuriers se félicitent de leur succès...";
    public static string ScenarioFailure = "Malheureusement pour les Aventuriers, c'est un échec...";
    public static string ScenarioCriticalFailure = "Les Aventuriers essuient un échec monumental !";

    public static string TrapHasBeenPlayed = "Le Colonel a posé un piège.";
    public static string ThreatDanger = "pourrait se dégrader !";

    public static string TrapAvoided = "Heureusement, le piège a été évité !";

    public static string ObjectHaBeenPlayed = "Les Aventuriers jouent un objet !";


    public static string Turn = "Tour";
    public static string TurnsLeft1 = "Dans";
    public static string TurnsLeft2 = "tours, les Aventuriers échapperont au Colonel !";

    public static string PickCard = "Pioche";
    public static string PickCardInstructions = "- Colonel : 2 cartes SCENARIO + 2 cartes OBJET\n- Aventuriers : 2 cartes ACTION";

    public void Init()
    {
        Characters[0] = "Tom";
        Characters[1] = "Paul";
        Characters[2] = "Laura";

        Actions[0, 0] = "fait des zigzags.";
        Actions[0, 1] = "donne un coup de frein.";
        Actions[0, 2] = "accélère...";
        Actions[0, 3] = "appuie sur le bouton inconnu !";
        Actions[0, 4] = "ne sait pas quoi faire...";

        Actions[1, 0] = "donne un coup de lasso.";
        Actions[1, 1] = "verse de la Nitro dans le réservoir.";
        Actions[1, 2] = "tente de réparer la Jeep...";
        Actions[1, 3] = "invoque l'esprit d'Opayan !";
        Actions[1, 4] = "ne fait rien de particulier...";

        Actions[2, 0] = "tire au fusil.";
        Actions[2, 1] = "balance de la Nitro.";
        Actions[2, 2] = "calme la Nitro...";
        Actions[2, 3] = "tire à la mitrailleuse lourde !";
        Actions[2, 4] = "regarde agir ses coéquipiers...";

        Items[0] = "Enfer ! Une mouche se met à tourner autour de Tom et l'empêche de manoeuvrer correctement";
        Items[1] = "Manque de chance, le soleil éblouit Laura et la gêne dans son action";
        Items[2] = "Aïe ! Paul a marché sur un caillou. Arrivera t'il à réagir malgré la douleur ?";
        Items[3] = "Soudain, un clou fait éclater un pneu. Si la Jeep ralentit, le Tank la rattrapera !";
        Items[4] = "Paul glisse sur un savon et percute le bidon de Nitro. Pourvu que la Nitro ne subisse pas d'autre choc !";
        Items[5] = "Damned ! La Jeep percute un gros rocher. Pourvu que la Jeep ne subisse pas d'autres dégâts !";
        Items[6] = "Haha mon piège de test va influencer le perso 0 !";


        CharactersSucess[0] = "Bravo, Tom !";
        CharactersSucess[1] = "Quelle action de Paul !";
        CharactersSucess[2] = "Bien jouée, Laura !";

        CharactersFailure[0] = "Il a fait un bien mauvais choix...";
        CharactersFailure[1] = "Il fera sans doute mieux la prochaine fois...";
        CharactersFailure[2] = "Elle rate son coup cette fois-ci...";


        Threats[0] = "L'état de la Jeep";
        Threats[1] = "La stabilité de la Nitro";
        Threats[2] = "La distance au Tank";

        ThreatPlus[0] = "Le moteur de la Jeep va casser !";
        ThreatPlus[1] = "Le bidon de Nitro s'agite de plus en plus... J'espère qu'il ne va pas exploser !";
        ThreatPlus[2] = "Le Tank se rapproche dangereusement, il faut fuir !";

        ThreatMinus[0] = "la Jeep peut encore rouler !";
        ThreatMinus[1] = "la Nitro ne va pas exploser (pas tout de suite)";
        ThreatMinus[2] = "la Jeep s'éloigne du Tank.";


    }
}
