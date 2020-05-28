namespace EdaBoardViewer.ViewModels {

    using System.Reactive;
    using System.Threading.Tasks;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Controls.ApplicationLifetimes;
    using ReactiveUI;

    public class MainWindowViewModel : ViewModelBase {

        public ReactiveCommand<Unit, Unit> OpenCommand { get; }

        public MainWindowViewModel() {

            OpenCommand = ReactiveCommand.CreateFromTask(Open);
        }

        public async Task Open() {

            var xbrdFilter = new FileDialogFilter();
            xbrdFilter.Name = "EDA board";
            xbrdFilter.Extensions.Add("xbrd");

            var allFilter = new FileDialogFilter();
            allFilter.Name = "All";
            allFilter.Extensions.Add("*");

            var dialog = new OpenFileDialog();
            dialog.Title = "Open board file";
            dialog.AllowMultiple = false;
            dialog.Filters.Add(xbrdFilter);
            dialog.Filters.Add(allFilter);

            var appLifetime = Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var parent = appLifetime.MainWindow;

            var result = await dialog.ShowAsync(parent);
            if (result != null) {
                foreach (var path in result) {
                    System.Diagnostics.Debug.WriteLine($"Opened: {path}");
                }
            }
        }

    }
}
