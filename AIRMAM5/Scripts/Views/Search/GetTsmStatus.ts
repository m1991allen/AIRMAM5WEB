import { FileStatusResult } from '../../Models/Interface/Search/ISearchResponseVideoModel';
import { MediaType } from '../../Models/Enum/MediaType';
import { IResponse } from '../../Models/Interface/Shared/IResponse';
import { PostJXR } from '../../Models/Function/Ajax';
import { Logger } from '../../Models/Class/LoggerService';

/**暫存GetTmsStatus請求 */
var GetTmsStatusRequest: Array<JQueryXHR> = [];

export const GetTmsStatus = (
    apiURL: string,
    mediaType: MediaType,
    fileNos: Array<string>
): Promise<IResponse<
    { mediaType: MediaType; fileNos: Array<string> },
    { TsmFileStatus: Array<FileStatusResult>; IsUseTSM: boolean }
>> => {
    Logger.log('Step2:TSM API參數', { mediaType, fileNos });
    return ((): Promise<
        IResponse<
            { mediaType: MediaType; fileNos: Array<string> },
            { TsmFileStatus: Array<FileStatusResult>; IsUseTSM: boolean }
        >
    > => {
        /**有分批取得議題,前一次的不能取消 */
        // if (GetTmsStatusRequest.length > 1) {
        //     for (let request of GetTmsStatusRequest.slice(0, -1)) {
        //         request.abort();
        //     }
        //     GetTmsStatusRequest = [];
        //     initSetting.ShowLog && console.log('Step2-2:tsm api abort');
        // }
        return new Promise((resolve, reject) => {
            if (fileNos.length > 0) {
                const newTmsRequest = PostJXR<{ mediaType: MediaType; fileNos: Array<string> }>(
                    apiURL,
                    { mediaType: mediaType, fileNos: fileNos },
                    false
                );
                GetTmsStatusRequest.push(newTmsRequest);
                newTmsRequest
                    .then(json => {
                        Logger.log('Step3:TSM API　新響應成功', json);
                        const data = json;
                        resolve(data);
                    })
                    .catch(data => {
                        Logger.error('Step3:TSM API　新響應失敗', data);
                        reject(data);
                    });
            }
        });
    })();
};
