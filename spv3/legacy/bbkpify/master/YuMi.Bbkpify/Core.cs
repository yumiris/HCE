using System.ComponentModel;
using System.IO;
using static System.Console;
using static System.ConsoleColor;

namespace YuMi.Bbkpify
{
    /// <summary>
    ///     Class with core properties or methods.
    /// </summary>
    public static class Core
    {
        /// <summary>
        ///     File extension to use for the backup file.
        /// </summary>
        public static readonly string Extension = "bbkp";
        
        /// <summary>
        /// Backs up all given bitmaps and replaces them with the given placeholder.
        /// </summary>
        /// <param name="bitmapPaths">Array of bitmaps to back up and replace.</param>
        /// <param name="placeholderPath">Path to the placeholder file.</param>
        public static void Commit(string[] bitmapPaths, string placeholderPath)
        {
            for (var i = 0; i < bitmapPaths.Length; i++)
            {
                var file = bitmapPaths[i];
                var bbkpFile = $"{file}.{Extension}";

                // looks like: [1/10]
                var progress = Ascii.Progress(i, bitmapPaths.Length);

                // check if the current file has been handled in a previous execution
                if (!file.Contains(Extension) && !File.Exists(bbkpFile))
                {
                    ForegroundColor = Green;
                    WriteLine($"{progress}\t| HANDLING {file}");

                    // backup through renaming, and replace with the placeholder
                    File.Move(file, bbkpFile);
                    File.Copy(placeholderPath, file);
                }
                else
                {
                    ForegroundColor = Yellow;
                    WriteLine($"{progress}\t| SKIPPING {file}");
                }
            }
        }

        /// <summary>
        /// Restores the backed up bitmaps and discards the placeholders.
        /// </summary>
        /// <param name="bitmapBbkpPaths">
        /// Bitmaps to restore. They must end in ".bbkp"!
        /// </param>
        /// <exception cref="InvalidEnumArgumentException">
        /// One of the files in the array is not a ".bbkp" file.
        /// </exception>
        public static void Revert(string[] bitmapBbkpPaths)
        {
            ForegroundColor = Green;
            for (var i = 0; i < bitmapBbkpPaths.Length; i++)
            {
                var currentFile = bitmapBbkpPaths[i];

                if (!currentFile.Contains($".{Extension}"))
                {
                    throw new InvalidEnumArgumentException($"Bitmap '{currentFile}' is not a backup file.");
                }
                
                var placeholder = currentFile.Substring(0, currentFile.Length - Extension.Length - 1);

                var progress = Ascii.Progress(i, bitmapBbkpPaths.Length);
                WriteLine($"{progress}\t| RESTORING {placeholder}");
                
                File.Delete(placeholder);
                File.Move(currentFile, placeholder);
            }
        }
    }
}