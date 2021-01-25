using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace WinServices.Utils
{
    //https://stackoverflow.com/questions/5238133/getting-logged-on-username-from-a-service
    public class InteractiveUser
    {
        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSQueryUserToken(uint sessionId, out IntPtr token);

        [DllImport("kernel32.dll")]
        private static extern uint WTSGetActiveConsoleSessionId();

        private enum TOKEN_INFORMATION_CLASS
        {
            TokenUser = 1,
            TokenGroups,
            TokenPrivileges,
            TokenOwner,
            TokenPrimaryGroup,
            TokenDefaultDacl,
            TokenSource,
            TokenType,
            TokenImpersonationLevel,
            TokenStatistics,
            TokenRestrictedSids,
            TokenSessionId,
            TokenGroupsAndPrivileges,
            TokenSessionReference,
            TokenSandBoxInert,
            TokenAuditPolicy,
            TokenOrigin
        }

        public struct TOKEN_USER
        {
            public SID_AND_ATTRIBUTES User;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SID_AND_ATTRIBUTES
        {
            public IntPtr Sid;
            public int Attributes;
        }

        // Using IntPtr for pSID insted of Byte[]
        [DllImport("advapi32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ConvertSidToStringSid(
            IntPtr pSID,
            out IntPtr ptrSid);

        [DllImport("kernel32.dll")]
        private static extern IntPtr LocalFree(IntPtr hMem);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool GetTokenInformation(
            IntPtr tokenHandle,
            TOKEN_INFORMATION_CLASS tokenInformationClass,
            IntPtr tokenInformation,
            int tokenInformationLength,
            out int returnLength);

        private static string GetSID(IntPtr token)
        {
            int tokenInfLength = 0;
            string sidAsString = string.Empty;

            // first call gets lenght of TokenInformation
            GetTokenInformation(token, TOKEN_INFORMATION_CLASS.TokenUser, IntPtr.Zero, tokenInfLength, out tokenInfLength);

            IntPtr tokenInformation = Marshal.AllocHGlobal(tokenInfLength);
            bool result = GetTokenInformation(token, TOKEN_INFORMATION_CLASS.TokenUser, tokenInformation, tokenInfLength, out _);

            if (result) {
                TOKEN_USER tokenUser = (TOKEN_USER)Marshal.PtrToStructure(tokenInformation, typeof(TOKEN_USER));
                ConvertSidToStringSid(tokenUser.User.Sid, out IntPtr pstr);
                sidAsString = Marshal.PtrToStringAuto(pstr);
                LocalFree(pstr);
            }

            Marshal.FreeHGlobal(tokenInformation);

            return sidAsString;
        }

        public static string Account()
        {
            string account;
            if (WTSQueryUserToken(WTSGetActiveConsoleSessionId(), out IntPtr token)) {
                account = new SecurityIdentifier(GetSID(token)).Translate(typeof(NTAccount)).ToString();
            }
            else {
                int err = Marshal.GetLastWin32Error();
                account = err switch {
                    5 => "ERROR_ACCESS_DENIED",
                    87 => "ERROR_INVALID_PARAMETER",
                    1008 => "ERROR_NO_TOKEN",
                    1314 => "ERROR_PRIVILEGE_NOT_HELD",
                    7022 => "ERROR_CTX_WINSTATION_NOT_FOUND",
                    _ => string.Format("ERROR_{0}", err.ToString()),
                };
            }

            return account;
        }

        public static string Account(int sessionId)
        {
            string account;
            if (WTSQueryUserToken(Convert.ToUInt32(sessionId), out IntPtr token)) {
                account = new SecurityIdentifier(GetSID(token)).Translate(typeof(NTAccount)).ToString();
            }
            else {
                int err = Marshal.GetLastWin32Error();
                account = err switch {
                    5 => "ERROR_ACCESS_DENIED",
                    87 => "ERROR_INVALID_PARAMETER",
                    1008 => "ERROR_NO_TOKEN",
                    1314 => "ERROR_PRIVILEGE_NOT_HELD",
                    7022 => "ERROR_CTX_WINSTATION_NOT_FOUND",
                    _ => string.Format("ERROR_{0}", err.ToString()),
                };
            }

            return account;
        }
    }
}