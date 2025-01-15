using System.Text;
using System.Windows.Forms;

public interface IFileExportService
{
    Task ExportToCsvAsync(string content, string defaultFileName);
}

public class FileExportService : IFileExportService
{
    public async Task ExportToCsvAsync(string content, string defaultFileName)
    {
        using (SaveFileDialog saveFileDialog = new SaveFileDialog())
        {
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.FileName = defaultFileName;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                await File.WriteAllTextAsync(saveFileDialog.FileName, content, Encoding.UTF8);
                MessageBox.Show($"File exported successfully to: {saveFileDialog.FileName}", "Export Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}