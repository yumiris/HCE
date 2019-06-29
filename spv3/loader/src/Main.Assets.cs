/**
 * Copyright (c) 2019 Emilian Roman
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using HXE;
using SPV3.Annotations;

namespace SPV3
{
  public partial class Main
  {
    public class MainAssets : INotifyPropertyChanged
    {
      private const    string     Address     = "https://raw.githubusercontent.com/yumiris/SPV3/meta/update.hxe";
      private          Visibility _visibility = Visibility.Collapsed;
      private          string     _status     = "Update";
      private readonly Update     _update     = new Update();

      public Visibility Visibility
      {
        get => _visibility;
        set
        {
          if (value == _visibility) return;
          _visibility = value;
          OnPropertyChanged();
        }
      }

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

      public event PropertyChangedEventHandler PropertyChanged;

      public void Initialise()
      {
        try
        {
          _update.Import(Address);
          Visibility = Visibility.Visible;
        }
        catch (Exception)
        {
          Visibility = Visibility.Collapsed;
        }
      }

      public void Update()
      {
        var progress = new Progress<Status>();
        progress.ProgressChanged += (sender, status) => Status = status.Description;

        _update.Commit(progress);

        Status = "You are up to date!";
      }

      [NotifyPropertyChangedInvocator]
      protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
      {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}