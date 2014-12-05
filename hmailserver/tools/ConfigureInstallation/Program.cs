﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ConfigureInstallation
{
   static class Program
   {

      static int Main(string []args)
      {
         if (args.Count() != 3)
         {
            Console.WriteLine("Expected two parameters: RootDir, Version, Build");
            return -1;
         }

         var rootDir = args[0];
         var version = args[1];
         var build = args[2];

         if (!Directory.Exists(rootDir))
         {
            Console.WriteLine("Root directory {0} was not found.", rootDir);
            return -1;
         }

         // Write C++ version header file.
         string cppVersionFile = Path.Combine(rootDir, @"hmailserver\source\Server\Common\Application\Version.h");
         if (!File.Exists(cppVersionFile))
         {
            Console.WriteLine("Version file {0} was not found.", cppVersionFile);
            return -1;
         }
         Console.WriteLine("Writing c++ version info to {0}", cppVersionFile);
         var versionContent = string.Format("#pragma once\r\n\r\n#define HMAILSERVER_VERSION \"{0}\"\r\n#define HMAILSERVER_BUILD {1}\r\n\r\n", version, build);
         File.WriteAllText(cppVersionFile, versionContent);

         // Write C++ version header file.
         string phpVersionFile = Path.Combine(rootDir, @"hmailserver\source\WebAdmin\include_versioncheck.php");

         Console.WriteLine("Writing php version info to {0}", phpVersionFile);
         var phpVersionContent = string.Format("<?php\r\ndefine('REQUIRED_VERSION', '{0}-B{1}');\r\n?>", version, build);
         File.WriteAllText(phpVersionFile, phpVersionContent);

         // Write installation program verison
         if (ConfigureInstallationFile(Path.Combine(rootDir, @"hmailserver\Installation\hMailServer32.iss"), version, build))
            return -1;
         if (ConfigureInstallationFile(Path.Combine(rootDir, @"hmailserver\Installation\hMailServer64.iss"), version, build))
            return -1;

         Console.WriteLine("All done. Exiting.");
         return 0;
      }

      private static bool ConfigureInstallationFile(string installationFile, string version, string build)
      {
         if (!File.Exists(installationFile))
         {
            Console.WriteLine("Installation file {0} was not found.", installationFile);
            {
               
               return true;
            }
         }
         Console.WriteLine("Writing install version and output name to {0}", installationFile);


         Ini.Write(installationFile, "Setup", "AppVerName", string.Format("hMailServer {0}-B{1}", version, build));

         // Write installation output verison
         Ini.Write(installationFile, "Setup", "OutputBaseFilename", string.Format("hMailServer-{0}-B{1}", version, build));
         return false;
      }
   }
}
