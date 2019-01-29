﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FAESInstaller
{
    static class Program
    {

        private static bool _headlessInstall = false;
        private static bool _verbose = false;
        private static Int16 _acceptedTOS = -1;
        private static string _branch = "stable";
        private static string _directory = Directory.GetCurrentDirectory();
        private static string _tos;
        private static List<string> _strippedArgs = new List<string>();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FreeConsole();

        [DllImport("kernel32", SetLastError = true)]
        static extern bool AttachConsole(int dwProcessId);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [STAThread]
        static void Main(string[] args)
        {
            getUpdatedTOS();

            for (int i = 0; i < args.Length; i++)
            {
                args[i].ToLower();

                string strippedArg = args[i];

                if (Directory.Exists(args[i])) _directory = args[i];

                if (args[i][0] == '-') strippedArg = args[i].Replace("-", string.Empty);
                else if (args[i][0] == '/') strippedArg = args[i].Replace("/", string.Empty);
                else if (args[i][0] == '\\') strippedArg = args[i].Replace("\\", string.Empty);

                if (strippedArg == "headless" || strippedArg == "h") _headlessInstall = true;
                else if (strippedArg == "verbose" || strippedArg == "v") _verbose = true;
                else if (strippedArg == "accept" || strippedArg == "a") _acceptedTOS = 1;
                else if (strippedArg == "deny" || strippedArg == "d") _acceptedTOS = 0;
                else if (strippedArg == "branch" || strippedArg == "b" && args[i + 1] == "stable") _branch = "stable";
                else if (strippedArg == "branch" || strippedArg == "b" && args[i + 1] == "dev") _branch = "dev";

                _strippedArgs.Add(strippedArg);
            }

            if (!_headlessInstall)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FAESInstaller());
            }
            else
            {
                IntPtr ptr = GetForegroundWindow();
                int u;
                GetWindowThreadProcessId(ptr, out u);
                Process process = Process.GetProcessById(u);

                if (process.ProcessName == "cmd")
                {
                    AttachConsole(process.Id);
                    Console.WriteLine("");
                }
                else
                    AllocConsole();

                if (Directory.Exists(_directory))
                {
                    if (_verbose)
                    {
                        Console.WriteLine("Launched using the following arguments: '{0}'.", string.Join(", ", _strippedArgs.ToArray()));
                    }

                    if (_acceptedTOS == 1)
                        Console.WriteLine(Installer.doInstall(true, true, _directory, _branch));
                    else if (_acceptedTOS == 0)
                        Console.WriteLine("Installation Canceled!" + Environment.NewLine + "You need to accept the TOS to install and use this program.");
                    else
                        Console.WriteLine(Environment.NewLine + getUpdatedTOS() + Environment.NewLine);
                }
                else
                    Console.WriteLine("Count not install to the directory '{0}'!", _directory);
            }
        }

        public static bool isVerbose()
        {
            return _verbose;
        }

        public static string getBranch()
        {
            return _branch;
        }

        public static string getUpdatedTOS()
        {
            return _localTOS;
        }

        public static bool getAccepted()
        {
            if (_acceptedTOS == 1) return true;
            else return false;
        }

        #region TOS
        private static string _localTOS = @"FileAES - Terms and conditions

1. Preamble: This Agreement, signed on May 24, 2017 (hereinafter: Effective Date) governs the relationship between you, a user, (hereinafter: Licensee) and mullak99, a private person (hereinafter: Licensor). This Agreement sets the terms, rights, restrictions and obligations on using FileAES (hereinafter: The Software) created and owned by Licensor, as detailed herein.


2. License Grant: Licensor hereby grants Licensee a Personal, Non-assignable & non-transferable, Pepetual, Commercial, Royalty free, Without the rights to create derivative works, Non-exclusive license, all with accordance with the terms set forth and other legal restrictions set forth in 3rd party software used while running Software.

2.1. Limited: Licensee may use Software for the purpose of: 
2.1.1. Running Software on Licensee’s Website[s] and Server[s];
2.1.2. Allowing 3rd Parties to run Software on Licensee’s Website[s] and Server[s];
2.1.3. Publishing Software’s output to Licensee and 3rd Parties;
2.1.4. Distribute verbatim copies of Software’s output;
2.1.5. Modify Software to suit Licensee’s needs and specifications.

2.2. This license is granted perpetually, as long as you do not materially breach it.
2.3. Binary Restricted: Licensee may sublicense Software as a part of a larger work containing more than Software, distributed solely in Object or Binary form under a personal, non-sublicensable, limited license. Such redistribution shall be limited to unlimited codebases.
2.4. Non Assignable & Non-Transferable: Licensee may not assign or transfer his rights and duties under this license.
2.5. Commercial, Royalty Free: Licensee may use Software for any purpose, including paid-services, without any royalties
2.6. With Attribution Requirements﻿: State that 'FileAES' is being used, while linking to 'mullak99.co.uk'.


3. Term & Termination: The Term of this license shall be until terminated. Licensor may terminate this Agreement, including Licensee’s license in the case where Licensee:

3.1. became insolvent or otherwise entered into any liquidation process; or
3.2. exported The Software to any jurisdiction where licensor may not enforce his rights under this agreements in; or
3.3. Licensee was in breach of any of this license's terms and conditions and such breach was not cured, immediately upon notification; or
3.4. Licensee in breach of any of the terms of clause 2 to this license; or
3.5. Licensee otherwise entered into any arrangement which caused Licensor to be unable to enforce his rights under this License.


4. Payment:  In consideration of the License granted under clause 2, Licensee should not be required to pay a fee. This software is distributed as free-ware.


5. Upgrades, Updates and Fixes: Licensor may provide Licensee, from time to time, with Upgrades, Updates or Fixes, as detailed herein and according to his sole discretion. Licensee hereby warrants to keep The Software up-to-date and install all relevant updates and fixes, and may, at his sole discretion, purchase upgrades, according to the rates set by Licensor. Licensor shall provide any update or Fix free of charge; however, nothing in this Agreement shall require Licensor to provide Updates or Fixes.

5.1. Upgrades: for the purpose of this license, an Upgrade shall be a material amendment in The Software, which contains new features and or major performance improvements and shall be marked as a new version number. For example, should Licensee purchase The Software under version 1.X.X.X, an upgrade shall commence under number 2.0.0.0.
5.2. Major Updates: for the purpose of this license, a major update shall be a semi-major amendment in The Software, which may contain new features or semi-major improvements and shall be marked as a new sub-version number. For example, should Licensee purchase The Software under version 1.1.X.X, an upgrade shall commence under number 1.2.0.0.
5.3. Minor Updates: for the purpose of this license, a minor update shall be a minor amendment in The Software, which may contain new features or minor improvements and shall be marked as a new sub-sub-version number. For example, should Licensee purchase The Software under version 1.1.1.X, an upgrade shall commence under number 1.1.2.0.
5.4. Fix: for the purpose of this license, a fix shall be a minor amendment in The Software, intended to remove bugs or alter minor features which impair the The Software's functionality. A fix shall be marked as a new sub-sub-sub-version number. For example, should Licensee purchase Software under version 1.1.0.1, an upgrade shall commence under number 1.1.0.2.


6. Support: Software is provided under an AS-IS basis and without any support, updates or maintenance. Nothing in this Agreement shall require Licensor to provide Licensee with support or fixes to any bug, failure, mis-performance or other defect in The Software.

6.1. Bug Notification: Licensee may provide Licensor of details regarding any bug, defect or failure in The Software promptly and with no delay from such event; Licensee shall comply with Licensor's request for information regarding bugs, defects or failures and furnish him with information, screenshots and try to reproduce such bugs, defects or failures.
6.2. Feature Request: Licensee may request additional features in Software, provided, however, that (i) Licensee shall waive any claim or right in such feature should feature be developed by Licensor; (ii) Licensee shall be prohibited from developing the feature, or disclose such feature request, or feature, to any 3rd party directly competing with Licensor or any 3rd party which may be, following the development of such feature, in direct competition with Licensor; (iii) Licensee warrants that feature does not infringe any 3rd party patent, trademark, trade-secret or any other intellectual property right; and (iv) Licensee developed, envisioned or created the feature solely by himself.


7. Liability:  To the extent permitted under Law, The Software is provided under an AS-IS basis. Licensor shall never, and without any limit, be liable for any damage, cost, expense or any other payment incurred by Licensee as a result of Software’s actions, failure, bugs and/or any other interaction between The Software  and Licensee’s end-equipment, computers, other software or any 3rd party, end-equipment, computer or services.  Moreover, Licensor shall never be liable for any defect in source code written by Licensee when relying on The Software or using The Software’s source code.


8. Warranty:  

8.1. Intellectual Property: Licensor hereby warrants that The Software does not violate or infringe any 3rd party claims in regards to intellectual property, patents and/or trademarks and that to the best of its knowledge no legal action has been taken against it for any infringement or violation of any 3rd party intellectual property rights.
8.2. No-Warranty: The Software is provided without any warranty; Licensor hereby disclaims any warranty that The Software shall be error free, without defects or code which may cause damage to Licensee’s computers or to Licensee, and that Software shall be functional. Licensee shall be solely liable to any damage, defect or loss incurred as a result of operating software and undertake the risks contained in running The Software on License’s Server[s] and Website[s].
8.3. Prior Inspection: Licensee hereby states that he inspected The Software thoroughly and found it satisfactory and adequate to his needs, that it does not interfere with his regular operation and that it does meet the standards and scope of his computer systems and architecture. Licensee found that The Software interacts with his development, website and server environment and that it does not infringe any of End User License Agreement of any software Licensee may use in performing his services. Licensee hereby waives any claims regarding The Software's incompatibility, performance, results and features, and warrants that he inspected the The Software.";

        #endregion
    }
}
