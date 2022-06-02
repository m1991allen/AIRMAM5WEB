/**
 * Http狀態(與.NET相同)
 */
export const enum HttpStatusCode{
        /**  相當於 HTTP 狀態 100。 System.Net.HttpStatusCode.Continue 指示用戶端可以繼續它的要求。*/
        Continue = 100,
        /** 相當於 HTTP 狀態 101。 System.Net.HttpStatusCode.SwitchingProtocols 指示正在變更通訊協定版本或通訊協定。*/
        SwitchingProtocols = 101,
        /**  相當於 HTTP 狀態 200。 System.Net.HttpStatusCode.OK 指示要求成功，並且要求的資訊在回應中。*/
        OK = 200,
        /**  相當於 HTTP 狀態 201。 System.Net.HttpStatusCode.Created 指示在傳送回應之前，要求導致新資源的建立。*/
        Created = 201,
        /**   相當於 HTTP 狀態 202。 System.Net.HttpStatusCode.Accepted 表示已接受要求進行進一步處理。*/
        Accepted = 202,
        /**  相當於 HTTP 狀態 203。 System.Net.HttpStatusCode.NonAuthoritativeInformation 指示傳回的中繼資訊來自快取備份而非原始伺服器，
         *   因此可能不正確。*/
        NonAuthoritativeInformation = 203,
        /**  相當於 HTTP 狀態 204。 System.Net.HttpStatusCode.NoContent 表示已成功處理要求，並且回應預設為空白。*/
        NoContent = 204,
        /**  相當於 HTTP 狀態 205。 System.Net.HttpStatusCode.ResetContent 指示用戶端應該重新設定 (非重新載入) 目前的資源。*/
        ResetContent = 205,
        /** 相當於 HTTP 狀態 206。 System.Net.HttpStatusCode.PartialContent 表示回應是包括位元組範圍之 GET 要求的部分回應。*/
        PartialContent = 206,
        /**  相當於 HTTP 狀態 300。 System.Net.HttpStatusCode.MultipleChoices 指示要求的資訊有多種表示法。 
          *  預設動作是將這個狀態視為重新導向並跟隨在與這個回應相關的Location 標頭內容之後。*/
        MultipleChoices = 300,
        /** 相當於 HTTP 狀態 300。 System.Net.HttpStatusCode.Ambiguous 指示要求的資訊有多種表示法。 
         *  預設動作是將這個狀態視為重新導向並跟隨在與這個回應相關的Location 標頭內容之後。*/
        Ambiguous = 300,
        /** 相當於 HTTP 狀態 301。 System.Net.HttpStatusCode.MovedPermanently 指示要求的資訊已經移至 Location
          * 標頭中指定的 URI。 在接收這個狀態時，預設動作會跟隨與回應相關的 Location 標頭。*/
        MovedPermanently = 301,
        /** 相當於 HTTP 狀態 301。 System.Net.HttpStatusCode.Moved 指示要求的資訊已經移至 Location 標頭中指定的
          * URI。 在接收這個狀態時，預設動作會跟隨與回應相關的 Location 標頭。 當原始的要求方法是 POST 時，重新導向的要求將使用 GET 方法。*/
        Moved = 301,
        /** 相當於 HTTP 狀態 302。 System.Net.HttpStatusCode.Found 指示要求的資訊位於 Location 標頭中所指定的 URI。
          * 在接收這個狀態時，預設動作會跟隨與回應相關的 Location 標頭。 當原始的要求方法是 POST 時，重新導向的要求將使用 GET 方法。*/
        Found = 302,
        /** 相當於 HTTP 狀態 302。 System.Net.HttpStatusCode.Redirect 指示要求的資訊位於 Location 標頭中所指定的
          * URI。 在接收這個狀態時，預設動作會跟隨與回應相關的 Location 標頭。 當原始的要求方法是 POST 時，重新導向的要求將使用 GET 方法。*/
        Redirect = 302,
        /**相當於 HTTP 狀態 303。 System.Net.HttpStatusCode.SeeOther 自動將用戶端重新導向至 Location 標頭中指定的
          * URI，做為 POST 的結果。 Location 標頭所指定的資源要求，將使用 GET 進行處理。*/
        SeeOther = 303,
       /** 相當於 HTTP 狀態 303。 System.Net.HttpStatusCode.RedirectMethod 自動將用戶端重新導向至 Location 標頭中指定的 URI，做為 POST 的結果。
         * Location 標頭所指定的資源要求，將使用 GET 進行處理。 */
        RedirectMethod = 303,
        /**相當於 HTTP 狀態 304。 System.Net.HttpStatusCode.NotModified 指示用戶端的快取備份已經是最新的。 不傳輸資源的內容。 */
        NotModified = 304,
        /** 相當於 HTTP 狀態 305。 System.Net.HttpStatusCode.UseProxy 指示要求應該使用位於 Location 標題中所指定 URI 的 Proxy 伺服器。 */
        UseProxy = 305,
        /**相當於 HTTP 狀態 306。 System.Net.HttpStatusCode.Unused 是 HTTP/1.1 規格未完全指定的建議擴充。 */
        Unused = 306,
        /**相當於 HTTP 狀態 307。 System.Net.HttpStatusCode.TemporaryRedirect 指示要求資訊位於 Location標頭中所指定的 URI。
         *  在接收這個狀態時，預設動作會跟隨與回應相關的 Location 標頭。當原始的要求方法是 POST 時，重新導向的要求也將使用POST 方法。 */  
        TemporaryRedirect = 307,
        /** 相當於 HTTP 狀態 307。 System.Net.HttpStatusCode.RedirectKeepVerb 表示要求資訊位於 Location 標頭中所指定的 URI。
         *  在接收這個狀態時，預設動作會跟隨與回應相關的 Location 標頭。當原始的要求方法是 POST 時，重新導向的要求也將使用 POST 方法。 */
        RedirectKeepVerb = 307,
        /**相當於 HTTP 狀態 400。 System.Net.HttpStatusCode.BadRequest 指示伺服器無法了解要求。 
         * 當沒有其他適用的錯誤，或者如果確實的錯誤是未知的或沒有自己的錯誤碼時，傳送System.Net.HttpStatusCode.BadRequest。 **/
        BadRequest = 400,
        /**相當於 HTTP 狀態 401。 System.Net.HttpStatusCode.Unauthorized 指示要求的資源需要驗證。
          * WWW-Authenticate標頭包含如何執行驗證的詳細資料。 */ 
        Unauthorized = 401,
        /**相當於 HTTP 狀態 402。 System.Net.HttpStatusCode.PaymentRequired 保留供日後使用。 */
        PaymentRequired = 402,
        /**相當於 HTTP 狀態 403。 System.Net.HttpStatusCode.Forbidden 指示伺服器拒絕處理要求。 */
        Forbidden = 403,
        /**相當於 HTTP 狀態 404。 System.Net.HttpStatusCode.NotFound 指示要求的資源不存在於伺服器。 */
        NotFound = 404,
        /**相當於 HTTP 狀態 405。 System.Net.HttpStatusCode.MethodNotAllowed 指示在要求的資源上不允許該要求方法(POST 或 GET)。 */ 
        MethodNotAllowed = 405,
        /** 相當於 HTTP 狀態 406。 System.Net.HttpStatusCode.NotAcceptable 指示用戶端已經指示將不接受任何可用資源表示的 Accept 標頭。 */
        NotAcceptable = 406,
        /** 相當於 HTTP 狀態 407。 System.Net.HttpStatusCode.ProxyAuthenticationRequired 指示要求的Proxy 需要驗證。
          * Proxy 驗證標頭包含如何執行驗證的詳細資料。 */
        ProxyAuthenticationRequired = 407,
        /** 相當於 HTTP 狀態 408。 System.Net.HttpStatusCode.RequestTimeout 指示用戶端的要求未在伺服器期待要求時傳送。 */
        RequestTimeout = 408,
        /** 相當於 HTTP 狀態 409。 System.Net.HttpStatusCode.Conflict 指示因為伺服器上的衝突而無法完成要求。 */
        Conflict = 409,
        /** 相當於 HTTP 狀態 410。 System.Net.HttpStatusCode.Gone 指示要求的資源已不能再使用。 */
        Gone = 410,
        /** 相當於 HTTP 狀態 411。 System.Net.HttpStatusCode.LengthRequired 指示遺漏要求的 Content-Length 標頭。 */
        LengthRequired = 411,
       /** 相當於 HTTP 狀態 412。 System.Net.HttpStatusCode.PreconditionFailed 指示這個要求的條件設定失敗，
         * 並且無法執行要求。使用條件式要求標頭 (例如 If-Match、If-None-Match 或 If-Unmodified-Since) 設定條件。 */  
        PreconditionFailed = 412,
        /**相當於 HTTP 狀態 413。 System.Net.HttpStatusCode.RequestEntityTooLarge 指示要求太大，伺服器無法處理。 */
        RequestEntityTooLarge = 413,
        /** 相當於 HTTP 狀態 414。 System.Net.HttpStatusCode.RequestUriTooLong 指示 URI 過長 */
        RequestUriTooLong = 414,
        /**  相當於 HTTP 狀態 415。 System.Net.HttpStatusCode.UnsupportedMediaType 指示要求是不支援的類型。 */
        UnsupportedMediaType = 415,
        /**相當於 HTTP 狀態 416。 System.Net.HttpStatusCode.RequestedRangeNotSatisfiable 表示無法傳回資源所要求的資料範圍，
          *可能是因為範圍的開頭在資源的開頭之前，或是範圍的結尾在資源的結尾之後。 */  
        RequestedRangeNotSatisfiable = 416,
        /**相當於 HTTP 狀態 417。 System.Net.HttpStatusCode.ExpectationFailed 指示在 Expect 標頭中所指定的預期項目不符合伺服器的要求。 */
        ExpectationFailed = 417,
        /** 相當於 HTTP 狀態 426。 System.Net.HttpStatusCode.UpgradeRequired 指示用戶端應該切換至不同的通訊協定，例如 TLS/1.0。 */
        UpgradeRequired = 426,
        /** 相當於 HTTP 狀態 500。 System.Net.HttpStatusCode.InternalServerError 指示伺服器上已經發生泛用錯誤。 */
        InternalServerError = 500,
        /**相當於 HTTP 狀態 501。 System.Net.HttpStatusCode.NotImplemented 指示伺服器不支援要求的功能。 */
        NotImplemented = 501,
        /** 相當於 HTTP 狀態 502。 System.Net.HttpStatusCode.BadGateway 表示中繼 Proxy 伺服器收到其他 Proxy 或原始伺服器的錯誤回應。 */
        BadGateway = 502,
        /** 相當於 HTTP 狀態 503。 System.Net.HttpStatusCode.ServiceUnavailable 表示伺服器暫時無法使用，通常是因為高負載或維護的緣故。 */
        ServiceUnavailable = 503,
        /**相當於 HTTP 狀態 504。 System.Net.HttpStatusCode.GatewayTimeout 指示中繼 Proxy 伺服器在等候來自其他Proxy 或原始伺服器的回應時已逾時。 */
        GatewayTimeout = 504,
        /**相當於 HTTP 狀態 505。 System.Net.HttpStatusCode.HttpVersionNotSupported 指示伺服器不支援要求的 HTTP 版本 */
        HttpVersionNotSupported = 505
}