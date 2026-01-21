using Labb2_DungeonCrawler;
using Labb2_DungeonCrawler.GameFunctions;
using Labb2_DungeonCrawler.Log;
using Labb2_DungeonCrawler.State;



public abstract class LevelElement
{
    public int xCordinate { get; set; }
    public int yCordinate { get; set; }
    public char Symbol { get; set; }
    public ConsoleColor MyColor { get; set; }
    public string? Name { get; set; }
    public int TurnsPlayed { get; set; }
    public int XP { get; set; }
    public Dice? AttackDice { get; set; }
    public Dice? DefenceDice { get; set; }
    public int HP { get; set; }


    public virtual string PrintUnitInfo() 
    {
        return "";
    }
    public static void LevelChoice(GameState currentGameState)
    {
        ConsoleKeyInfo userChoice;
        bool validChoiceFlag = false;
        Console.ForegroundColor = ConsoleColor.Green;
        do
        {
            userChoice = Console.ReadKey(true);
            switch (userChoice.Key)
            {
                case ConsoleKey.D1:
                    Console.SetCursorPosition(15, 16);
                    Console.Write("press [1] to play level 1");
                    currentGameState.CurrentState = LevelData.Load("ProjectFiles\\Level1.txt");
                    validChoiceFlag = true;
                    break;
                case ConsoleKey.D2:
                    Console.SetCursorPosition(15, 17);
                    Console.Write("press [2] to play level 2");
                    currentGameState.CurrentState = LevelData.Load("ProjectFiles\\Level2.txt");
                    validChoiceFlag = true;
                    break;
                case ConsoleKey.D3:
                    Console.SetCursorPosition(15, 18);
                    Console.Write("press [3] to play level 3");
                    currentGameState.CurrentState = LevelData.Load("ProjectFiles\\Level3.txt");
                    validChoiceFlag = true;
                    break;
                case ConsoleKey.D4:
                    Console.SetCursorPosition(15, 19);
                    Console.Write("press [4] to generate a random level");
                    currentGameState.CurrentState = LevelData.Load(RandomMap.GenerateMap());
                    validChoiceFlag = true;
                    break;
            }
        } while (!validChoiceFlag);
        Thread.Sleep(500);
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(23, 10);
        switch(userChoice.Key)
        {
            case ConsoleKey.D1:
                Console.WriteLine("loading level 1...");
                Console.SetCursorPosition(15, 12);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("move around by using the arrow keys");
                break;
            case ConsoleKey.D2:
                Console.WriteLine("loading level 2...");
                Console.SetCursorPosition(15, 12);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("did you know that the ratking has 2 tails?");
                break;
            case ConsoleKey.D3:
                Console.WriteLine("loading level 3...");
                Console.SetCursorPosition(15, 12);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("did you know that you can shoot la[z]er?");
                break;
            case ConsoleKey.D4:
                Console.WriteLine("generating level...");
                Console.SetCursorPosition(15, 12);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("hopefully there are no holes in the wall and");
                Console.SetCursorPosition(18, 13);
                Console.Write("no indestructable rocks in the way");
                break;
        }
        Thread.Sleep(4000);
    }
    public void Draw()
    {
        Console.SetCursorPosition(xCordinate, yCordinate);
        Console.ForegroundColor = MyColor;
        Console.Write(Symbol);
    }
    public void Erase()
    {
        Console.SetCursorPosition(this.xCordinate, this.yCordinate);
        Console.Write(' ');
    }
    public double GetDistanceTo(Player player)
    {
        return Math.Sqrt(Math.Abs(Math.Pow(this.yCordinate - player.yCordinate, 2) + Math.Abs(Math.Pow(this.xCordinate - player.xCordinate, 2))));
    }
    public bool IsSpaceAvailable(GameState currentGameState)
    {
        CoOrdinate targetSpace = new CoOrdinate(this);


        return currentGameState.CurrentState != null &&
               !currentGameState.CurrentState.Any(k => k != this && k.yCordinate == targetSpace.YCord && k.xCordinate == targetSpace.XCord);
    }
    public void CollideAndConcequences(Player player, string logMessage, MessageLog messageLog)
    {
        var collider = this.GetCollider();
        if (collider is not Wall && !(collider is Enemy && this is Enemy))
        {
            Console.SetCursorPosition(0, 1);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, 1);
            PrintFightresult(Fight(collider), collider, player, logMessage, messageLog);
            if (collider.HP > 0) collider.PrintFightresult(collider.Fight(this), this, player, logMessage, messageLog);
            logMessage = this.PrintUnitInfo();
            messageLog.MyLog.Add(logMessage);
            logMessage = collider.PrintUnitInfo();
            messageLog.MyLog.Add(logMessage);
        }
    }
    public LevelElement? GetCollider(GameState currentGameState)
    {
        CoOrdinate targetSpace = new CoOrdinate(this);
        if (currentGameState.CurrentState == null)
            return null;
        return currentGameState.CurrentState.FirstOrDefault(k => k != this && k.xCordinate == targetSpace.XCord && k.yCordinate == targetSpace.YCord);
    }
    public int Attack(LevelElement enemy)
    {
        int attack = this.AttackDice.Throw();
        int defence = enemy.DefenceDice.Throw();
        int result = attack - defence;

        if (result > 0)
        {
            enemy.HP -= (result);
            return result;
        }
        else return -1;
    }
    public int Fight(LevelElement enemy)
    {

        if (enemy != this && (enemy is Enemy || enemy is Player))
        {
            return this.Attack(enemy);
        }
        else return -1;
    }
    public void PrintFightresult(int fightreturn, LevelElement enemy, Player player, string logMessage, MessageLog messageLog)
    {
        if (enemy is TheKingsTail)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            logMessage = $"{player.Name} attacked the Kings tail and it had no effect. You can't damage the tail";
            Console.WriteLine(logMessage);
        }
        else if (enemy is Lazer)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            logMessage = $"{this.Name} attacked the lazer and it had no effect. You can't damage the lazer";
            Console.WriteLine(logMessage);
        }
        else if (fightreturn != -1 && this is Lazer)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            logMessage = $"{player.Name} used {this.Name} on {enemy.Name} with {this.AttackDice} attack and {enemy.Name} defended with {enemy.DefenceDice}. Attack was successfull and did {fightreturn} damage";
            Console.WriteLine(logMessage);
        }
        else if (fightreturn != -1)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            logMessage = $"{this.Name} attacked {enemy.Name} with {this.AttackDice} and {enemy.Name} defended with {enemy.DefenceDice}. Attack was successfull and did {fightreturn} damage";
            Console.WriteLine(logMessage);
        }
        else if (this is Lazer)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            logMessage = $"{player.Name} used {this.Name} on {enemy.Name} with {this.AttackDice} attack and {enemy.Name} defended with {enemy.DefenceDice}. Attack failed and did no damage";
            Console.WriteLine(logMessage);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            logMessage = $"{this.Name} attacked {enemy.Name} with {this.AttackDice} and {enemy.Name} defended with {enemy.DefenceDice}. Attack failed and did no damage";
            Console.WriteLine(logMessage);
        }
        messageLog.MyLog.Add(logMessage);
    }



}