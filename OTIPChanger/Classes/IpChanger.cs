﻿using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;

namespace OTIPChanger.Classes
{
    public class IpChanger
    {
        private readonly string CipRSA = "BC27F992A96B8E2A43F4DFBE1CEF8FD51CF43D2803EE34FBBD8634D8B4FA32F7D9D9E159978DD29156D62F4153E9C5914263FC4986797E12245C1A6C4531EFE48A6F7C2EFFFFF18F2C9E1C504031F3E4A2C788EE96618FFFCEC2C3E5BFAFAF743B3FC7A872EE60A52C29AA688BDAF8692305312882F1F66EE9D8AEB7F84B1949";
        private readonly string OTSRSA = "9B646903B45B07AC956568D87353BD7165139DD7940703B03E6DD079399661B4A837AA60561D7CCB9452FA0080594909882AB5BCA58A1A1B35F8B1059B72B1212611C6152AD3DBB3CFBEE7ADC142A75D3D75971509C321C5C24A5BD51FD460F01B4E15BEB0DE1930528A5D3F15C1E3CBF5C401D6777E10ACAAB33DBE8D5B7FF5";

        private string currentPath = null;
        private string currentLoginUrl = null;
        private string currentServiceUrl = null;

        public bool SaveAsNewFile { get; set; } = false;

        public (string, string) LoadClientData(string path)
        {
            currentPath = path;
            try
            {
                var lines = File.ReadAllLines(path);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("loginWebService="))
                    {
                        int index = lines[i].IndexOf("=");
                        currentLoginUrl = lines[i].Substring(index + 1);
                    }
                    else if (lines[i].Contains("clientWebService="))
                    {
                        int index = lines[i].IndexOf("=");
                        currentServiceUrl = lines[i].Substring(index + 1);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Can't read client data, try again, close client if opened or run this program as admin.");
            }
            return (currentLoginUrl, currentServiceUrl);
        }

        public int ReplaceSequence(int pos, byte[] data, byte[] pattern, byte[] replacement)
        {
            while (pos < data.Length)
            {
                bool foundMatch = true;
                for (int i = 0; i < pattern.Length; i++)
                {
                    if (data[pos + i] != pattern[i])
                    {
                        foundMatch = false;
                        break;
                    }
                }

                if (foundMatch)
                {
                    for (int i = 0; i < pattern.Length; i++)
                    {
                        data[pos + i] = i <= replacement.Length - 1 ? replacement[i] : (byte)0x20;
                    }

                    pos += pattern.Length - 1;
                    break;
                }
                else
                {
                    ++pos;
                }
            }
            return pos;
        }

        public (bool, string) TryChangeIp(string path, string loginUrl, string serviceUrl)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(loginUrl) || string.IsNullOrEmpty(serviceUrl))
            {
                return (false, "Login url, service url or path is empty.");
            }

            byte[] allBytes = File.ReadAllBytes(path);

            byte[] oldLoginUrlBytes = Encoding.UTF8.GetBytes(currentLoginUrl);
            byte[] newLoginUrlBytes = Encoding.UTF8.GetBytes(loginUrl);

            byte[] oldServiceUrlBytes = Encoding.UTF8.GetBytes(currentServiceUrl);
            byte[] newServiceUrlBytes = Encoding.UTF8.GetBytes(serviceUrl);

            byte[] cipRsaKeyBytes = Encoding.UTF8.GetBytes(CipRSA);
            byte[] otsRsaKeyBytes = Encoding.UTF8.GetBytes(OTSRSA);

            int pos = 0;
            pos = ReplaceSequence(pos, allBytes, oldLoginUrlBytes, newLoginUrlBytes);
            pos = ReplaceSequence(pos, allBytes, oldServiceUrlBytes, newServiceUrlBytes);
            ReplaceSequence(pos, allBytes, cipRsaKeyBytes, otsRsaKeyBytes);

            string outputPath = null;
            if (SaveAsNewFile)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog { Filter = "Tibia exe (*.exe)|*.exe", Title = "Save new Tibia file" };
                if (saveFileDialog.ShowDialog() == true)
                {
                    outputPath = saveFileDialog.FileName;
                }
                else
                {
                    return (true, "Nothing was changed.");
                }
            }
            else
            {
                outputPath = currentPath;
            }
           
            if (string.IsNullOrEmpty(outputPath))
            {
                return (false, "Failed to save file.");
            }

            File.WriteAllBytes(outputPath, allBytes);

            return (true, $"Login service and web service was replaced succesfully.");
        }
    }
}
