using Lab3Json.Services;

namespace Lab3Json.Pages;

/// <summary>Форма «Про програму»: ПІБ, курс, група, рік, короткий опис.</summary>
public partial class AboutPage : ContentPage
{
    public AboutPage()
    {
        InitializeComponent();

        NameLabel.Text = AuthorInfo.FullName;
        CourseLabel.Text = AuthorInfo.Course;
        GroupLabel.Text = AuthorInfo.Group;
        YearLabel.Text = AuthorInfo.Year;
        DescriptionLabel.Text = AuthorInfo.Description;
    }

    private async void OnCloseClicked(object? sender, EventArgs e) => await Navigation.PopModalAsync();
}
