using MarqMetrix.Communications;
using MarqMetrix.Communications.Client;
using MarqMetrix.Communications.Client.Direct;
using MarqMetrix.Communications.Instruments;
using MarqMetrix.Communications.Samples;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool _darkSubtraction;
        private bool _intensityCorrection;
        private bool _ramanShift;
        private IClientContext _clientContext;
        private IComputationDependencyInfo _computationDependencies;
        private bool _loaded;
        private PlotModel _plotModel;

        public MainPage()
        {
            this.InitializeComponent();

            Loaded += PageLoaded;
            Unloaded += PageUnloaded;

            PlotView.Model = _plotModel = new PlotModel();
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            _loaded = true;
        }

        private async void PageUnloaded(object sender, RoutedEventArgs e)
        {
            await _clientContext?.CloseAsync();
            _clientContext?.Dispose();
            _loaded = false;
        }

        private void UpdatePlot()
        {
            SampleComputationFlags computation = SampleComputationFlags.None;

            if (_darkSubtraction)
                computation = computation | SampleComputationFlags.DarkSubtraction;

            if (_intensityCorrection)
            {
                computation = computation | SampleComputationFlags.IntensityCorrection;
            }

            IComputedSample computedSample = _clientContext.ComputeSample(_computationDependencies, computation);

            _plotModel.Series.Clear();

            LineSeries lineSeries = new LineSeries();

            _plotModel.Axes.Clear();
            _plotModel.Series.Add(lineSeries);

            _plotModel.Axes.Add(new LinearAxis { Key = "sigY", Position = AxisPosition.Left, Title = "Counts" });
            if (_ramanShift)
            {
                _plotModel.Axes.Add(new LinearAxis { Key = "sigX", Position = AxisPosition.Bottom, Title = "Raman Shift" });

                for (int i = 0; i < computedSample.Data.Length; i++)
                    lineSeries.Points.Add(new DataPoint(computedSample.RamanShiftData[i], computedSample.Data[i]));
            }
            else
            {
                _plotModel.Axes.Add(new LinearAxis { Key = "sigX", Position = AxisPosition.Bottom, Title = "Wavelength" });

                for (int i = 0; i < computedSample.Data.Length; i++)
                    lineSeries.Points.Add(new DataPoint(computedSample.WavelengthData[i], computedSample.Data[i]));
            }

            PlotView.InvalidatePlot();
        }

        private void DarkSubtractionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _darkSubtraction = true;
            if (!_loaded)
                return;
            UpdatePlot();
        }

        private void DarkSubtractionCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _darkSubtraction = false;
            if (!_loaded)
                return;
            UpdatePlot();
        }

        private void IntensityCorrectionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _intensityCorrection = true;
            if (!_loaded)
                return;
            UpdatePlot();
        }

        private void IntensityCorrectionCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _intensityCorrection = false;
            if (!_loaded)
                return;
            UpdatePlot();
        }

        private void RamanShiftRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            _ramanShift = true;
            if (!_loaded)
                return;
            UpdatePlot();
        }

        private void WavelengthRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            _ramanShift = false;
            if (!_loaded)
                return;
            UpdatePlot();
        }

        private async Task ConnectAsync()
        {
            if (_clientContext != null)
            {
                await _clientContext.CloseAsync();
                _clientContext.Dispose();
            }

            string host = HostTextBox.Text;
            int port = int.Parse(PortTextBox.Text);
            bool useHttps = UseHttpsCheckBox.IsChecked == true;
            string apiKey = ApiKeyTextBox.Text;
            _clientContext = ClientContext.Factory.CreateDirectClientContext(new DirectOptions
            {
                HostName = host,
                Port = port,
                UseHttps = useHttps,
                ApiKey = apiKey
            });

            await _clientContext.OpenAsync();

        }

        private async void ConnectClick(object sender, RoutedEventArgs e)
        {
            await ConnectAsync();
        }

        private async void AcquireClick(object sender, RoutedEventArgs e)
        {
            if (_clientContext == null)
                await ConnectAsync();

            ICollectionResult<IInstrumentInfo> instruments = await _clientContext.GetInstrumentsAsync();
            IInstrumentInfo instrument = instruments.Items.First();

            ISampleInfo sampleInfo = await _clientContext.AcquireSampleAsync(instrument.Id, new ExtendedSampleAcquisitionOptions
            {
                DarkSampleOptions = DarkSampleOptions.NewDark,
                IntegrationTime = TimeSpan.Parse(IntegrationTimeTextBox.Text),
                LaserPower = int.Parse((string)((ComboBoxItem)LaserPowerComboBox.SelectedItem).Content),
                SampleAverageCount = int.Parse(AverageSamplesText.Text)
            });

            _computationDependencies = await _clientContext.GetComputationDependencyInfoAsync(instrument.Id, sampleInfo);

            UpdatePlot();
        }

    }
}
