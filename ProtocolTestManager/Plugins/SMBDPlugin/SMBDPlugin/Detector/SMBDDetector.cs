﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Protocols.TestManager.Detector;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation.Runspaces;

namespace Microsoft.Protocols.TestManager.SMBDPlugin.Detector
{
    class SMBDDetector
    {
        public DetectionInfo DetectionInfo { get; private set; }

        public SMBDDetector(DetectionInfo detectionInfo)
        {
            DetectionInfo = detectionInfo;
        }

        public bool PingSUT()
        {
            DetectorUtil.WriteLog("Ping Target SUT...");

            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();

            // Use the default TtL value which is 128,
            // but change the fragmentation behavior.
            options.DontFragment = true;

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "0123456789ABCDEF0123456789ABCDEF";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 5000;
            bool result = false;
            List<PingReply> replys = new List<PingReply>();
            try
            {
                for (int i = 0; i < 4; i++)
                {
                    replys.Add(pingSender.Send(DetectionInfo.SUTName, timeout, buffer, options));
                }

            }
            catch
            {
                DetectorUtil.WriteLog("Error", false, LogStyle.Error);

                //return false;
                throw;
            }
            foreach (var reply in replys)
            {

                result |= (reply.Status == IPStatus.Success);
            }
            if (result)
            {
                DetectorUtil.WriteLog("Passed", false, LogStyle.StepPassed);
                return true;
            }
            else
            {
                DetectorUtil.WriteLog("Failed", false, LogStyle.StepFailed);
                DetectorUtil.WriteLog("Target SUT don't respond.");
                return false;
            }

        }

        public bool CheckUsernamePassword()
        {
            if (!DetectionInfo.IsWindowsImplementation)
            {
                DetectorUtil.WriteLog("Skip for non-Windows", false, LogStyle.StepSkipped);
                return true;
            }

            DetectorUtil.WriteLog("Check the Credential...");

            string[] error;

            var output = ExecutePowerShellCommand(@"..\etc\MS-SMBD\CheckRemoteCredential.ps1", out error);

            if (output.Length == 1 && output[0].ToString() == "0")
            {
                DetectorUtil.WriteLog("Finished", false, LogStyle.StepPassed);
                return true;
            }
            else
            {
                if (error != null)
                {
                    foreach (var item in error)
                    {
                        DetectorUtil.WriteLog(item.ToString());
                    }
                }
                DetectorUtil.WriteLog("Failed", false, LogStyle.StepFailed);
                return false;
            }

        }

        public bool get



        /// <summary>
        /// Execute a PowerShell script file
        /// </summary>
        /// <param name="scriptPath"></param>
        /// <returns></returns>
        private object[] ExecutePowerShellCommand(string scriptPath, out string[] error)
        {

            using (var runspace = RunspaceFactory.CreateRunspace())
            {

                runspace.Open();

                runspace.SessionStateProxy.SetVariable("PtfPropSUTName", DetectionInfo.SUTName);
                runspace.SessionStateProxy.SetVariable("PtfPropDomainName", DetectionInfo.DomainName);
                runspace.SessionStateProxy.SetVariable("PtfPropSUTUserName", DetectionInfo.UserName);
                runspace.SessionStateProxy.SetVariable("PtfPropSUTUserPassword", DetectionInfo.Password);

                using (var pipeline = runspace.CreatePipeline())
                {
                    pipeline.Commands.Add(scriptPath);

                    var output = pipeline.Invoke();



                    var result = output.Select(element => element.BaseObject);


                    if (pipeline.HadErrors)
                    {
                        var errors = pipeline.Error.ReadToEnd();


                        error = errors.Select(element => element.ToString()).ToArray();
                    }
                    else
                    {
                        error = null;
                    }

                    return result.ToArray();
                }
            }



        }

    }
}
