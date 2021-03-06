/**
 * Copyright (C) 2019 Emilian Roman
 * 
 * This file is part of SPV3.Launcher.
 * 
 * SPV3.Launcher is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * SPV3.Launcher is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with SPV3.Launcher.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SPV3.Launcher.GUI
{
    /// <summary>
    ///     Class containing code that handles this UI.
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        ///     Model used for the data context.
        /// </summary>
        private readonly Main _main;

        /// <summary>
        ///     Constructor that conducts startup routines & UI initialisation.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            InitialiseBackground();

            _main = (Main) DataContext;
        }

        /// <summary>
        ///     Event handler for the Load button.
        /// </summary>
        private void Load(object sender, RoutedEventArgs e)
        {
            _main.Load();
        }

        /// <summary>
        ///     Event handler for the Configuration button.
        /// </summary>
        private void Configuration(object sender, RoutedEventArgs e)
        {
            if (File.Exists("SPV3.Settings.GUI.exe")) Process.Start("SPV3.Settings.GUI.exe");
        }

        /// <summary>
        ///     Event handler for the Exit button.
        /// </summary>
        private void Exit(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        ///     Sets the background image to one of the randomly picked images in the Assets/Backgrounds directory.
        /// </summary>
        private void InitialiseBackground()
        {
            var bgDirPath = Path.Combine("Assets", "Backgrounds");
            if (!Directory.Exists(bgDirPath)) return;

            var directory = new DirectoryInfo(bgDirPath);
            var backgrounds = directory.GetFiles("*.jpg");

            var pickedImage = backgrounds[new Random().Next(0, backgrounds.Length)].FullName;
            if (!File.Exists(pickedImage)) return;

            Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(pickedImage, UriKind.Absolute)),
                Stretch = Stretch.UniformToFill
            };
        }

        /// <summary>
        ///     Disables tab navigation using tab (w/ modifying keys).
        /// </summary>
        private void HandleTabNavigation(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab && (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) ==
                (ModifierKeys.Control | ModifierKeys.Shift))
                e.Handled = true;

            if (e.Key == Key.Tab && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                e.Handled = true;
        }
    }
}