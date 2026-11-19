namespace Lab3Json;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell())
		{
			Title = "Кафедральна бібліотека (JSON)",
			Width = 1400,
			Height = 850
		};
	}
}
