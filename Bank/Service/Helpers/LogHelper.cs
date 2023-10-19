using Manager;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers
{
    static class LogHelper
    {
        public static void CheckLogs(string clientName)
        {
            // From App.config

            int time = Int32.Parse(ConfigurationManager.AppSettings["time"]);
            int withdrawNumber = Int32.Parse(ConfigurationManager.AppSettings["withdrawNumber"]);

            List<EventLogEntry> entries = new List<EventLogEntry>();

            EventLogEntryCollection events = Audit.customLog.Entries;

            foreach (EventLogEntry _event in events)
            {
                if (_event.InstanceId.Equals((int)AuditEventTypes.WithdrawSuccess))
                {
                    string logUser = _event.Message.Split()[1];

                    if (logUser.Equals(clientName))
                    {
                        entries.Add(_event);
                    }
                }
            }

            if (entries.Count >= withdrawNumber)
            {
                entries = entries.OrderByDescending(x => x.TimeGenerated).ToList();

                DateTime last = entries[0].TimeGenerated;

                DateTime first = entries[withdrawNumber - 1].TimeGenerated;

                double elapsed = last.Subtract(first).TotalSeconds;

                if (time >= elapsed)
                {
                    List<string> amounts = new List<string>();

                    for (int i = 0; i < withdrawNumber; i++)
                    {
                        string amount = entries[i].Message.Split()[8].Split('.')[0];

                        amounts.Add(amount);
                    }

                    try
                    {
                        Program.bankingAuditProxy.Notify("bankservice", clientName, DateTime.Now, amounts);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}
