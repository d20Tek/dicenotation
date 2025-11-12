# Sample: DiceRoller - Win10/UWP

This sample code shows you how to use the d20Tek.DiceNotation library in your very own UWP application. The sample was simply created from creating a new VS UWP project, and then installing the d20Tek.DiceNotation NuGet package.

#### 1 - Dice Input

This code takes input from the UI for number of dice, dice sides, and modifier to build a Dice expression from it, and then roll the results.

``` csharp

    /// <summary>
    /// Click handler for the Roll button for basic dice definition.
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">event args</param>
    private void RollButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        // setup the dice expression
        DiceType diceType = (DiceType)this.DiceTypeCombobox.SelectedItem;
        var expression = DiceExpression.Create()
                                       .AddDice(diceType.DiceSides, (int)this.DiceNumberNumeric.Value)
                                       .AddConstant((int)this.DiceModifierNumeric.Value);

        // roll the dice and save the results
        DiceResult result = this.diceService.Roll(expression, this.dieRoller);
        this.DiceRollResults.Insert(0, result);
    }
```

#### 2 - Dice Expression Input

The other important method takes a dice expression string and uses the Dice class to parse and roll the corresponding dice.

``` csharp

    /// <summary>
    /// Click handler for the Roll button to handle dice expression rolls.
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">event args</param>
    private async void RollExpressionButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        try
        {
            // roll the dice (based on dice expression string) and save the results.
            DiceResult result = this.diceService.Roll(this.DiceExpressionTextbox.Text, this.dieRoller);
            this.DiceRollResults.Insert(0, result);
        }
        catch (Exception ex)
        {
            // if there's an error in parsing the expression string, show an error message.            string message = "There was a error parsing the dice expression: " +
                             this.DiceExpressionTextbox.Text +
                             "\r\nException Text: " + ex.Message +
                             "\r\nPlease correct the expression and try again.";

            MessageDialog dialog = new MessageDialog(message, "Dice Parsing Error");
            await dialog.ShowAsync();
        }
    }
```

And there is a DiceService that these methods use to do the work. That DiceService is no other than the IDice implementation from our d20Tek.DiceNotation library. Pretty straightforward.

#### 3 - DiceResult Value Converters

Also, notice in the MainPage.xaml file the definition of the results listview. It uses a couple of IValueConverters to format the DiceResult and ResultsList into the appropriate string representation. These converters are part of d20TEk.DiceNotation package. You can use them if you'd like to share the formatting. If not, you're free to display the results however you would like.

``` xaml

     <Page.Resources>
        <ResourceDictionary>
            <conv:DiceResultConverter x:Key="DiceResultConverter" />
            <conv:TermResultListConverter x:Key="TermResultListConverter" />
        </ResourceDictionary>
    </Page.Resources>

    ...
    
          <ListView x:Name="ResultsListview" Grid.Row="4" Padding="12,0" MaxWidth="640"
                  ItemsSource="{Binding DiceRollResults}">
            <ListView.ItemTemplate>
                <DataTemplate>
                    <Grid Padding="0,4">
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="auto" />
                            <ColumnDefinition Width="*" />
                        </Grid.ColumnDefinitions>
                        <Grid.RowDefinitions>
                            <RowDefinition Height="auto" />
                            <RowDefinition Height="auto" />
                        </Grid.RowDefinitions>
                        <TextBlock Grid.Row="0" Grid.Column="0" FontWeight="SemiBold" Text="Result:" />
                        <TextBlock Grid.Row="0" Grid.Column="1" FontWeight="SemiBold" Padding="4,0"
                                   Text="{Binding Converter={StaticResource DiceResultConverter}}" />
                        <TextBlock Grid.Row="1" Grid.Column="0" Text="Rolls:" />
                        <TextBlock Grid.Row="1" Grid.Column="1" Padding="4,0"
                                   Text="{Binding Results, Converter={StaticResource TermResultListConverter}}" />
                    </Grid>
                </DataTemplate>
            </ListView.ItemTemplate>
        </ListView>
```

There is some other boilerplate code for getting XAML and Windows 10 apps to work. Feel free to review it and use it in your own applications. 

*Note:* I didn't write this app using an MVVM library because I wanted to focus on the integration code for our library, not explaining MVVM. If this were real Windows app, I would have incorporated an MVVM framework.

**Related topics:**
* [Using IDieRollTracker](SampleWin10Tracker.md)
