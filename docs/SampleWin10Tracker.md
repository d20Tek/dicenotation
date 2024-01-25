# Sample - Win10/UWP (Using IDieRollTracker)
As part of a recent release, we add the ability to gather statistical frequency data of die rolls based on the IDieRoller used, the type of dice that were used, and instances of each roll result. This helps show the randomness of some of the random number algorithms, and allows application developers to use the library to show that frequency data to users.

The responsibility for capturing and querying data in memory is on the d20Tek.DiceNotation Library. Developers can disable die roll tracking by not providing one to the IDieRoller constructor. App developers are responsible for visualization of the frequency data and persisting that data between application instance (if desired). 

The interface we focus on for these capabilities is the IDieRollTracker interface.

### Setting Up IDieRollers To Use Roll Tracker
Create a single instance of the DieRollTracker, and then pass it into the IDieRoller that you create:

``` csharp
    IDieRollTracker diceTracker = new DieRollTracker();
    IDieRoller roller = null;
    roller = new RandomDieRoller(tracker);
    DiceResult result = new Dice().Roll("d20+3");
```

Passing a DieRollTracker into the constructor makes it so the IDieRoller uses it for every die roll (even if it is dropped from the final result). If you do not wish to track die roll frequency, then just pass in null for the DieRollTracker. There is overhead in tracking the data, so if you're not going to use that data, it's best to not provide a Tracker.

You can also set how much tracking data is held in memory by using the DieRollTracker.TrackerDataLimit. The default value is 250000 rolls are tracked.

``` csharp
    diceTracker.TrackerDataLimit = 25000;
```

### Querying for Roll Tracker Data
Once you make some rolls with the frequency tracker enabled, you can then query the data for display. We use an IList implementation and Linq, so that you can easily build queries to structure and filter the data the way you want. In our sample application, we use the following code to query frequency data for display:

``` csharp
    /// <summary>
    /// Called when navigated to this page.
    /// </summary>
    /// <param name="e">event args</param>
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        this.UpdateBusyProgress(true);
        this.frequencyData = await new DiceFrequencyTracker().GetFrequencyDataViewAsync();

        this.DataContext = this;

        if (this.RollerTypes.Count == 1)
        {
            this.RollerTypesCombobox.SelectedIndex = 0;
        }

        this.UpdateBusyProgress(false);
    }

    /// <summary>
    /// Updates the display state of the busy progress indicator.
    /// </summary>
    /// <param name="busy">Is the app busy</param>
    private void UpdateBusyProgress(bool busy)
    {
        this.BusyProgressBar.Visibility = busy ? Visibility.Visible : Visibility.Collapsed;
        this.ShowStatsButton.IsEnabled = !busy;
    }

    /// <summary>
    /// Updates the frequency data based on the filters selected
    /// on this page.
    /// </summary>
    private void UpdateFrequencyData()
    {
        string selectedRollerType = this.RollerTypesCombobox.SelectedValue as string;
        string selectedDiceSides = this.DiceSidesCombobox.SelectedValue as string;

        var list = from d in this.frequencyData
                   where d.RollerType == selectedRollerType && d.DieSides == selectedDiceSides
                   select d;

        this.Items = list.ToList();
        this.StatsListView.ItemsSource = this.Items;
    }
```

Note that all of the performance intensive data retrieval methods are Async, so that callers can make these calls asynchronously and still keep their applications responsive.

### Persisting Tracker Data
Then the application writer may choose to persist the roll tracking data a particular intervals (or when the app is closing), so that there is longer tracking of frequency data. This is left up to the application to do, but there are a couple of methods that help with frequency data persistence: ToJsonAsync and LoadFromJsonAsync.

To save the frequency data to a text file, you can do the following:

``` csharp
    string jsonText = await this.diceFrequencyTracker.ToJsonAsync();
    await this.fileService.WriteFileAsync(Constants.DieFrequencyDataFilename, jsonText);
```

To then load the frequency data back into our service's memory, you can:

``` csharp
      // load any cached dice frequency data.
      AppServices appServices = AppServices.Instance;
      string jsonText = await appServices.FileService.ReadFileAsync(Constants.DieFrequencyDataFilename);
      await appServices.DiceFrequencyTracker.LoadFromJsonAsync(jsonText);
```

You can learn more about the actual file saving code by looking at the TextFileService class in the sample.
