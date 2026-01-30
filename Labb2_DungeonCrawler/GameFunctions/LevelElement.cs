using Labb2_DungeonCrawler;
using Labb2_DungeonCrawler.GameFunctions;
using Labb2_DungeonCrawler.Log;
using Labb2_DungeonCrawler.Menu;
using Labb2_DungeonCrawler.State;
using MongoDB.Bson.Serialization.Attributes;
using System.Xml.Linq;

    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(typeof(Player), typeof(Rat), typeof(Snake), typeof(TheRatKing), typeof(TheKingsTail), typeof(Wall), typeof(Lazer))]
public abstract class LevelElement
{
    [BsonIgnore]
    protected GameState? Game { get; private set; }
    public int xCordinate { get; set; }
    public int yCordinate { get; set; }

    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public char Symbol { get; set; }
    [BsonIgnore]
    public ConsoleColor MyColor { get; set; }
    public string? Name { get; set; }
    public int TurnsPlayed { get; set; }
    public int XP { get; set; }
    [BsonIgnore]
    public Dice? AttackDice { get; set; }
    [BsonIgnore]
    public Dice? DefenceDice { get; set; }
    public int HP { get; set; }

    public void SetGame(GameState game)
    {
        Game = game;
    }


    public virtual string PrintUnitInfo() 
    {
        return "";
    }
    public static void LevelChoice(string playerName, GameState gameState, List<LevelModel> levels)
    {
        var options = new List<MenuOption>();
        for (int i = 0; i < levels.Count; i++)
        {
            options.Add(new MenuOption(levels[i].Name, levels[i].IsAccessable));
        }
        options.Add(new MenuOption("Generate level"));

        int index = MenuHelper.ShowMenu($"=== {playerName} ===", options, false);

        switch (index)
        {
            case -1:
                break;
            case 0:
            case 1:
                LoadLevel(gameState, levels, index, true);
                break;

            case 2:
                LoadLevel(gameState, levels, index, false);
                break;

            case 3:
                gameState.SetCurrentGame(LevelData.Load(RandomMap.GenerateMap()));
                gameState.MessageLog.MyLog.Add("generating a random level...");
                gameState.ActiveLevel = "*randomly generated map*";
                break;

        }
        Thread.Sleep(500);
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        int leftPos = (Console.WindowWidth - 23) / 2;
        int topPos = (Console.WindowHeight - 10) / 2;
        Console.SetCursorPosition(leftPos, topPos);
        string[] hints =
        {
            "move around by using the arrow keys",
            "did you know that the ratking has 2 tails?",
            "did you know that you can shoot la[z]er?"
        };

        if (index >= 0 && index <= 2)
        {
            Console.WriteLine($"loading {levels[index].Name.ToLower()} ...");
            ShowLevelMessage(hints[index], leftPos, topPos);
        }
        else if (index == 3)
        {
            Console.WriteLine("generating level...");
            ShowLevelMessage("hopefully there are no holes in the wall and", leftPos, topPos);
            Console.SetCursorPosition(leftPos - 8, topPos + 3);
            Console.Write("no indestructable rocks in the way");
        }

        Thread.Sleep(4000);
    }
    public static void ShowLevelMessage(string message, int leftPos, int topPos)
    {
        Console.SetCursorPosition(leftPos - 8, topPos + 2);
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(message);
    }

    public static void LoadLevel(GameState gameState, List<LevelModel> levels, int index, bool unlockNext)
    {
        gameState.SetCurrentGame(levels[index].Elements);
        gameState.MessageLog.MyLog.Add($"loading {levels[index].Name.ToLower()}...");
        gameState.ActiveLevel = $"{index + 1}";

        if (unlockNext)
        {
            levels[index + 1].IsAccessable = true;
        }
    }

    public void Draw()
    {
        int leftPos = (Console.WindowWidth - 60) / 2;
        int topPos = (Console.WindowHeight - 25) / 2;
        Console.SetCursorPosition(leftPos + xCordinate, topPos + yCordinate);
        Console.ForegroundColor = MyColor;
        Console.Write(Symbol);
    }
    public void Erase()
    {
        int leftPos = (Console.WindowWidth - 60) / 2;
        int topPos = (Console.WindowHeight - 25) / 2;
        Console.SetCursorPosition(leftPos + this.xCordinate, topPos + this.yCordinate);
        Console.Write(' ');
    }
    public double GetDistanceTo(Player player)
    {
        return Math.Sqrt(Math.Abs(Math.Pow(this.yCordinate - player.yCordinate, 2) + Math.Abs(Math.Pow(this.xCordinate - player.xCordinate, 2))));
    }
    public bool IsSpaceAvailable()
    {
        CoOrdinate targetSpace = new CoOrdinate(this);


        return Game != null && Game.CurrentState != null &&
               !Game.CurrentState.Any(k => k != this && k.yCordinate == targetSpace.YCord && k.xCordinate == targetSpace.XCord);
    }

    public void CollideAndConcequences(Player player)
    {
        var collider = GetCollider();
        string message;
        int leftPos;
        if (collider is not Wall && !(collider is Enemy && this is Enemy))
        {

            message = PrintFightresult(Fight(collider), collider, player);
            leftPos = (Console.WindowWidth - message.Length) / 2;
            Console.SetCursorPosition(0, 1);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(leftPos, 1);
            Console.WriteLine(message);

            if (collider.HP > 0)
            {
                message = collider.PrintFightresult(collider.Fight(this), this, player);
                leftPos = (Console.WindowWidth - message.Length) / 2;
                Console.SetCursorPosition(0, 2);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(leftPos, 2);
                Console.WriteLine(message);
            }

            Game.MessageLog.MyLog.Add(PrintUnitInfo());
            Game.MessageLog.MyLog.Add(collider.PrintUnitInfo());
        }
    }

    public LevelElement? GetCollider()
    {
        CoOrdinate targetSpace = new CoOrdinate(this);

        if (Game.CurrentState == null) return null;

        return Game.CurrentState
            .FirstOrDefault(k => k != this && k.xCordinate == targetSpace.XCord && k.yCordinate == targetSpace.YCord);
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
    public string PrintFightresult(int fightreturn, LevelElement enemy, Player player)
    {
        string logMessage;

        if (enemy is TheKingsTail)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            logMessage = $"{player.Name} attacked the Kings tail and it had no effect. You can't damage the tail";
        }
        else if (enemy is Lazer)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            logMessage = $"{this.Name} attacked the lazer and it had no effect. You can't damage the lazer";
        }
        else if (fightreturn != -1 && this is Lazer)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            logMessage = $"{player.Name} used {this.Name} on {enemy.Name} with {this.AttackDice} attack and {enemy.Name} defended with {enemy.DefenceDice}. Attack was successfull and did {fightreturn} damage";
        }
        else if (fightreturn != -1)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            logMessage = $"{this.Name} attacked {enemy.Name} with {this.AttackDice} and {enemy.Name} defended with {enemy.DefenceDice}. Attack was successfull and did {fightreturn} damage";
        }
        else if (this is Lazer)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            logMessage = $"{player.Name} used {this.Name} on {enemy.Name} with {this.AttackDice} attack and {enemy.Name} defended with {enemy.DefenceDice}. Attack failed and did no damage";
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            logMessage = $"{this.Name} attacked {enemy.Name} with {this.AttackDice} and {enemy.Name} defended with {enemy.DefenceDice}. Attack failed and did no damage";
        }
        Game.MessageLog.MyLog.Add(logMessage);
        return logMessage;
    }
}