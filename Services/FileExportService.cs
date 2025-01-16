using System.Text;
using System.Windows.Forms;

// Interface for file export operations
public interface IFileExportService
{
    Task ExportToCsvAsync(string content, string defaultFileName);
}

/* 
 * Implementation of IFileExportService that handles CSV file exports
 * This service provides functionality to save content as CSV files
 */
public class FileExportService : IFileExportService
{
    // Exports content to a CSV file with a user-selected location
    // Parameters:
    //   content: The data to be written to the CSV file
    //   defaultFileName: The suggested file name shown in the save dialog
    public async Task ExportToCsvAsync(string content, string defaultFileName)
    {
        // Create and configure save file dialog
        using (SaveFileDialog saveFileDialog = new SaveFileDialog())
        {
            // Set file type filter to only show CSV files
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.FileName = defaultFileName;

            // Show the dialog and wait for user input
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Write the content to the selected file asynchronously using UTF-8 encoding
                await File.WriteAllTextAsync(saveFileDialog.FileName, content, Encoding.UTF8);
                // Show success message to the user
                MessageBox.Show($"File exported successfully to: {saveFileDialog.FileName}", "Export Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}