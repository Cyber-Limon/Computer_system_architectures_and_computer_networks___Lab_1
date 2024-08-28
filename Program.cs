using System;
using System.Net;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;

namespace IPLab1
{
    class Program
    {
        static bool isANumber(string s)
        {

            return !Regex.IsMatch(s, @"[^0-9]");

        }



        static void Main(string[] args)
        {

            string beginStr;
            string endStr;

            do
            {

                Console.Write("Введите начальный адрес: ");
                beginStr = Console.ReadLine();
                Console.Write("Введите конечный адрес: ");
                endStr = Console.ReadLine();

                if (beginStr.Split('.').Length == 4 && beginStr.Split('.').All(isANumber) &&
                   endStr.Split('.').Length == 4 && endStr.Split('.').All(isANumber)) break;

            } while (true);

            IPAddress beginIP = new IPAddress(beginStr.Split('.').Select((string s) => Convert.ToByte(s)).ToArray());
            IPAddress endIP = new IPAddress(endStr.Split('.').Select((string s) => Convert.ToByte(s)).ToArray());

            byte[] begin = beginIP.GetAddressBytes();
            byte[] end = endIP.GetAddressBytes();

            byte[] mask = new byte[4];

            bool edge = false;

            for (int i = 0; i < 4; i++)
            {

                for (byte b = 128; b >= 1; b /= 2)
                {

                    if (!edge && ((begin[i] & b) == (end[i] & b)))
                    {

                        mask[i] |= b;

                    }
                    else
                    {

                        edge = true;
                        mask[i] = (byte)(mask[i] & ~b);

                    }

                }

            }

            byte[] address = new byte[4];
            byte[] broadcast = new byte[4];

            for (int i = 0; i < 4; i++)
            {

                address[i] = (byte)(begin[i] & end[i] & mask[i]);
                broadcast[i] = (byte)(address[i] + (~mask[i]));

            }

            Console.WriteLine("Маска сети: " + String.Join('.', mask));
            Console.WriteLine("Адрес сети: " + String.Join('.', address));
            Console.WriteLine("Broadcast: " + String.Join('.', broadcast));

            var macAddr = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                           where nic.OperationalStatus == OperationalStatus.Up
                           select nic.GetPhysicalAddress().GetAddressBytes()
                            ).FirstOrDefault();

            Console.WriteLine("Mac-address: " + String.Join('-', macAddr.Select((byte b) => Convert.ToHexString(new byte[] { b })).ToArray()));

        }
    }
}
