using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class Audit : IDisposable
    {
        public static EventLog customLog = null;
        const string SourceName = "Manager.Audit";
        const string LogName = "BankLogs";

        static Audit()   //ono sto se ispisuje u event viweru, uvijek je isto samo se mijenja ono sto prosledjujem 
        {
            try
            {
                /// create customLog handle
                if (!EventLog.SourceExists(SourceName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);
                }

                customLog = new EventLog(LogName, Environment.MachineName, SourceName);

            }
            catch (Exception e)
            {
                customLog = null;
                Console.WriteLine("Error while trying to create log handle. Error = {0}", e.Message);
            }
        }

        public static void CardRequestSuccess(string userName)
        {
            string msg = AuditEvents.CardRequestSuccess;
            if (customLog != null)
            {
                customLog.WriteEntry(msg.Replace("{0}", userName), EventLogEntryType.Information, (int)AuditEventTypes.CardRequestSuccess);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.CardRequestSuccess));
            }
        }

        public static void CardRequestFailure(string userName, string reason)
        {
            string msg = AuditEvents.CardRequestFailure;
            if (customLog != null)
            {
                customLog.WriteEntry(String.Format(msg, userName, reason), EventLogEntryType.Information, (int)AuditEventTypes.CardRequestFailure);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.CardRequestFailure));
            }
        }

        public static void RevokeRequestSuccess(string userName)
        {
            string msg = AuditEvents.RevokeRequestSuccess;
            if (customLog != null)
            {
                customLog.WriteEntry(msg.Replace("{0}", userName), EventLogEntryType.Information, (int)AuditEventTypes.RevokeRequestSuccess);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.RevokeRequestSuccess));
            }
        }

        public static void RevokeRequestFailure(string userName, string reason)
        {
            string msg = AuditEvents.RevokeRequestFailure;
            if (customLog != null)
            {
                customLog.WriteEntry(String.Format(msg, userName, reason), EventLogEntryType.Information, (int)AuditEventTypes.RevokeRequestFailure);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.RevokeRequestFailure));
            }
        }

        public static void DepositSuccess(string userName, float amount)
        {
            string msg = AuditEvents.DepositSuccess;
            if (customLog != null)
            {
                customLog.WriteEntry(String.Format(msg, userName, amount), EventLogEntryType.Information, (int)AuditEventTypes.DepositSuccess);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.DepositSuccess));
            }
        }

        public static void DepositFailure(string userName, string reason)
        {
            string msg = AuditEvents.DepositFailure;
            if (customLog != null)
            {
                customLog.WriteEntry(String.Format(msg, userName, reason), EventLogEntryType.Information, (int)AuditEventTypes.DepositFailure);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.DepositFailure));
            }
        }

        public static void WithdrawSuccess(string userName, float amount)
        {
            string msg = AuditEvents.WithdrawSuccess;
            if (customLog != null)
            {
                customLog.WriteEntry(String.Format(msg, userName, amount), EventLogEntryType.Information, (int)AuditEventTypes.WithdrawSuccess);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.WithdrawSuccess));
            }
        }

        public static void WithdrawFailure(string userName, string reason)
        {
            string msg = AuditEvents.WithdrawFailure;
            if (customLog != null)
            {
                customLog.WriteEntry(String.Format(msg, userName, reason), EventLogEntryType.Information, (int)AuditEventTypes.WithdrawFailure);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.WithdrawFailure));
            }
        }

        public static void ResetPinSuccess(string userName)
        {
            string msg = AuditEvents.ResetPinSuccess;
            if (customLog != null)
            {
                customLog.WriteEntry(String.Format(msg, userName), EventLogEntryType.Information, (int)AuditEventTypes.ResetPinSuccess);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.ResetPinSuccess));
            }
        }

        public static void ResetPinFailure(string userName, string reason)
        {
            string msg = AuditEvents.ResetPinFailure;
            if (customLog != null)
            {
                customLog.WriteEntry(String.Format(msg, userName, reason), EventLogEntryType.Information, (int)AuditEventTypes.ResetPinFailure);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.ResetPinFailure));
            }
        }

        public void Dispose()
        {
            if (customLog != null)
            {
                customLog.Dispose();
                customLog = null;
            }
        }
    }
}
