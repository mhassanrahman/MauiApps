﻿namespace HelloWorldUsingMauiAppAcceleratorExt.Views;

public partial class SignInPage : ContentPage
{
	public SignInPage(SignInViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
