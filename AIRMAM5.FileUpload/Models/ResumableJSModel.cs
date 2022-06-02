
namespace AIRMAM5.FileUpload.Models
{
    /// <summary>
    /// 前端上傳進度套件Resumable.js參數 MODEL
    /// </summary>
    public class ResumableJSModel
    {
        /// <summary>
        /// ResumableChunkNumber
        /// </summary>
        public int ResumableChunkNumber { get; set; }

        /// <summary>
        /// ResumableFilename
        /// </summary>
        public string ResumableFilename { get; set; }

        /// <summary>
        /// ResumableIdentifier
        /// </summary>
        public string ResumableIdentifier { get; set; }

        /// <summary>
        /// ResumableChunkSize
        /// </summary>
        public long ResumableChunkSize { get; set; }

        /// <summary>
        /// ResumableTotalSize
        /// </summary>
        public double ResumableTotalSize { get; set; }
    }

}