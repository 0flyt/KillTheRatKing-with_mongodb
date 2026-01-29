using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler.Menu;

public class MenuOption
{
    public string Text { get; set; }
    public bool IsEnabled { get; set; } = true;

    public MenuOption(string text, bool isEnabled = true)
    {
        Text = text;
        IsEnabled = isEnabled;
    }
}
