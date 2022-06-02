
namespace AIRMAM5.DBEntity.Models.Enums
{
    public class FormatString
    {
        /// <summary>
        /// 記錄操作-文字格式："@USER_ID={0};@USER_NAME={1};@DATA_TYPE={2};@RESULT={3}; "。
        ///　<para>　　 說明：{0}=帳號 , {1}=顯示名稱 , {2}=操作功能名稱(目錄使用者權限/角色群組...)。 , {3}=成功/失敗 </para>
        ///　<para>　　 範例：String.Format($"@USER_ID={UserName};@USER_NAME={fsNAME};@DATA_TYPE=目錄使用者權限;@RESULT={Result};") </para>
        /// </summary>
        public static string LogParams = "@USER_ID={0};@USER_NAME={1};@DATA_TYPE={2};@RESULT={3};";

        /// <summary>
        /// 記錄登入-文字格式 : "@USER_ID={0};@USER_NAME={1};@RESULT={2};
        /// <para>　　 說明：{0}=帳號 , {1}=顯示名稱 , {2}=成功/失敗　</para>
        /// <para>　　 範例：String.Format($"@USER_ID={UserName};@USER_NAME={fsNAME};@RESULT={Result};" </para>
        /// </summary>
        public static string LoginParams = "@USER_ID={0};@USER_NAME={1};@RESULT={2};";

        /// <summary>
        /// 記錄操作-文字格式 :  "@USER_ID={0};@USER_NAME={1};@DATA_TYPE={2};@TARGET={3};@RESULT={4};"。
        /// <para>　　 說明：{0}=帳號 , {1}=顯示名稱 , {2}=操作功能名稱(刪除檔案/角色群組...) , {3}=target(檔名 或其它) , {4}=Result(成功/失敗) </para>
        /// <para>　　 範例：String.Format($"@USER_ID={UserName};@USER_NAME={fsNAME};@DATA_TYPE=刪除檔案;@TARGET=影片File字幕檔案.txt;@RESULT=成功;"  </para>
        /// </summary>
        public static string LogTargetParams = "@USER_ID={0};@USER_NAME={1};@DATA_TYPE={2};@TARGET={3};@RESULT={4};";

        /// <summary>
        /// 使用頁面-文字格式 :  "@USER_ID={0};@USER_NAME={1};@WORK_PAGE={2};"。
        /// <para>　　 說明：{0}=帳號 , {1}=顯示名稱 , {2}=頁面名稱 </para>
        /// <para>　　 範例：String.Format($"@USER_ID={UserName};@USER_NAME={fsNAME};@WORK_PAGE=頁面名稱"  </para>
        /// </summary>
        public static string UsePageParams = "@USER_ID={0};@USER_NAME={1};@WORK_PAGE={2};";

        /// <summary>
        /// 忘記密碼 郵件內容
        /// </summary>
        /// <param name="sysname">系統名稱, 如: AIRMAM媒資管理系統 </param>
        /// <param name="callbackUrl"></param>
        public static string ForgetPwdContent(string sysname, string callbackUrl)
        {
            string _body = "此郵件為【" + sysname + "】自動送出。<br/><br/>"
                        + " ------------------------- <br/>"
                        + "  請按 <a href=\"" + callbackUrl + "\">這裏</a> 重設密碼 <br/><br/>"
                        + " ※若您對本郵件沒有印象，煩請刪除本郵件。 <br/><br/>"
                        + " ------------------------- <br/>"
                        + "請勿直接回覆至此電子郵件信箱。 <br/>";
            return _body;
        }
        /// <summary>
        /// 還原密碼 郵件內容
        /// </summary>
        /// <param name="sysname">系統名稱, 如: AIRMAM媒資管理系統 </param>
        /// <param name="callbackUrl"></param>
        public static string RestorePwdContent(string sysname, string callbackUrl )
        {
            string _body = "此郵件為【" + sysname + "】自動送出。<br/><br/>"
                        + " ------------------------- <br/>"
                        + " 您申請還原【" + sysname + "】登入密碼, <br/>"
                        + "  請按 <a href=\"" + callbackUrl + "\">這裏</a> 重設密碼 <br/><br/>"
                        + " ※若您對本郵件沒有印象，煩請刪除本郵件。 <br/><br/>"
                        + " ------------------------- <br/>"
                        + "請勿直接回覆至此電子郵件信箱。 <br/>";
            return _body;
        }

        /// <summary>
        /// 電子信箱 驗證郵件內容
        /// </summary>
        /// <param name="sysname">系統名稱, 如: AIRMAM媒資管理系統 </param>
        /// <param name="callbackUrl"></param>
        /// <param name="email"></param>
        /// <param name="code"></param>
        /// <param name="username">使用者帳號 </param>
        public static string ConfirmEmailContent(string sysname, string callbackUrl, string email, string code, string username)
        {
            string _body = "此郵件為【" + sysname + "】自動送出。<br/><br/>"
                        + " ------------------------- <br/>"
                        + " 確認 " + email + " 是您的電子郵件。<br/>"
                        + " 你可能會被要求輸入這個確認碼：" + code + " <br/>"
                        + "  請按 <a href=\"" + callbackUrl + "\">這裏</a> 登入系統 <br/><br/>"
                        + " ※若您對本郵件沒有印象，煩請刪除本郵件。 <br/><br/>"
                        + " ------------------------- <br/>"
                        + "請勿直接回覆至此電子郵件信箱。 <br/>";
            return _body;
        }

        /// <summary>
        /// 變更電子信箱 驗證郵件內容
        /// </summary>
        /// <param name="sysname">系統名稱, 如: AIRMAM媒資管理系統 </param>
        /// <param name="callbackUrl"></param>
        /// <param name="email"></param>
        /// <param name="code"></param>
        /// <param name="username">使用者帳號 </param>
        public static string ConfirmChangeEmailContent(string sysname, string callbackUrl, string email, string code, string username)
        {
            string _body = "此郵件為【" + sysname + "】自動送出。<br/><br/>"
                        + " ------------------------- <br/>"
                        + " 您申請變更【" + sysname + "】帳號電子郵件，"
                        + " 確認 " + email + " 是您帳號(" + username + ")的電子郵件。<br/>"
                        + " 你可能會被要求輸入這個確認碼：" + code + " <br/>"
                        + "  請按 <a href=\"" + callbackUrl + "\">這裏</a> 登入系統 <br/><br/>"
                        + " ※若您對本郵件沒有印象，煩請刪除本郵件。 <br/><br/>"
                        + " ------------------------- <br/>"
                        + "請勿直接回覆至此電子郵件信箱。 <br/>";
            return _body;
        }


        /// <summary>
        /// 註冊 郵件內容
        /// </summary>
        /// <param name="sysname">系統名稱, 如: AIRMAM媒資管理系統 </param>
        /// <param name="callbackUrl"></param>
        /// <param name="email"></param>
        /// <param name="code"></param>
        /// <param name="username">使用者帳號 </param>
        public static string RegisterContent(string sysname, string callbackUrl, string email, string code, string username)
        {
            string _body = "此郵件為【" + sysname + "】自動送出。<br/><br/>"
                        + " ------------------------- <br/>"
                        + " 您已成功註冊【" + sysname + "】帳號(" + username + "), <br/>"
                        + " 確認 " + email + " 是您的電子郵件。<br/><br/>"
                        + " 你可能會被要求輸入這個確認碼：" + code + " <br/>"
                        + "  請按 <a href=\"" + callbackUrl + "\">這裏</a> 登入系統 <br/><br/>"
                        + " ※若您對本郵件沒有印象，煩請刪除本郵件。 <br/><br/>"
                        + " ------------------------- <br/>"
                        + "請勿直接回覆至此電子郵件信箱。 <br/>";
            return _body;
        }
    }
}
