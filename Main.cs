

using System.Runtime.InteropServices;
using ManagedCommon;
using Wox.Plugin;

namespace ClearRecycleBin
{
    enum RecycleFlags : uint
    {
        SHRB_NOCONFIRMATION = 0x00000001, // Don't ask confirmation
        SHRB_NOPROGRESSUI = 0x00000002, // Don't show any windows dialog
        SHRB_NOSOUND = 0x00000004 // Don't make sound, ninja mode enabled :v
    }

    public class Main : IPlugin
    {
        public string Name => nameof(ClearRecycleBin);
        public string Description => "Clear RecycleBin";
        private string? IconPath { get; set; }

        public void Init(PluginInitContext context)
        {
            context.API.ThemeChanged += OnThemeChanged;
            UpdateIconPath(context.API.GetCurrentTheme());
        }

        private void OnThemeChanged(Theme currentTheme, Theme newTheme)
        {
            UpdateIconPath(newTheme);
        }

        private void UpdateIconPath(Theme theme)
        {
            var isLight = theme == Theme.Light || theme == Theme.HighContrastWhite;
            IconPath = isLight ? "Images/ClearRecycleBin.light.png" : "Images/ClearRecycleBin.dark.png";
        }

        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, RecycleFlags dwFlags);

        public List<Result> Query(Query query)
        {
            string message;
            try
            {
                var code = SHEmptyRecycleBin(IntPtr.Zero, "", RecycleFlags.SHRB_NOCONFIRMATION);
                message = code == 0 ? $"The recycle bin has been succesfully recycled" : $"The recycle bin couldn't be recycled, code: {code}";
            }
            catch (Exception ex)
            {
                message = $"The recycle bin couldn't be recycled, {ex.Message}";
            }

            return new List<Result> {
                new() {
                    Title = message,
                    IcoPath = IconPath,
                    Action = e =>true
                },
            };
        }
    }
}
