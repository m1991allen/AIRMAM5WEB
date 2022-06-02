import { ValidDate } from "../Function/Date";

/**增加自定義規則 */
$.fn.form.settings.rules.YYYYMMDD= function(param:string) {
    return ValidDate(param, 'YYYY/MM/DD');
}
/**表單驗證欄位規則 */
export const FormValidField = {
    Ann: {
        Search: {
            sdate: {
                identifier: 'sdate',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入上架日期(起)',
                    },
                ],
            },
            edate: {
                identifier: 'edate',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入上架日期(迄)',
                    },
                ],
            },
        },
        Create: {
            fsTITLE: {
                identifier: 'fsTITLE',
                rules: [{ type: 'empty', prompt: '必須輸入公告標題' }],
            },
            fdSDATE: {
                identifier: 'fdSDATE',
                rules: [{ type: 'empty', prompt: '必須輸入{name}' }],
            },
            fdEDATE: {
                identifier: 'fdEDATE',
                rules: [{ type: 'empty', prompt: '必須輸入{name}' }],
            },
            fsCONTENT: {
                identifier: 'fsCONTENT',
                rules: [{ type: 'empty', prompt: '必須輸入{name}' }],
            },
            fsDEPT: {
                identifier: 'fsDEPT',
                rules: [{ type: 'empty', prompt: '必須選擇{name}' }],
            },
            fsTYPE: {
                identifier: 'fsTYPE',
                rules: [{ type: 'empty', prompt: '必須選擇{name}}' }],
            },
            fnORDER: {
                identifier: 'fnORDER',
                rules: [{ type: 'integer', prompt: '請輸入正整數' }],
            },
        },
        Edit: {
            fsTITLE: {
                identifier: 'fsTITLE',
                rules: [{ type: 'empty', prompt: '必須輸入公告標題' }],
            },
            fdSDATE: {
                identifier: 'fdSDATE',
                rules: [{ type: 'empty', prompt: '必須輸入{name}' }],
            },
            fdEDATE: {
                identifier: 'fdEDATE',
                rules: [{ type: 'empty', prompt: '必須輸入{name}' }],
            },
            fsCONTENT: {
                identifier: 'fsCONTENT',
                rules: [{ type: 'empty', prompt: '必須輸入{name}' }],
            },
            fsDEPT: {
                identifier: 'fsDEPT',
                rules: [{ type: 'empty', prompt: '必須選擇{name}' }],
            },
            fsTYPE: {
                identifier: 'fsTYPE',
                rules: [{ type: 'empty', prompt: '必須選擇{name}}' }],
            },
            fnORDER: {
                identifier: 'fnORDER',
                rules: [{ type: 'integer', prompt: '請輸入正整數' }],
            },
        },
    },
    L_Login: {
        Search: {
            sdate: {
                identifier: 'sdate',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入登入日期(起)',
                    },
                ],
            },
            edate: {
                identifier: 'edate',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入登入日期(迄)',
                    },
                ],
            },
        },
    },
    L_Search: {
        Search: {
            sdate: {
                identifier: 'sdate',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            edate: {
                identifier: 'edate',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
        },
    },
    L_Log: {
        Search: {
            sdate: {
                identifier: 'sdate',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            edate: {
                identifier: 'edate',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
        },
    },
    L_Tran: {
        Search: {
            sdate: {
                identifier: 'sdate',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            edate: {
                identifier: 'edate',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
        },
    },
    L_Upload: {
        Search: {
            sdate: {
                identifier: 'sdate',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            edate: {
                identifier: 'edate',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
        },
        Edit: {
            fsPRIORITY: {
                identifier: 'fsPRIORITY',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                    {
                        type: 'maxLength[1]',
                        prompt: '長度僅限{ruleValue}位數',
                    },
                ],
            },
        },
    },
    MyBooking: {
        Search: {},
    },
    Booking: {
        Search: {},
        Edit: {
            Priority: {
                identifier: 'Priority',
                optional: true, //當不為空值時才驗證
                rules: [
                    {
                        type: 'integer[1..9]',
                        prompt: '{name}只能輸入1~9(1為最高、9為最低)',
                    },
                ],
            },
        },
    },
    Delete: {
        Search: {
            sdate: {
                identifier: 'StartDate',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            edate: {
                identifier: 'EndDate',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
        },
    },
    Subject: {
        CreateSubject: {//因為驗證規則只有Index使用,故此驗證方法放在Subject/Index.ts內的新增主題事件
            Title: {
                identifier: 'Title',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            DateInSubjId:{
                identifier:'DateInSubjId',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                    {
                        type: "YYYYMMDD[param]",
                        prompt: "日期格式必須為YYYY/MM/DD(例如:2020/01/01),且必須為有效日期"
                    }
                ],
            }
        },
        EditSubject: {
            Title: {
                identifier: 'Title',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
        },
        EditMedia: {
            Title: {
                identifier: 'Title',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                    {
                        optional:true,
                        type:'regExp[/^(?:(?!([\\\\/:*?<>|]+)).)*$/gm]',
                        prompt:'{name}不可以包含下列任意特殊字元 \\/:*?<>|'
                    }
                ],
            },
        },
        Paragraph: {
            BegTime: {
                identifier: 'BegTime',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            EndTime: {
                identifier: 'EndTime',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            Description: {
                identifier: 'Description',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            ParaDescription: {
                identifier: 'ParaDescription',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
        },
        Upload:{//因為驗證規則只有upload使用,故此驗證方法放在upload.ts內的upload事件
          DateInFileNo:{
                    identifier:'DateInFileNo',
                    rules: [
                        {
                            type:'empty',
                            prompt: '請輸入{name}',
                        },
                        {
                            optional:true,
                            type: "YYYYMMDD[param]",
                            prompt: "日期格式必須為YYYY/MM/DD(例如:2020/01/01),且必須為有效日期"
                        }
                    ],
          },
          ArcPreTempList:{
                    identifier:'ArcPreTempList',
                    rules:[
                        {
                           type:`checkPre[param]`,
                            prompt:`選擇預編詮釋資料為標題時,必須選擇預編詮釋資料!如果預編詮釋資料為"無",請先新增預編詮釋資料或選擇其他標題類型`
                            
                        }
                    ]
          },
          titletype:{
                    identifier:'titletype',
                    rules:[
                        {
                           type:`checkCustitle[param]`,
                            prompt:`需要輸入自訂標題`
                            
                        }
                    ]
          },
          custitle:{
            identifier:'custitle',
            optional:true,
            rules:[
                {
                    type: 'regExp[/^(?:(?!([\\\\/:*?<>|]+)).)*$/gm]',
                    prompt: '自訂標題不可以包含下列任意特殊字元 \\/:*?<>|'
                }
            ]
          }
        }
    },
    Materia: {},
    UserCode: {
        Search: {},
        Create: {
            fsTITLE: {
                identifier: 'fsTITLE',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
         ],
            },
            fsCODE_ID: {
                identifier: 'fsCODE_ID',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                    {
                        type: 'regExp[/^[A-Za-z0-9_-]+$/]',
                        prompt: '只能輸入英文字母、數字與符號-_',
                    },
                ],
            },
        },
        Edit: {
            fsTITLE: {
                identifier: 'fsTITLE',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            fsCODE_ID: {
                identifier: 'fsCODE_ID',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                    {
                        type: 'regExp[/^[A-Za-z0-9_-]+$/]',
                        prompt: '只能輸入英文字母、數字與符號-_',
                    },
                ],
            },
        },
        CreateSubCode: {
            'Code.fsCODE': {
                identifier: 'Code.fsCODE',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                    {
                        type: 'regExp[/^[A-Za-z0-9_-]+$/]',
                        prompt: '只能輸入英文字母、數字與符號-_',
                    },
                ],
            },
            'Code.fsENAME': {
                identifier: 'Code.fsENAME',
                rules: [
                    {
                        type: 'regExp[/^[A-Za-z0-9_ - ]+$|^$|^s$/]',
                        prompt: '只能輸入英文字母、數字',
                    },
                ],
            },
            'Code.fsNAME': {
                identifier: 'Code.fsNAME',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            'Code.fnORDER': {
                identifier: 'Code.fnORDER',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
        },
    },
    SysCode: {
        Search: {},
        Create: {
            fsTITLE: {
                identifier: 'fsTITLE',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            fsCODE_ID: {
                identifier: 'fsCODE_ID',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                    {
                        type: 'regExp[/^[A-Za-z0-9_-]+$/]',
                        prompt: '只能輸入英文字母、數字與符號-_',
                    },
                ],
            },
        },
        Edit: {
            fsTITLE: {
                identifier: 'fsTITLE',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            fsCODE_ID: {
                identifier: 'fsCODE_ID',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                    {
                        type: 'regExp[/^[A-Za-z0-9_-]+$/]',
                        prompt: '只能輸入英文字母、數字與符號-_',
                    },
                ],
            },
        },
        CreateSubCode: {
            'Code.fsCODE_ID': {
                identifier: 'Code.fsCODE_ID',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                    {
                        type: 'regExp[/^[A-Za-z0-9_-]+$/]',
                        prompt: '只能輸入英文字母、數字與符號-_',
                    },
                ],
            },
            'Code.fsENAME': {
                identifier: 'Code.fsENAME',
                rules: [
                    {
                        type: 'regExp[/^[A-Za-z0-9_ - ]+$|^$|^s$/]',
                        prompt: '只能輸入英文字母、數字',
                    },
                ],
            },
            'Code.fsNAME': {
                identifier: 'Code.fsNAME',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            'Code.fnORDER': {
                identifier: 'Code.fnORDER',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
        },
    },
    Report: {
        Search: {
            sdate: {
                identifier: 'StartDate',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            edate: {
                identifier: 'EndDate',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
        },
    },
    ArcPre: {
        Search: {},
        Edit: {
            fsNAME: {
                identifier: 'fsNAME',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            fsTITLE: {
                identifier: 'fsTITLE',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                    {
                        optional: true,
                        type: 'regExp[/^(?:(?!([\\\\/:*?<>|]+)).)*$/gm]',
                        prompt: '{name}不可以包含下列任意特殊字元 \\/:*?<>|'
                    }
                ],
            },
        },
        Create: {
            templetetype: {
                identifier: 'templetetype',
                rules: [
                    {
                        type: 'checked',
                        prompt: '請選擇樣板類型',
                    },
                ],
            },
            templetelayout: {
                identifier: 'templetelayout',
                rules: [
                    {
                        type: 'checked',
                        prompt: '請選擇樣板',
                    },
                ],
            },
        },
        SubCreate: {
            fsNAME: {
                identifier: 'fsNAME',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                    
                ],
            },
            fsTITLE: {
                identifier: 'fsTITLE',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                    {
                        optional:true,
                        type: 'regExp[/^(?:(?!([\\\\/:*?<>|]+)).)*$/gm]',
                        prompt: '{name}不可以包含下列任意特殊字元 \\/:*?<>|'
                    }
                ],
            },
        },
    },
    User: {
        Serch: {},
        Create: {
            UserName: {
                identifier: 'UserName',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            Name: {
                identifier: 'Name',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            //Tips_2020.3.13: 新增帳號時,不指定密碼。透過電子郵件驗證信,登入系統、再強制使用者變更密碼。
            // Password: {
            //     identifier: 'Password',
            //     rules: [
            //         {
            //             type: 'empty',
            //             prompt: '請輸入{name}',
            //         },
            //         {
            //             type: 'minLength[6]',
            //             prompt: '密碼的長度至少必須為{ruleValue}個字元',
            //         },
            //     ],
            // },
            // ConfirmPassword: {
            //     identifier: 'ConfirmPassword',
            //     rules: [
            //         {
            //             type: 'empty',
            //             prompt: '請輸入{name}',
            //         },
            //         {
            //             type: 'match[Password]',
            //             prompt: '{name}和密碼不相符',
            //         },
            //     ],
            // },
            Email: {
                identifier: 'Email',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                    {
                        type: 'email',
                        prompt: '{name}格式錯誤',
                    },
                ],
            },
            RoleList: {
                identifier: 'RoleList',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}至少選擇一個',
                    },
                ],
            },
            // BookingTargetPath: {
            //     identifier: 'BookingTargetPath',
            //     optional: true, //當不為空值時才驗證
            //     rules: [
            //         {
            //             type: 'url',
            //             prompt: '{name}必須為有效的路徑',
            //         },
            //     ],
            // },
        },
        Edit: {
            fsNAME: {
                identifier: 'fsNAME',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            fsEMAIL: {
                identifier: 'fsEMAIL',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                    {
                        type: 'email',
                        prompt: '{name}格式錯誤',
                    },
                ],
            },
            GroupList: {
                identifier: 'GroupList',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}至少選擇一個',
                    },
                ],
            },
        },
        ChangePassword: {
            CurrentPassword: {
                identifier: 'CurrentPassword',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            NewPassword: {
                identifier: 'NewPassword',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            ConfirmPassword: {
                identifier: 'ConfirmPassword',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                        optional: false,
                    },
                    {
                        type: 'match[NewPassword]',
                        prompt: '【{name}】必須與【新密碼】相同',
                        optional: true, //當不為空值時才驗證
                    },
                ],
            },
        },
    },
    VerifyBooking: {
        Search: {},
        Delete: {
            // Reason: {
            //     identifier: 'Reason',
            //     rules: [
            //         {
            //             type: 'empty',
            //             prompt: '請輸入{name}',
            //         },
            //     ],
            // },
            VerifyReason: {
                identifier: 'VerifyReason',
                rules: [
                    {
                        type: 'checked',
                        prompt: '至少選擇一個{name}',
                    },
                ],
            },
        },
    },
    Dir: {
        CreateNode: {
            fsNAME: {
                identifier: 'fsNAME',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            fnORDER: {
                identifier: 'fnORDER',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                    {
                        type: 'integer',
                        prompt: '請輸入正整數',
                    },
                ],
            },
            fnTEMP_ID_SUBJECT: {
                identifier: 'fnTEMP_ID_SUBJECT',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}至少需要選擇一種',
                    },
                ],
            },
            fnTEMP_ID_VIDEO: {
                identifier: 'fnTEMP_ID_VIDEO',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}至少需要選擇一種',
                    },
                ],
            },
            fnTEMP_ID_AUDIO: {
                identifier: 'fnTEMP_ID_AUDIO',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}至少需要選擇一種',
                    },
                ],
            },
            fnTEMP_ID_PHOTO: {
                identifier: 'fnTEMP_ID_PHOTO',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}至少需要選擇一種',
                    },
                ],
            },
            fnTEMP_ID_DOC: {
                identifier: 'fnTEMP_ID_DOC',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}至少需要選擇一種',
                    },
                ],
            },
            fsSHOWTYPE: {
                identifier: 'fsSHOWTYPE',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}需要選擇一種',
                    },
                ],
            },
        },
        EditNode: {
            fsNAME: {
                identifier: 'fsNAME',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            fnORDER: {
                identifier: 'fnORDER',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                    {
                        type: 'integer',
                        prompt: '請輸入正整數',
                    },
                ],
            },
            fnTEMP_ID_SUBJECT: {
                identifier: 'fnTEMP_ID_SUBJECT',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}至少需要選擇一種',
                    },
                ],
            },
            fnTEMP_ID_VIDEO: {
                identifier: 'fnTEMP_ID_VIDEO',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}至少需要選擇一種',
                    },
                ],
            },
            fnTEMP_ID_AUDIO: {
                identifier: 'fnTEMP_ID_AUDIO',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}至少需要選擇一種',
                    },
                ],
            },
            fnTEMP_ID_PHOTO: {
                identifier: 'fnTEMP_ID_PHOTO',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}至少需要選擇一種',
                    },
                ],
            },
            fnTEMP_ID_DOC: {
                identifier: 'fnTEMP_ID_DOC',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}至少需要選擇一種',
                    },
                ],
            },
            fsSHOWTYPE: {
                identifier: 'fsSHOWTYPE',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}需要選擇一種',
                    },
                ],
            },
        },
        DeleteNode: {},
        CreateGroupAuth:{
            fsGROUP_ID:{
                identifier:'fsGROUP_ID',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}需要選擇',
                    },
                ],
            }
        },
        CreateUserAuth: {
            fsLOGIN_ID:{
                identifier:'fsLOGIN_ID',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}需要選擇',
                    },
                ],
            }
        },
        EditAuth: {},
    },
    Synonym: {
        Search: {},
        Create: {
            fsTYPE: {
                identifier: 'fsTYPE',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '至少選擇一種{name}',
                    },
                ],
            },
            fsTEXT_LIST: {
                identifier: 'fsTEXT_LIST',
                rules: [
                    {
                        type: 'empty',
                        prompt: '至少輸入一個{name}',
                    },
                ],
            },
        },
        Edit: {
            fsTEXT_LIST: {
                identifier: 'fsTEXT_LIST',
                rules: [
                    {
                        type: 'empty',
                        prompt: '至少輸入一個同義詞',
                    },
                ],
            },
        },
    },
    Group: {
        Create: {
            fsNAME:{
                identifier:'fsNAME',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            fsDESCRIPTION:{
                identifier:'fsDESCRIPTION',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            }
        },
    },
    Search: {
        Edit: {
            Title: {
                identifier: 'Title',
                rules: [
                    {
                        type: 'empty',
                        prompt: '至少輸入一個{name}',
                    },
                    {
                        optional:true,
                        type: 'regExp[/^(?:(?!([\\\\/:*?<>|]+)).)*$/gm]',
                        prompt: '{name}不可以包含下列任意特殊字元 \\/:*?<>|'
                    }
                ],
            },
        },
    },
    ColTemplete: {
        CreateTemp: {
            fsNAME: {
                identifier: 'fsNAME',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            fsTABLE: {
                identifier: 'fsTABLE',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}至少選擇一個',
                    },
                ],
            },
        },
        AddStringField: {
            FieldName: {
                identifier: 'FieldName',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            FieldOrder: {
                identifier: 'FieldOrder',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            FieldLen: {
                identifier: 'FieldLen',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
        },
        AddNumberField: {
            FieldName: {
                identifier: 'FieldName',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            FieldOrder: {
                identifier: 'FieldOrder',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            FieldLen: {
                identifier: 'FieldLen',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
        },
        AddDateField: {
            FieldName: {
                identifier: 'FieldName',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            FieldOrder: {
                identifier: 'FieldOrder',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
        },
        AddCodeField: {
            FieldCodeId: {
                identifier: 'FieldCodeId',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}至少選擇一個',
                    },
                ],
            },
            FieldName: {
                identifier: 'FieldName',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            FieldOrder: {
                identifier: 'FieldOrder',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            fnCODE_CNT: {
                identifier: 'fnCODE_CNT',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}至少選擇一個',
                    },
                ],
            },
        },
    },
    Rule: {
        CreateRule: {
            RuleName: {
                identifier: 'RuleName',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            RuleCategory: {
                identifier: 'RuleCategory',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}至少選擇一個',
                    },
                ],
            },
        },
        CreateRuleItem: {
            RuleOrder: {
                identifier: 'RuleOrder',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
            RuleWhereClause: {
                identifier: 'RuleWhereClause',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}至少選擇一個',
                    },
                ],
            },
            RuleTable: {
                identifier: 'RuleTable',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}至少選擇一個',
                    },
                ],
            },
            RuleColumn: {
                identifier: 'RuleColumn',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}至少選擇一個',
                    },
                ],
            },
            RuleOperator: {
                identifier: 'RuleOperator',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}至少選擇一個',
                    },
                ],
            },
            RuleFilter: {
                identifier: 'RuleFilter',
                rules: [
                    {
                        type: 'minCount[1]',
                        prompt: '{name}至少選擇一個',
                    },
                ],
            },
            RuleInput: {
                identifier: 'RuleInput',
                rules: [
                    {
                        type: 'empty',
                        prompt: '請輸入{name}',
                    },
                ],
            },
        },
    },
    ArchiveMove: {},
    Tsm: {
        Search: {},
    },
    License: {
        Create: {
            LicenseCode: {
                identifier: 'LicenseCode',
                rules: [{ type: 'empty', prompt: '必須輸入版權代碼' }],
            },
            LicenseName: {
                identifier: 'LicenseName',
                rules: [{ type: 'empty', prompt: '必須輸入{name}' }],
            },
            // EndDate: {
            //     identifier: 'EndDate',
            //     rules: [{ type: 'empty', prompt: '必須輸入{name}' }],
            // },
            Order: {
                identifier: 'Order',
                rules: [{ type: 'integer', prompt: '請輸入正整數' }],
            },
        },
        Edit: {
            LicenseName: {
                identifier: 'LicenseName',
                rules: [{ type: 'empty', prompt: '必須輸入{name}' }],
            },
            Order: {
                identifier: 'Order',
                rules: [{ type: 'integer', prompt: '請輸入正整數' }],
            },
        },
    },
};
