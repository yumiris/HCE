using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using SPV3.Domain;
using SPV3.Installer.Data;
using Directory = SPV3.Domain.Directory;
using File = SPV3.Domain.File;
using Package = SPV3.Installer.Data.Package;
using Version = SPV3.Domain.Version;

namespace SPV3.Compiler
{
    /// <summary>
    ///     Compiles SPV3 source to redistributable packages w/ manifest.
    /// </summary>
    public class Compiler
    {
        /// <summary>
        ///     Prefix used for the package/manifest files.
        /// </summary>
        private const string Prefix = "0x";

        /// <summary>
        ///     Suffix (or extension) used for the package/manifest files.
        /// </summary>
        private const string Suffix = ".bin";

        /// <summary>
        ///     Name for the manifest file.
        /// </summary>
        private const string ManifestBin = Prefix + "00" + Suffix;

        /// <summary>
        ///     Name for the core package.
        /// </summary>
        private const string CorePackage = Prefix + "01" + Suffix;

        /// <summary>
        ///     Current manifest/compiler version.
        /// </summary>
        private static readonly Version Version = new Version
        {
            Major = 1,
            Minor = 0,
            Patch = 0
        };

        /// <summary>
        ///     Created & compiled packages.
        /// </summary>
        private readonly List<Package> _packages = new List<Package>();

        /// <summary>
        ///     SPV3 source data directory.
        /// </summary>
        private readonly Directory _source;

        /// <summary>
        ///     Target directory for manifest & packages.
        /// </summary>
        private readonly Directory _target;

        /// <summary>
        ///     Compiler constructor.
        /// </summary>
        /// <param name="source">
        ///     SPV3 source data directory.
        /// </param>
        /// <param name="target">
        ///     Target directory for manifest & packages.
        /// </param>
        public Compiler(Directory source, Directory target)
        {
            _source = source;
            _target = target;
        }

        /// <summary>
        ///     Compiles the provided source directory into DEFLATE packages & manifest in the target directory.
        /// </summary>
        public void Compile()
        {
            CreateCorePackage();
            CreateDirPackages();
            CreateMetadataBin();
        }

        /// <summary>
        ///     Creates the metadata binary in the target directory.
        /// </summary>
        private void CreateMetadataBin()
        {
            var metaPath = (File) Path.Combine(_target, ManifestBin);

            var metadata = new Manifest
            {
                Version = Version,
                Packages = _packages
            };

            new ManifestRepository(metaPath).Save(metadata);
        }

        /// <summary>
        ///     Creates the core DEFLATE package and adds it as a Package in the Packages list.
        ///     The package files & Entries are all of the root files in the source directory.
        /// </summary>
        private void CreateCorePackage()
        {
            var core = new DirectoryInfo(_source)
                .GetFiles("*.*");

            /**
             * Invokes the Compressor class with the intent of creating a single DEFLATE package, which would contain
             * all of the root files in the source directory.
             */
            void Invoke()
            {
                var target = (File) Path.Combine(_target, CorePackage);

                var files = core
                    .Select(info => (File) info.Name)
                    .ToList();

                new Compressor().Compress(target, _source, files);
            }

            /**
             * Adds an Entry for each root file in the source directory in the Package, which is then added to the main
             * Packages List.
             */
            void Append()
            {
                var files = core
                    .Select(file => new Entry
                    {
                        Name = (Name) file.Name,
                        Type = EntryType.File
                    })
                    .ToList();

                var package = new Package
                {
                    Name = (Name) CorePackage,
                    Description = (Description) "Core SPV3/HCE data",
                    Directory = new Directory(),
                    Entries = files
                };

                _packages.Add(package);
            }

            Invoke();
            Append();
        }

        /// <summary>
        ///     Creates the DEFLATE packages for subdirectories in the source folder, and adds each subdirectory as a
        ///     Package in the Packages list. Each created Package's Entries represent the files in the respective
        ///     subdirectory.
        /// </summary>
        private void CreateDirPackages()
        {
            /**
             * 
             */
            void Invoke(string name, FileSystemInfo directory)
            {
                var target = (File) Path.Combine(_target, $"{name}");
                var source = (Directory) Path.Combine(_source, directory.Name);

                new Compressor().Compress(target, source);
            }

            /**
             * Adds an Entry for each file in the subdirectory in a new Package, which is then added to the main
             * Packages List.
             */
            void Append(string name, FileSystemInfo directory)
            {
                var files = System.IO.Directory
                    .GetFileSystemEntries(directory.FullName, "*");

                var entries = new List<Entry>();

                foreach (var file in files)
                {
                    var type = EntryType.Unknown;

                    if (System.IO.File.Exists(file))
                        type = EntryType.File;

                    if (System.IO.Directory.Exists(file))
                        type = EntryType.Directory;

                    if (type == EntryType.Unknown)
                        throw new FormatException("Cannot infer Entry Type. Does filesystem record exist?");

                    entries.Add(new Entry
                    {
                        Name = (Name) Path.GetFileName(file),
                        Type = type
                    });
                }

                _packages.Add(new Package
                {
                    Name = (Name) name,
                    Description = (Description) $"{directory.Name} files",
                    Directory = (Directory) directory.Name,
                    Entries = entries
                });
            }

            /**
             * This loop ensures that a single package per subdirectory in the source folder is created.
             * The index starts at two, considering that 0 & 1 represent the manifest and core package, respectively.
             */
            var index = 2;
            foreach (var directory in new DirectoryInfo(_source).GetDirectories())
            {
                var name = $"{Prefix}{index:D2}{Suffix}";

                Invoke(name, directory);
                Append(name, directory);

                index++;
            }
        }
    }
}