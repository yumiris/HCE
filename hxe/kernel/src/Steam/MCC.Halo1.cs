/**
 * Copyright (c) 2019 Emilian Roman
 * Copyright (c) 2020 Noah Sherwin
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

using System.IO;
using static HXE.Paths.MCC;

namespace HXE.Steam
{
  public partial class MCC
  {
    public static class Halo1
    {
      /// <summary>
      ///   Set a new path for Halo1.dll
      /// </summary>
      public static void SetHalo1Path()
      {
        var libs = new Libraries();
        var mccH1 = Path.Combine(SteamMccH1, Halo1dll);

        libs.ParseLibraries();
        libs.ScanLibraries(mccH1);
        Halo1Path = libs.ReturnPaths[0];
        if (Halo1Path == null) 
          throw new FileNotFoundException("Halo1.dll not found");
        if (!VerifyHalo1DLL())
          throw new System.Exception("Halo1.dll is invalid.");
      }

      /// <summary>
      /// Check if the inferred Halo1.dll is probably legitimate.
      /// </summary>
      /// <returns>Returns true if the file is larger than 20MiB.</returns>
      public static bool VerifyHalo1DLL()
      {
        var fileinfo = new FileInfo(Halo1Path);
        if (20971520 < (ulong)fileinfo.Length)
            return true;
        else return false;
      }
    }
  }
}
