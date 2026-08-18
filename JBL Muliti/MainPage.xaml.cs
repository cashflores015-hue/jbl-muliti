#if WINDOWS
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
#endif
using Microsoft.Maui.Graphics;

namespace JBL_Muliti;

public partial class MainPage : ContentPage
{
    bool isPlaying = false;

    public MainPage()
    {
        InitializeComponent();

        VolumeSlider.ValueChanged += (sender, e) =>
        {
            VolumeLabel.Text = $"Volume: {(int)e.NewValue}%";
        };
    }

    private async void OnFindSpeakerClicked(object sender, EventArgs e)
    {
        FindSpeakerButton.Text = "SEARCHING...";
        FindSpeakerButton.IsEnabled = false;

#if WINDOWS
        try
        {
            string selector = BluetoothDevice.GetDeviceSelectorFromPairingState(true);
            var devices = await DeviceInformation.FindAllAsync(selector);

            var names = devices
                .Where(device => !string.IsNullOrWhiteSpace(device.Name))
                .Select(device => device.Name)
                .Distinct()
                .ToArray();

            if (names.Length == 0)
            {
                await DisplayAlert(
                    "No paired devices found",
                    "Pair your JBL speaker in Windows Bluetooth settings first, then try again.",
                    "OK");

                FindSpeakerButton.Text = "FIND SPEAKER";
                FindSpeakerButton.IsEnabled = true;
                return;
            }

            string selected = await DisplayActionSheetAsync(
                "Choose your speaker",
                "Cancel",
                null,
                names);

            if (string.IsNullOrEmpty(selected) || selected == "Cancel")
            {
                FindSpeakerButton.Text = "FIND SPEAKER";
                FindSpeakerButton.IsEnabled = true;
                return;
            }

            StatusLabel.Text = "CONNECTED SPEAKER";
            StatusLabel.TextColor = Color.FromArgb("#55DD55");
            SpeakerNameLabel.Text = selected;
            BatteryLabel.Text = "Bluetooth device selected";
            BatteryLabel.TextColor = Color.FromArgb("#55DD55");

            FindSpeakerButton.Text = "CONNECTED ✓";
            ControlsLayout.IsEnabled = true;
            EqualizerGrid.IsEnabled = true;
            AddSpeakerButton.IsEnabled = true;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Bluetooth error", ex.Message, "OK");
            FindSpeakerButton.Text = "FIND SPEAKER";
            FindSpeakerButton.IsEnabled = true;
        }
#else
        await DisplayAlert(
            "Windows required",
            "Run using Windows Machine to find Bluetooth devices.",
            "OK");

        FindSpeakerButton.Text = "FIND SPEAKER";
        FindSpeakerButton.IsEnabled = true;
#endif
    }

    private void OnPlayClicked(object sender, EventArgs e)
    {
        isPlaying = !isPlaying;
        PlayButton.Text = isPlaying ? "⏸  PAUSE" : "▶  PLAY";
    }

    private async void OnPresetClicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        await DisplayAlert("Equalizer", $"{button.Text} is now active.", "OK");
    }

    private async void OnAddSpeakerClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Add speaker", "Searching for another speaker...", "OK");
    }
}