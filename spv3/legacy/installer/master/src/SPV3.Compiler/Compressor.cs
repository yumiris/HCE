using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Directory = SPV3.Domain.Directory;
using File = SPV3.Domain.File;

namespace SPV3.Compiler
{
    /// <summary>
    ///     Creates DEFLATE archives for files/directories on the filesystem.
    /// </summary>
    /// <remarks>
    ///     7-Zip 18.05 (2018-04-30) is used for creating the archive.
    /// </remarks>
    public class Compressor
    {
        /// <summary>
        ///     Expected path for the 7-Zip executable, relative to the this DLL/Executable path.
        /// </summary>
        private static readonly string SevenZipPath = Path.Combine("7z", "7za.exe");

        /// <summary>
        ///     MD5 hash of the 7-Zip 18.05 (2018-04-30) (x64) (Windows) executable.
        /// </summary>
        private static readonly string SevenZipHash = "e93c0c45d84f328afba850e87ac7398c";

        /// <summary>
        ///     Creates a DEFLATE archive for the provided source Directory at the given target.
        /// </summary>
        /// <param name="target">
        ///     Target archive which will contain the compressed source Directory.
        /// </param>
        /// <param name="source">
        ///     Source Directory on the filesystem to compress to the target archive.
        /// </param>
        public void Compress(File target, Directory source)
        {
            var args = new StringBuilder($"7z a -tzip \"{target}\" \"{source}\"");

            InvokeProcess(args.ToString());
        }

        /// <summary>
        ///     Creates a DEFLATE archive for the provided files in source Directory at the given target.
        /// </summary>
        /// <param name="target">
        ///     Target archive which will contain the compressed source Directory.
        /// </param>
        /// <param name="source">
        ///     Source Directory on the filesystem containing the files.
        /// </param>
        /// <param name="files">
        ///     Files in the source Directory to compress to the target archive.
        /// </param>
        public void Compress(File target, Directory source, IEnumerable<File> files)
        {
            var args = new StringBuilder($"7z a -tzip \"{target}\" ");

            foreach (var file in files)
            {
                var path = Path.Combine(source, file);
                args.Append($"\"{path}\" ");
            }

            InvokeProcess(args.ToString());
        }

        /// <summary>
        ///     Safely invokes the 7-Zip executable with the provided arguments.
        /// </summary>
        /// <param name="args">
        ///     Arguments to pass onto the 7-Zip executable. See: https://sevenzip.osdn.jp/chm/cmdline/
        /// </param>
        /// <param name="waitForExit">
        ///     Hangs the thread until the process is finished. By default, this is true.
        ///     This is useful in loop contexts where invoking one process at a time is preferable.
        /// </param>
        /// <exception cref="FormatException">
        ///     Could not infer working directory.
        ///     - or -
        ///     Could not infer executable binary.
        /// </exception>
        /// <exception cref="SecurityException">
        ///     7z executable hash does not match the expected one.
        /// </exception>
        private void InvokeProcess(string args, bool waitForExit = true)
        {
            var workingDirectory = Path.GetDirectoryName(SevenZipPath) ??
                                   throw new FormatException("Could not infer working directory.");

            var executableBinary = Path.GetFileName(SevenZipPath) ??
                                   throw new FormatException("Could not infer executable binary.");

            if (!VerifyExecutable())
                throw new SecurityException("7z executable hash does not match the expected one.");

            var process = new Process
            {
                StartInfo =
                {
                    FileName = executableBinary,
                    WorkingDirectory = workingDirectory,
                    Arguments = args
                }
            };

            process.Start();

            if (waitForExit)
                process.WaitForExit();
        }

        /// <summary>
        ///     Verifies the hash of the 7-Zip executable.
        /// </summary>
        /// <returns>
        ///     True on the executable hash matching the expected one; otherwise false.
        /// </returns>
        private bool VerifyExecutable()
        {
            using (var md5 = MD5.Create())
            using (var stream = System.IO.File.OpenRead(SevenZipPath))
            {
                return BitConverter
                    .ToString(md5.ComputeHash(stream))
                    .Replace("-", "")
                    .ToLowerInvariant()
                    .Equals(SevenZipHash);
            }
        }
    }
}