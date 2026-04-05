namespace Fader;

public class Profile
{
    public string Name { get; set; } = "Default";
    public double GradientStart { get; set; } = 0.0;
    public double GradientEnd { get; set; } = 1.0;
    public bool Topmost { get; set; } = true;
    public bool Resizable { get; set; }
    public bool Draggable { get; set; }
}
