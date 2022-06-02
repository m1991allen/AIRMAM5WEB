import { WarningMessage } from '../../Models/Function/Message';
import { initSetting } from '../../Models/initSetting';
import { DocumentOpenMessageModel } from '../../Models/Interface/DocumentViewer/DocumentPostMessageModal';

/*semantic ui 初始化事件*/
$('.tabs.menu .item').tab();
$('.dropdown').dropdown({ fullTextSearch: true });
$('.checkbox').checkbox();
$('.ui.embed').embed();

/**開啟文件檢視器,傳送訊息給Home通知開啟事件  */
$(document).on('click', "a[name='openDocumentViewer']", function(event) {
    event.preventDefault();
    const canOpenDoc=$(this).attr('data-showdoc')!=='false'?true:false;
    if(canOpenDoc){
        const href: string = [
            initSetting.WebUrl,
            $(this)
                .attr('data-href')
                .replace('~', ''),
        ].join('/');
        window.top.postMessage(
            <DocumentOpenMessageModel>{
                eventid: 'OpenDocViewer',
                href: href,
            },
            '/'
        );
    }else{
       WarningMessage('預覽功能不支援此類型的檔案');
    }
  
});
