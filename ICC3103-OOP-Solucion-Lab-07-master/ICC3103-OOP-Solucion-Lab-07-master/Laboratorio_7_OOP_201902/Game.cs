using Laboratorio_7_OOP_201902.Cards;
using Laboratorio_7_OOP_201902.Enums;
using Laboratorio_7_OOP_201902.Static;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary; // Para serializar
using System.Runtime.Serialization; //Serializar
	

	

namespace Laboratorio_7_OOP_201902

{   [Serializable]
    public class Game
    {
        //Constantes
        private const int DEFAULT_CHANGE_CARDS_NUMBER = 3;

        //Atributos
        private Player[] players;
        private Player activePlayer;
        private List<Deck> decks;
        private List<SpecialCard> captains;
        private Board boardGame;
        internal int turn;

        //Constructor
        public Game()
        {
            Random random = new Random();
            decks = new List<Deck>();
            captains = new List<SpecialCard>();
            AddDecks();
            AddCaptains();
            players = new Player[2] { new Player(), new Player() };
            ActivePlayer = Players[random.Next(2)];
            boardGame = new Board();
            //Add board to players
            players[0].Board = boardGame;
            players[1].Board = boardGame;
            turn = 0;
        }
        //Propiedades
        public Player[] Players
        {
            get
            {
                return this.players;
            }
        }
        public Player ActivePlayer
        {
            get
            {
                return this.activePlayer;
            }
            set
            {
                activePlayer = value;
            }
        }
        public List<Deck> Decks
        {
            get
            {
                return this.decks;
            }
        }
        public List<SpecialCard> Captains
        {
            get
            {
                return this.captains;
            }
        }
        public Board BoardGame
        {
            get
            {
                return this.boardGame;
            }
        }


        //Metodos
        public bool CheckIfEndGame()
        {
            if (players[0].LifePoints == 0 || players[1].LifePoints == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int GetWinner()
        {
            if (players[0].LifePoints == 0 && players[1].LifePoints > 0)
            {
                return 1;
            }
            else if (players[1].LifePoints == 0 && players[0].LifePoints > 0)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
        public int GetRoundWinner()
        {
            if (Players[0].GetAttackPoints()[0] == Players[1].GetAttackPoints()[0])
            {
                return -1;
            }
            else
            {
                int winner = Players[0].GetAttackPoints()[0] > Players[1].GetAttackPoints()[0] ? 0 : 1;
                return winner;
            }
        }
        public void Play()
        {
            int userInput = 0;
            int firstOrSecondUser = ActivePlayer.Id == 0 ? 0 : 1;
            int winner = -1;
            bool bothPlayersPlayed = false;
            

            while (turn < 4 && !CheckIfEndGame())
            {
                bool drawCard = false;
                //turno 0 o configuracion
                if (turn == 0)
                {
                    for (int _ = 0; _<Players.Length; _++)
                    {
                        ActivePlayer = Players[firstOrSecondUser];
                        Visualization.ClearConsole();
                        //Mostrar mensaje de inicio
                        Visualization.ShowProgramMessage($"Player {ActivePlayer.Id+1} select Deck and Captain:");
                        //Preguntar por deck
                        Visualization.ShowDecks(this.Decks);
                        userInput = Visualization.GetUserInput(this.Decks.Count - 1);
                        Deck deck = new Deck();
                        deck.Cards = new List<Card>(Decks[userInput].Cards);
                        ActivePlayer.Deck = deck;
                        //Preguntar por capitan
                        Visualization.ShowCaptains(Captains);
                        userInput = Visualization.GetUserInput(this.Captains.Count - 1);
                        ActivePlayer.ChooseCaptainCard(new SpecialCard(Captains[userInput].Name, Captains[userInput].Type, Captains[userInput].Effect));
                        //Asignar mano
                        ActivePlayer.FirstHand();
                        //Mostrar mano
                        Visualization.ShowHand(ActivePlayer.Hand);
                        //Mostar opciones, cambiar carta o pasar
                        Visualization.ShowListOptions(new List<string>() { "Change Card", "Pass" }, "Change 3 cards or ready to play:");
                        userInput = Visualization.GetUserInput(1);
                        if (userInput == 0)
                        {
                            Visualization.ClearConsole();
                            Visualization.ShowProgramMessage($"Player {ActivePlayer.Id+1} change cards:");
                            Visualization.ShowHand(ActivePlayer.Hand);
                            for (int i = 0; i < DEFAULT_CHANGE_CARDS_NUMBER; i++)
                            {
                                Visualization.ShowProgramMessage($"Input the numbers of the cards to change (max {DEFAULT_CHANGE_CARDS_NUMBER}). To stop enter -1");
                                userInput = Visualization.GetUserInput(ActivePlayer.Hand.Cards.Count, true);
                                if (userInput == -1) break;
                                ActivePlayer.ChangeCard(userInput);
                                Visualization.ShowHand(ActivePlayer.Hand);
                            }
                        }
                        firstOrSecondUser = ActivePlayer.Id == 0 ? 1 : 0;
                    }
                    turn += 1;
                }
                //turnos siguientes
                else
                {
                    while (true)
                    {
                        ActivePlayer = Players[firstOrSecondUser];
                        //Obtener lifePoints
                        int[] lifePoints = new int[2] { Players[0].LifePoints, Players[1].LifePoints };
                        //Obtener total de ataque:
                        int[] attackPoints = new int[2] { Players[0].GetAttackPoints()[0], Players[1].GetAttackPoints()[0] };
                        //Mostrar tablero, mano y solicitar jugada
                        Visualization.ClearConsole();
                        Visualization.ShowBoard(boardGame, ActivePlayer.Id, lifePoints,attackPoints);
                        //Robar carta
                        if (!drawCard)
                        {
                            ActivePlayer.DrawCard();
                            drawCard = true;
                        }
                        Visualization.ShowHand(ActivePlayer.Hand);
                        Visualization.ShowListOptions(new List<string> { "Play Card", "Pass" }, $"Make your move player {ActivePlayer.Id+1}:");
                        userInput = Visualization.GetUserInput(1);
                        if (userInput == 0)
                        {
                            //Si la carta es un buff solicitar a la fila que va.
                            Visualization.ShowProgramMessage($"Input the number of the card to play. To cancel enter -1");
                            userInput = Visualization.GetUserInput(ActivePlayer.Hand.Cards.Count, true);
                            if (userInput != -1)
                            {
                                if (ActivePlayer.Hand.Cards[userInput].Type == EnumType.buff)
                                {
                                    Visualization.ShowListOptions(new List<string> { "Melee", "Range", "LongRange" }, $"Choose row to buff {ActivePlayer.Id}:");
                                    int cardId = userInput;
                                    userInput = Visualization.GetUserInput(2);
                                    if (userInput == 0)
                                    {
                                        ActivePlayer.PlayCard(cardId, EnumType.buffmelee);
                                    }
                                    else if (userInput == 1)
                                    {
                                        ActivePlayer.PlayCard(cardId, EnumType.buffrange);
                                    }
                                    else
                                    {
                                        ActivePlayer.PlayCard(cardId, EnumType.bufflongRange);
                                    }
                                }
                                else
                                {
                                    ActivePlayer.PlayCard(userInput);
                                }
                            }
                            //Revisar si le quedan cartas, si no le quedan obligar a pasar.
                            if (ActivePlayer.Hand.Cards.Count == 0)
                            {
                                firstOrSecondUser = ActivePlayer.Id == 0 ? 1 : 0;
                                break;
                            }
                        }
                        else
                        {
                            firstOrSecondUser = ActivePlayer.Id == 0 ? 1 : 0;
                            break;
                        }
                    }
                    //Cambiar al oponente si no ha jugado
                    if (!bothPlayersPlayed)
                    {
                        bothPlayersPlayed = true;
                        drawCard = false;
                    }
                    //Si ambos jugaron obtener el ganador de la ronda, reiniciar el tablero y pasar de turno.
                    else
                    {
                        winner = GetRoundWinner();
                        if (winner == 0)
                        {
                            Players[1].LifePoints -= 1;
                        }
                        else if (winner == 1)
                        {
                            Players[0].LifePoints -= 1;
                        }
                        else
                        {
                            Players[0].LifePoints -= 1;
                            Players[1].LifePoints -= 1;
                        }
                        bothPlayersPlayed = false;
                        turn += 1;
                        //Destruir Cartas
                        BoardGame.DestroyCards();
                    }
                }
            }
            //Definir cual es el ganador.
            winner = GetWinner();
            if (winner == 0)
            {
                Visualization.ShowProgramMessage($"Player 1 is the winner!");
            }
            else if (winner == 1)
            {
                Visualization.ShowProgramMessage($"Player 2 is the winner!");
            }
            else
            {
                Visualization.ShowProgramMessage($"Draw!");
            }

        }
        public void AddDecks()
        {
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent + @"\Files\Decks.txt";
            StreamReader reader = new StreamReader(path);
            int deckCounter = 0;
            List<Card> cards = new List<Card>();


            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string [] cardDetails = line.Split(",");

                if (cardDetails[0] == "END")
                {
                    decks[deckCounter].Cards = new List<Card>(cards);
                    deckCounter += 1;
                }
                else
                {
                    if (cardDetails[0] != "START")
                    {
                        if (cardDetails[0] == nameof(CombatCard))
                        {
                            cards.Add(new CombatCard(cardDetails[1], (EnumType) Enum.Parse(typeof(EnumType),cardDetails[2]), cardDetails[3], Int32.Parse(cardDetails[4]), bool.Parse(cardDetails[5])));
                        }
                        else
                        {
                            cards.Add(new SpecialCard(cardDetails[1], (EnumType)Enum.Parse(typeof(EnumType), cardDetails[2]), cardDetails[3]));
                        }
                    }
                    else
                    {
                        decks.Add(new Deck());
                        cards = new List<Card>();
                    }
                }

            }
            
        }
        public void AddCaptains()
        {
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent + @"\Files\Captains.txt";
            StreamReader reader = new StreamReader(path);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] cardDetails = line.Split(",");
                captains.Add(new SpecialCard(cardDetails[1], (EnumType)Enum.Parse(typeof(EnumType), cardDetails[2]), cardDetails[3]));
            }
        }
        // AHORA DEBO GUARDAR EL ESTADO DEL JUEGO
        public void Save()
        {
            FileStream gameState = new FileStream("gameStateDecks.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter saver1 = new BinaryFormatter();
            saver1.Serialize(gameState, this.decks);
            gameState.Close();

            FileStream gameState2 = new FileStream("gameStateBoard.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter saver2 = new BinaryFormatter();
            saver1.Serialize(gameState, this.boardGame);
            gameState.Close();

            FileStream gameState3 = new FileStream("gameStateCaptain.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter saver3 = new BinaryFormatter();
            saver1.Serialize(gameState, this.captains);
            gameState.Close();

            FileStream gameState4 = new FileStream("gameStatePlayer.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter saver4 = new BinaryFormatter();
            saver1.Serialize(gameState, this.players);
            gameState.Close();

            FileStream gameState5 = new FileStream("gameStateActiveP.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter saver5 = new BinaryFormatter();
            saver1.Serialize(gameState, this.activePlayer);
            gameState.Close();


            // De esta forma guardo 
        }
        public void loadState()
            //Asi cargo
        {
            //Deserializo la raiz
            FileStream gameState1 = new FileStream("gameStateDecks.txt", FileMode.Open, FileAccess.Read);
            BinaryFormatter saver1 = new BinaryFormatter();
            this.decks = (List<Deck>)saver1.Deserialize(gameState1);
            gameState1.Close();

            FileStream gameState2 = new FileStream("gameStateBoard.txt", FileMode.Open, FileAccess.Read);
            BinaryFormatter saver2 = new BinaryFormatter();
            this.boardGame = (Board)saver2.Deserialize(gameState2);
            gameState2.Close();

            FileStream gameState3 = new FileStream("gameStateCaptain.txt", FileMode.Open, FileAccess.Read);
            BinaryFormatter saver3 = new BinaryFormatter();
            this.captains = (List<SpecialCard>)saver3.Deserialize(gameState3);
            gameState3.Close();

            FileStream gameState4 = new FileStream("gameStatePlayer.txt", FileMode.Open, FileAccess.Read);
            BinaryFormatter saver4 = new BinaryFormatter();
            this.players = (Player[])saver4.Deserialize(gameState4);
            gameState4.Close();

            FileStream gameState5 = new FileStream("gameStateActiveP.txt", FileMode.Open, FileAccess.Read);
            BinaryFormatter saver5 = new BinaryFormatter();
            this.activePlayer = (Player)saver5.Deserialize(gameState5);
            gameState5.Close();



        }
    }

}
