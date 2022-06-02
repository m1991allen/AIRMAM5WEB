
namespace AIRMAM5.DBEntity.Models.Hub
{
    /// <summary>
    /// SignalR 使用者連線ID 
    /// </summary>
    public class SignalrUserConnectId
    {
        /// <summary>
        /// 使用者userid
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Signalr Client Connection Hub Id
        /// </summary>
        public string SignalrConnectionId { get; set; }
    }

}
