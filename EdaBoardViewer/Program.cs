using Avalonia;
using Avalonia.Dialogs;
using Avalonia.ReactiveUI;

namespace EdaBoardViewer {

    class Program {

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args) {

            /* EdaNumber n1 = new EdaNumber(123.456);
             EdaNumber n2 = new EdaNumber(100.100);
             EdaNumber r = n1 + n2;
             double d = (double)r;
             string s = d.ToString();

             EdaNumber n3 = EdaNumber.Parse("-123.4098");
             string s2 = n3.ToString();*/

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp() {

            return AppBuilder
                .Configure<App>()
                .UsePlatformDetect()
                .UseManagedSystemDialogs()
                .LogToTrace()
                .UseReactiveUI();
        }
    }
}
