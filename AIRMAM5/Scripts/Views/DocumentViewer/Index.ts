import { DocumentPostMessageModel } from '../../Models/Interface/DocumentViewer/DocumentPostMessageModal';

const $DocViewer: HTMLElement = document.getElementById('divDocViewer');
const subjectId: string = $DocViewer.getAttribute('data-SubjectId');
const fileNO: string = $DocViewer.getAttribute('data-FileNo');
const fileName: string = $DocViewer.getAttribute('data-ViewFileName');
const api: string = $DocViewer.getAttribute('data-API');

var popup = window.top;
/**當頁面卸載或關閉後,傳送訊息給Home通知刪除事件 */
popup.addEventListener('beforeunload', function(event) {
    event.preventDefault();
    popup.postMessage(
        <DocumentPostMessageModel>{
            eventid: 'DeleteDocViewer',
            subjectId: subjectId,
            fileNO: fileNO,
            fileName: fileName,
            api: api,
        },
        '/'
    );
    return;
});
