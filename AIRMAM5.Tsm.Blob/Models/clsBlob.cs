using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace AIRMAM5.Tsm.Blob.Models
{
    public class clsBlob
    {
        public string container { get; set; }
        public string name { get; set; }
        public clsPROPERTIES properties { get; set; }
        public string rehydrationStatus { get; set; }

        public class clsPROPERTIES
        {
            public string blobTier { get; set; }
            public long contentLength { get; set; }
        }
    /*
    {
      "container": "hvideo",
      "content": "",
      "deleted": false,
      "encryptedMetadata": null,
      "encryptionKeySha256": null,
      "encryptionScope": null,
      "metadata": {},
      "name": "2020/07/21/2020721C04M1.mxf",
      "properties": {
        "appendBlobCommittedBlockCount": null,
        "blobTier": "Archive",
        "blobTierChangeTime": "2020-07-23T07:32:23+00:00",
        "blobTierInferred": null,
        "blobType": "BlockBlob",
        "contentLength": 454805148,
        "contentRange": null,
        "contentSettings": {
          "cacheControl": null,
          "contentDisposition": "",
          "contentEncoding": null,
          "contentLanguage": null,
          "contentMd5": "BLIJ7i3SJUVv3MZSmzcGCQ==",
          "contentType": "application/octet-stream"
        },
        "copy": {
          "completionTime": null,
          "destinationSnapshot": null,
          "id": null,
          "incrementalCopy": null,
          "progress": null,
          "source": null,
          "status": null,
          "statusDescription": null
        },
        "creationTime": "2020-07-23T07:08:32+00:00",
        "deletedTime": null,
        "etag": "\"0x8D82ED735CC0E67\"",
        "lastModified": "2020-07-23T07:08:32+00:00",
        "lease": {
          "duration": null,
          "state": "available",
          "status": "unlocked"
        },
        "pageBlobSequenceNumber": null,
        "pageRanges": null,
        "rehydrationStatus": "rehydrate-pending-to-hot",<!--重點-->
        "remainingRetentionDays": null,
        "serverEncrypted": true
      },
      "requestServerEncrypted": true,
      "snapshot": null
    }*/
    }

    /// <summary>
    /// 查詢檔案狀態參數
    /// </summary>
    public class clsFILE_STATUS_ARGS
    {
        /// <summary>
        /// 檔案路徑(TSM)
        /// </summary>
        public List<FILE_NO_TSM_PATH> lstFILE_TSM_PATH { get; set; }

        public class FILE_NO_TSM_PATH
        {
            /// <summary>
            /// 檔案編號
            /// </summary>
            public string fsFILE_NO { get; set; }
            /// <summary>
            /// 檔案在TSM路徑
            /// </summary>
            public string fsFILE_TSM_PATH { get; set; }
        }
    }

    /// <summary>
    /// 查詢檔案狀態結果
    /// </summary>
    public class clsFILE_STATUS_RESULT
    {
        /// <summary>
        /// 檔案編號
        /// </summary>
        public string FILE_NO { get; set; }
        /// <summary>
        /// 檔案狀態【0(檔案在磁帶);1(檔案在磁碟);2(檔案不存在);3(處理中)】
        /// </summary>
        public FileStatus FILE_STATUS { get; set; }
    }

    /// <summary>
    /// 檔案Recall參數
    /// </summary>
    public class clsRECALL_ARGS
    {
        public string fsFILE_PATH { get; set; }
    }

    /// <summary>
    /// 檔案狀態列舉
    /// </summary>
    public enum FileStatus
    {
        /// <summary>
        /// 檔案在Tape
        /// </summary>
        [Description("檔案在磁帶")]
        Tape,
        /// <summary>
        /// 檔案在Nearline
        /// </summary>
        [Description("檔案在磁碟")]
        NearLine,
        /// <summary>
        /// 錯誤
        /// </summary>
        [Description("錯誤")]
        Error,
        /// <summary>
        /// 處理中
        /// </summary>
        [Description("處理中")]
        Processing,
        /// <summary>
        /// 無檔案路徑
        /// </summary>
        [Description("檔案不存在")]
        NotExist,
        /// <summary>
        /// 檔案在線
        /// </summary>
        [Description("檔案在線")]
        Online,
        /// <summary>
        /// 檔案離線
        /// </summary>
        [Description("檔案離線")]
        Offline,
        /// <summary>
        /// 檔案深度離線
        /// </summary>
        [Description("檔案深度離線")]
        Offline_Deep
    }

    /// <summary>
    /// 檔案是否存在
    /// </summary>
    public class clsFILE_EXIST
    {
        public bool exists { get; set; }
    }
}