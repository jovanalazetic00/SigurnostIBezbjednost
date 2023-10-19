using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public enum AuditEventTypes
    {
        CardRequestSuccess = 0,
        CardRequestFailure,
        RevokeRequestSuccess,
        RevokeRequestFailure,
        DepositSuccess,
        DepositFailure,
        WithdrawSuccess,
        WithdrawFailure,
        ResetPinSuccess,
        ResetPinFailure
    }
    public class AuditEvents
    {
        private static ResourceManager resourceManager = null;
        private static object resourceLock = new object();
        private static ResourceManager ResourceMgr
        {
            get
            {
                lock (resourceLock)
                {
                    if (resourceManager == null)
                    {
                        resourceManager = new ResourceManager(typeof(AuditEventFile).ToString(), Assembly.GetExecutingAssembly());
                    }
                    return resourceManager;
                }
            }
        }

        public static string CardRequestSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.CardRequestSuccess.ToString());
            }
        }

        public static string CardRequestFailure
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.CardRequestFailure.ToString());
            }
        }

        public static string RevokeRequestSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.RevokeRequestSuccess.ToString());
            }
        }

        public static string RevokeRequestFailure
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.RevokeRequestFailure.ToString());
            }
        }

        public static string DepositSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.DepositSuccess.ToString());
            }
        }

        public static string DepositFailure
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.DepositFailure.ToString());
            }
        }

        public static string WithdrawSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.WithdrawSuccess.ToString());
            }
        }

        public static string WithdrawFailure
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.WithdrawFailure.ToString());
            }
        }

        public static string ResetPinSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.ResetPinSuccess.ToString());
            }
        }

        public static string ResetPinFailure
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.ResetPinFailure.ToString());
            }
        }
    }
}
