using System.Numerics;
using System.Text.RegularExpressions;

bool game_running = true;
int round = 0;
Gladiator glad1 = new(), glad2 = new();
float gameRelativeSpeed = 0.5f;

Console.CursorVisible = false;
Console.CancelKeyPress += (sender, e) => { Quit(); };

Thread inputThread = new Thread(handleInput);
inputThread.Start();

// Main loop
while (game_running)
{
  logick();
}

inputThread.Join();
Quit();

void logick()
{
  Console.Clear();


  printGladiators(glad1, glad2);

  Console.WriteLine($"========⚔️FIGHT SPEED {1 + gameRelativeSpeed:F2}x⚔️========");
  Console.WriteLine($"             Round №{round}");

  if (glad1.alive && glad2.alive)
  {
    round++;
    fight(glad1, glad2);
  }
  else
  {
    Console.SetCursorPosition(13, 7);
    Console.Write("Winner: ");
    if (!glad1.alive && !glad2.alive)
    {
      Console.WriteLine("Draw!");
    }
    else if (!glad1.alive)
    {
      Console.WriteLine($"{glad2}");
    }
    else if (!glad2.alive)
    {
      Console.WriteLine($"{glad1}");
    }

  }
  printControls();

  Thread.Sleep(1000 - (int)(gameRelativeSpeed * 1000));
}

void printGladiators(Gladiator g1, Gladiator g2)
{
  ref Gladiator gladiator = ref g1;
  int[] positions = [1, 20];

  foreach (int pos in positions)
  {
    if (pos == 20) { gladiator = ref g2; }

    Console.SetCursorPosition(pos, 0);
    Console.Write($"{gladiator.name}({gladiator.status})");

    Console.SetCursorPosition(pos, 1);
    gladiator.PrintHealthUI();
    Console.SetCursorPosition(pos, 2);
    gladiator.PrintArmorUI();

    Console.SetCursorPosition(pos, 3);
    Console.WriteLine($"   💪 {gladiator.damage}  🦶 {gladiator.dodgeChanse}");
  }

}

void printControls()
{
  string text = $"R:Restart | [K, J]:Speed | [Esc, Q]:Quit";
  string border = new('─', text.Length - 2);

  Console.SetCursorPosition(0, 10);
  Console.WriteLine($" ╭{border}╮\n {text}\n ╰{border}╯");
}

void handleInput()
{
  while (true)
  {
    if (Console.KeyAvailable)
    {
      ConsoleKey key = Console.ReadKey(true).Key;
      switch (key)
      {
        case ConsoleKey.R: NewGame(); break;
        case ConsoleKey.K: gameRelativeSpeed = Math.Min(gameRelativeSpeed + 0.01f, 1); break;
        case ConsoleKey.J: gameRelativeSpeed = Math.Max(gameRelativeSpeed - 0.01f, -1); break;

        case ConsoleKey.Escape:
        case ConsoleKey.Q: Quit(); return;
        default: break;
      }
    }
    Thread.Sleep(100);
  }
}

void NewGame()
{
  glad1 = new(); glad2 = new();
  round = 0;
}

void Quit()
{
  game_running = false;
  Console.ResetColor(); Console.Clear();
  Console.CursorVisible = true;
}


void fight(Gladiator g1, Gladiator g2)
{
  g1.Attack(g2); g2.Attack(g1);
}


class Gladiator
{
  static readonly string[] NAMES = ["Aiden", "Alan", "Alastor", "Alex", "Alexander", "Alius", "Amaranth", "Amos", "Ananias", "Anders", "Andrew", "Angus", "Anthony", "Antony", "Archie", "Arthur", "Ashton", "Axel", "Bailey", "Baker", "Benjamin", "Bernard", "Blake", "Bodhi", "Brandon", "Bram", "Brendan", "Brian", "Briar", "Bristol", "Brody", "Bruce", "Cameron", "Carl", "Carlos", "Carter", "Chad", "Charlie", "Christopher", "Claude", "Cody", "Colin", "Conner", "Cooper", "Corey", "Craig", "Cristian", "Cruz", "Damian", "Daniel", "Danny", "Danton", "Darren", "David", "Diego", "Dominic", "Donovan", "Dylan", "Edmund", "Edward", "Elijah", "Elliot", "Ellis", "Emmett", "Eric", "Ethan", "Eugene", "Felix", "Finn", "Fisher", "Flynn", "Ford", "Francis", "Frank", "Frederick", "Gabriel", "Garret", "George", "Gordon", "Gregory", "Gus", "Harrison", "Hayes", "Henry", "Herbie", "Herman", "Hilton", "Holden", "Hugo", "Hunter", "Ian", "Isaac", "Jack", "Jackson", "Jacob", "Jaden", "Jayden", "Jedidiah", "Jesse", "Jethro"];
  static readonly string[] STATUSES = ["😵", "🤧", "🤩", "🤪", "🤫", "🤬", "🤭", "🤮", "🤯", "🤠", "🤡", "🤢", "🤣", "🤤", "🤥", "🤒", "🤓", "🤔", "🤕", "🤗", "🙂", "😊", "😀", "😁", "😃", "😄", "😎", "😆", "😂", "☹️", "🙁", "😞", "😟", "😣", "😖", "😢", "😭", "🥲", "🥹", "😂", "😠", "😡", "😨", "😧", "😦", "😱", "😫", "😩", "😮", "😯", "😲", "😗", "😙", "😚", "😘", "😍", "😉", "😜", "😛", "😝", "🤑", "🫤", "🤔", "😕", "😟", "😐", "😑", "😳", "😞", "😖", "🤐", "😶", "😇", "👼", "😈", "😎", "😪", "😏", "😒", "😵‍💫", "😕", "🤕", "🤒", "😷", "🤢", "🤨", "😬"];
  static readonly string[] HEALTH_STATUS = ["☠️", "💔", "❤️"];

  public bool alive = true;
  public readonly string name = "";
  public string status = STATUSES[0];
  public int health = 99;
  public int armor = Random.Shared.Next(80, 100);
  public int damage = Random.Shared.Next(40, 50);
  public int dodgeChanse = Random.Shared.Next(50, 90);

  public Gladiator()
  {
    name = NAMES[Random.Shared.Next(0, NAMES.Length)];
    ChangeStatus();
  }
  public override string ToString()
  {
    return $"{name}({status})";
  }

  public void PrintHealthUI()
  {
    int num = health / 10;
    int remains = 10 - num;

    Console.Write($"{GetHealthStatus()} {health:D2}[");
    Console.BackgroundColor = ConsoleColor.DarkRed;
    Console.Write(new string(' ', num));
    Console.ResetColor();
    Console.Write(new string(' ', remains));
    Console.Write("]");
  }
  public void PrintArmorUI()
  {
    int num = armor / 10;
    int remains = 10 - num;

    Console.Write($"🛡️ {armor:D2}[");
    Console.BackgroundColor = ConsoleColor.DarkBlue;
    Console.Write(new string(' ', num));
    Console.ResetColor();
    Console.Write(new string(' ', remains));
    Console.Write("]");
  }

  public string GetHealthStatus()
  {
    if (!alive) return HEALTH_STATUS[0];
    if (health < 40) return HEALTH_STATUS[1];
    return HEALTH_STATUS[2];
  }
  public void ChangeStatus()
  {
    if (!alive) { status = STATUSES[0]; return; }
    status = STATUSES[Random.Shared.Next(0, STATUSES.Length)];
  }
  public void TakeDamage(int damage)
  {
    if (dodgeChanse >= Random.Shared.Next(0, 100))
    {
      Console.WriteLine($"      {name} dodged the attack!");
      if (dodgeChanse > 0)
      {
        int dodge_damage = Math.Max(dodgeChanse / 10, 1);
        dodgeChanse = Math.Max(dodgeChanse - dodge_damage, 0);
      }
      return;
    }

    if (armor > 0)
    {
      int armor_damage = Math.Max(damage / 4, 1);
      armor = Math.Max(armor - armor_damage, 0);
      damage = Math.Max(damage / 10, 1);
    }

    health = Math.Max(health - damage, 0);
    if (health == 0) { alive = false; }

    ChangeStatus();
  }
  public void Attack(Gladiator target)
  {
    int spread = damage / 10;
    int punch = Random.Shared.Next(damage - spread, damage + spread);

    Console.WriteLine($" {name} punches with {punch} damage. ");
    target.TakeDamage(punch);
    if (damage > 1) { damage--; }
  }
}