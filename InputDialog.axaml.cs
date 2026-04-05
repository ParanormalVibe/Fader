using Avalonia.Controls;

namespace Fader;

public partial class InputDialog : Window
{
    public InputDialog() : this("Enter value:", "") { }

    public InputDialog(string prompt, string defaultValue)
    {
        InitializeComponent();
        PromptText.Text = prompt;
        InputBox.Text = defaultValue;
        InputBox.SelectAll();

        OkButton.Click += (_, _) => Close(InputBox.Text?.Trim());
        CancelButton.Click += (_, _) => Close(null as string);
    }
}
