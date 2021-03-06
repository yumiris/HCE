/**
 * Copyright (C) 2019 Emilian Roman
 * 
 * This file is part of SPV3.Installer.
 * 
 * SPV3.Installer is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * SPV3.Installer is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with SPV3.Installer.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SPV3.Compiler.Common;
using SPV3.Compiler.Compilers;
using SPV3.Compiler.Compressors;
using SPV3.Compiler.GUI.Annotations;
using SPV3.Domain;
using Directory = System.IO.Directory;

namespace SPV3.Compiler.GUI
{
    /// <summary>
    ///     Main model used for the UI bindings.
    /// </summary>
    public class Main : INotifyPropertyChanged, IStatus
    {
        /// <summary>
        ///     <see cref="CanCompile" />
        /// </summary>
        private bool _canCompile;

        /// <summary>
        ///     <see cref="Source" />
        /// </summary>
        private string _source;

        /// <summary>
        ///     <see cref="Status" />
        /// </summary>
        private string _status = "Awaiting end-user invocation...";

        /// <summary>
        ///     <see cref="Target" />
        /// </summary>
        private string _target;

        /// <summary>
        ///     <see cref="UseExternal" />
        /// </summary>
        private bool _useExternal;

        /// <summary>
        ///     Status output.
        /// </summary>
        public string Status
        {
            get => _status;
            set
            {
                if (value == _status) return;
                _status = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Source directory path.
        /// </summary>
        public string Source
        {
            get => _source;
            set
            {
                if (value == _source) return;
                _source = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Target directory path.
        /// </summary>
        public string Target
        {
            get => _target;
            set
            {
                if (value == _target) return;
                _target = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Use external compressor instead of the native one.
        /// </summary>
        public bool UseExternal
        {
            get => _useExternal;
            set
            {
                if (value == _useExternal) return;
                _useExternal = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Allow compilation.
        /// </summary>
        public bool CanCompile
        {
            get => _canCompile;
            set
            {
                if (value == _canCompile) return;
                _canCompile = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void CommitStatus(string data)
        {
            Status = $"{DateTime.Now:u} - {data}\n{Status}";
        }

        public void CommitStatus(string data, StatusType type)
        {
            var symbol = string.Empty;
            
            switch (type)
            {
                case StatusType.Success:
                    symbol = "???";
                    break;
                case StatusType.Warning:
                    symbol = "???";
                    break;
                case StatusType.Failure:
                    symbol = "???";
                    break;
                case StatusType.Require:
                    symbol = "???";
                    break;
            }

            CommitStatus($"{symbol} | {data}");
        }

        /// <summary>
        ///     Updates CanCompile. If Source & Target directories exist on the filesystem, CanCompile becomes true.
        /// </summary>
        public void NotifyCanCompile()
        {
            CanCompile = Directory.Exists(Source) && Directory.Exists(Target);
        }

        /// <summary>
        ///     Instantiates the SPV3.Compiler's Meta Compiler class with the given Compressor & IStatus instances, and
        ///     invokes the Compile method with the Source and Target Directories.
        /// </summary>
        public void Compile()
        {
            try
            {
                Status = string.Empty;

                Compressor compressor;

                if (UseExternal)
                    compressor = new ExternalCompressor();
                else
                    compressor = new InternalCompressor();

                var source = (Domain.Directory) Source;
                var target = (Domain.Directory) Target;
                var status = this;

                new MetaCompiler(compressor, status).Compile(source, target);
            }
            catch (Exception exception)
            {
                CommitStatus(exception.ToString());
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            NotifyCanCompile();
        }
    }
}
