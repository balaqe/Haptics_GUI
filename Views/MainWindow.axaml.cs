using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Platform.Storage;


namespace Haptics_GUI.Views;

public partial class MainWindow : Window
{
    private string selectedFileName;
    private string selectedFilePath;
    
    private TextBox openFileTextBox;
    
    
    private async void OpenFileButton_Clicked(object sender, System.EventArgs e)
    {
        var toplevel = TopLevel.GetTopLevel(this);
        
        FilePickerFileType filePickerFileType = new FilePickerFileType("Sound files")
        {
            Patterns = new[] {"*.wav", "*.mp3"},
            MimeTypes = new[] {"audio/x-wav", "audio/mp4"},
        };

        var file = await toplevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open sound file",
            AllowMultiple = false,
            FileTypeFilter = new[] {filePickerFileType},
        });

        if (file != null && file.Count == 1)
        {
            foreach (var path in file)
            {
                Console.WriteLine($"File selected: {path.Path}");
                Console.WriteLine($"File selected: {path.Name}");
                Console.WriteLine($"File selected: {path.Path.AbsolutePath}");
                
                openFileTextBox.Text = path.Path.AbsolutePath;
            }
        }

    }
    
    public MainWindow()
    {
        InitializeComponent();
        
        var openFileButton = this.FindControl<Button>("OpenFileButton");
        if (openFileButton != null)
        {
            openFileButton.Click += OpenFileButton_Clicked;
        }
        
        openFileTextBox = this.FindControl<TextBox>("OpenFileTextBox");
    }
}