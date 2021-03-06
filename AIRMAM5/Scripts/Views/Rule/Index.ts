import { EditButton, DeleteButton } from '../../Models/Templete/ButtonTemp';
import { RuleSearchModel } from '../../Models/Interface/Rule/RuleSearchModel';
import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { FormValidField } from '../../Models/Const/FormValid';
import { RuleMessageSetting } from '../../Models/MessageSetting';
import { IRuleController, RuleController } from '../../Models/Controller/RuleController';
import { tabulatorService } from '../../Models/Class/tabulatorService';
import { TabulatorSetting, initSetting } from '../../Models/initSetting';
import { RuleSearchListModel, RuleFrontEndListModel } from '../../Models/Interface/Rule/RuleSearchListModel';
import { StringEnum } from '../../Models/Enum/StringEnum';
import {
    SearchFormId,
    CreateModalId,
    CreateFormId,
    EditModalId,
    EditFormId,
    DeleteModalId,
    DeleteFormId,
} from '../../Models/Const/Const.';
import { Color } from '../../Models/Enum/ColorEnum';
import { RuleEnglishCategory } from '../../Models/Enum/RuleCategory';
import { getEnumKeyByEnumValue } from '../../Models/Function/KeyValuePair';
import { Filter } from '../../Models/Enum/Filter';
import { ModalTask, ShowModal } from '../../Models/Function/Modal';
import { GetDropdown, GetSelect } from '../../Models/Function/Element';
import { CreateRuleParamsModel } from '../../Models/Interface/Rule/CreateRuleParamsModel';
import { CreateSelect, CreateOption, CreateCheckbox } from '../../Models/Templete/FormTemp';
import { SelectListItem } from '../../Models/Interface/Shared/ISelectListItem';
import { TextInput, NumberInput } from '../../Models/Templete/InputTemp';
import { FilterTableModel } from '../../Models/Interface/Rule/FilterTableModel';
import { FieldInfo } from '../../Models/Interface/Rule/FieldInfo';
import { EditRuleFilterModel } from '../../Models/Interface/Rule/EditRuleFilterModel';
import { CheckForm } from '../../Models/Function/Form';
import { Label } from '../../Models/Templete/LabelTemp';
import { IsNULLorEmpty, IsNullorUndefined } from '../../Models/Function/Check';
import { ErrorMessage, SuccessMessage, WarningMessage } from '../../Models/Function/Message';
import { GetRuleModel } from '../../Models/Interface/Rule/GetRuleModel';
import { RuleDropdownGroup } from '../../Models/Interface/Rule/RuleDropdownGroup';
import { Logger } from '../../Models/Class/LoggerService';
import { ChineseRuleOperator } from '../../Models/Enum/RuleOperator';
import { IResponse } from '../../Models/Interface/Shared/IResponse';
import { RuleTableInheritModel } from '../../Models/Interface/Rule/RuleTableModel';
import { RuleEnabledStyle } from '../../Models/Enum/RuleEnableStyle';
import { CreateRuleModel } from '../../Models/Interface/Rule/CreateRuleModel';

/*=====================????????????====================================*/
var table: ItabulatorService;
var route: IRuleController = new RuleController();
/**???????????????????????????*/
const MaxRuleConditionLength: number = 5;
const valid = FormValidField.Rule;
const message = RuleMessageSetting;
/**?????????????????????????????????????????? */
var tempdata: CreateRuleParamsModel = <CreateRuleParamsModel>{};
const $RuleDropdown = GetDropdown(SearchFormId, 'condition');
const RuleInputId = '#RuleInput';
const CreateRuleAreaId = '#CreateRuleArea';
const $CreateRuleArea = $(CreateRuleAreaId);
const $CreateRuleTip = $('#CreateRuleTip');
const $CreateRuleBtn = $('button[name="createRule"]');
/**??????Modal??????*/
const prop = (key: keyof RuleFrontEndListModel): string => {
    return route.GetProperty<RuleFrontEndListModel>(key);
};
/*--------------------??????--------------------------------*/
/**???????????????????????? */
const GetDropdownGroups = (input: {
    category: RuleEnglishCategory;
    ruleInputId: string;
    formId: string;
}): RuleDropdownGroup => {
    const init: RuleDropdownGroup = {
        $OperatorUI: $(input.ruleInputId).find('div[name="FilterUI"]'),
        $RuleInput: $(input.ruleInputId),
        $RuleTableSelect: GetSelect(input.ruleInputId, 'RuleTable'),
        $RuleColumnSelect: GetSelect(input.ruleInputId, 'RuleColumn'),
        $RuleFilterSelect: GetSelect(input.ruleInputId, 'RuleFilter'),
        $RuleOperatorSelect: GetSelect(input.ruleInputId, 'RuleOperator'),
        $RuleTableDropdown: GetDropdown(input.ruleInputId, 'RuleTable'),
        $RuleColumnDropdown: GetDropdown(input.ruleInputId, 'RuleColumn'),
        $RuleOperatorDropdown: GetDropdown(input.ruleInputId, 'RuleOperator'),
        $CategoryDropdown: GetDropdown(input.formId, 'RuleCategory'),
    };
    return init;
};

/**??????:???????????????????????? */
const TaskInitDropdown = ($Elements: RuleDropdownGroup, $category: RuleEnglishCategory = null) => {
    const init = $Elements;
    const _category = $category == null ? RuleEnglishCategory.NotSpecify : $category;
    const $CreateSubModal = $('#CreateSubModal');
    route.GetCreateRuleParamsByType(_category).then(data => {
        tempdata = data;
        $CreateSubModal.find('.header').html(data.RuleName + '(??????: ' + data.RuleCategory + ') ???????????????'); //ADDED_2020/04/13
        //?????????????????????????????????
        if (data.TableList !== undefined && data.TableList.length > 0) {
            init.$RuleTableSelect.empty();
            $CreateSubModal.find('.ok.button').removeClass('disabled');
            for (let list of data.TableList) {
                const option = CreateOption(<SelectListItem>{
                    Value: list.TableName,
                    Text: list.TableDesc,
                });
                init.$RuleTableSelect.append(option);
            }
            //????????????????????????
            const firstList = data.TableList[0];
            if (firstList.Properties.length > 0) {
                for (let property of firstList.Properties) {
                    const option = CreateOption(<SelectListItem>{ Value: property.Column, Text: property.Desc }, {
                        attrName: 'data-type',
                        attrValue: property.Type,
                    });
                    init.$RuleColumnSelect.append(option);
                }
            } else {
                const option = CreateOption('none');
                init.$RuleColumnSelect.append(option);
                init.$RuleColumnDropdown.dropdown('refresh').addClass('disabled');
            }
            //???????????????????????????
            const firstProperty = firstList.Properties[0];
            if (firstProperty.ListItem.length > 0) {
                const newSelect = CreateSelect(firstProperty.ListItem, firstProperty.IsMultiple, 'RuleFilter');
                init.$OperatorUI.empty().append(newSelect);
                $(newSelect)
                    .dropdown('refresh')
                    .dropdown();
            } else {
                switch (firstProperty.Type.toLowerCase()) {
                    case 'string':
                        const StringInput = TextInput({
                            placeholder: `?????????${firstProperty.Desc}`,
                            name: 'RuleInput',
                        });
                        init.$OperatorUI.empty().html(StringInput);
                        break;
                    case 'int':
                        const IntInput = NumberInput({
                            placeholder: `?????????${firstProperty.Desc}`,
                            name: 'RuleInput',
                        });
                        init.$OperatorUI.empty().html(IntInput);
                        break;
                    default:
                        const IsFullsiteRule = firstList.TableName === StringEnum.All;
                        if (IsFullsiteRule) {
                            $Elements.$OperatorUI
                                .empty()
                                .html('????????????????????????????????????????????????????????????????????????????????????');
                        } else {
                            const Checkbox = CreateCheckbox({ label: firstProperty.Desc });
                            $Elements.$OperatorUI.empty().html(Checkbox);
                        }
                        break;
                }
            }
            //??????????????? ????????????
            init.$RuleOperatorSelect.empty();
            if (firstProperty.ListOperator.length > 0) {
                for (let opera of firstProperty.ListOperator) {
                    const option = CreateOption(<SelectListItem>opera);
                    init.$RuleOperatorSelect.append(option);
                }
                init.$RuleOperatorDropdown.removeClass('disabled');
            } else {
                const option = CreateOption('none');
                init.$RuleOperatorSelect.append(option);
                init.$RuleOperatorDropdown.addClass('disabled');
            }
        } else {
            init.$RuleOperatorSelect.empty().append(CreateOption('none'));
            init.$RuleColumnSelect.empty().append(CreateOption('none'));
            init.$RuleTableSelect.empty().append(CreateOption('none'));
            init.$RuleOperatorDropdown.dropdown('refresh').addClass('disabled');
            init.$RuleColumnDropdown.dropdown('refresh').addClass('disabled');
            init.$RuleTableDropdown.dropdown('refresh').addClass('disabled');
            $CreateSubModal.find('.ok.button').addClass('disabled');
        }
        init.$RuleTableDropdown.dropdown('refresh').removeClass('disabled');
        init.$RuleColumnDropdown.dropdown('refresh').removeClass('disabled');
    });
};
/**??????:?????????????????? */
const TaskChangeCategory = ($Elements: RuleDropdownGroup) => {
    $Elements.$CategoryDropdown.dropdown({
        onChange: function(value: string, text, $selectedItem) {
            createRules = [];
            $CreateRuleArea.empty();
            $CreateRuleTip.show();
            //----?????????????????????CreateMOdal OnShow??????
            route.GetCreateRuleParamsByType(<RuleEnglishCategory>value).then(data => {
                tempdata = data;
                //?????????????????????????????????
                if (data.TableList !== undefined && data.TableList.length > 0) {
                    $CreateRuleBtn.removeClass('disabled');
                    $Elements.$RuleTableSelect.empty(); //????????????????????????????????????
                    for (let list of data.TableList) {
                        const option = CreateOption(<SelectListItem>{ Value: list.TableName, Text: list.TableDesc });
                        $Elements.$RuleTableSelect.append(option);
                    }
                    $Elements.$RuleTableDropdown.removeClass('disabled');
                    //????????????????????????
                    const firstList = data.TableList[0];
                    $Elements.$RuleColumnSelect.empty();
                    if (firstList.Properties.length > 0) {
                        for (let property of firstList.Properties) {
                            const option = CreateOption(
                                <SelectListItem>{ Value: property.Column, Text: property.Desc },
                                {
                                    attrName: 'data-type',
                                    attrValue: property.Type,
                                }
                            );
                            $Elements.$RuleColumnSelect.append(option);
                        }
                        $Elements.$RuleColumnDropdown.removeClass('disabled');
                    } else {
                        const option = CreateOption('none');
                        $Elements.$RuleColumnSelect.append(option);
                        $Elements.$RuleColumnDropdown.addClass('disabled');
                    }
                    //???????????????????????????
                    const firstProperty = firstList.Properties[0];
                    if (firstProperty.ListItem.length > 0) {
                        const newSelect = CreateSelect(firstProperty.ListItem, firstProperty.IsMultiple, 'RuleFilter');
                        $Elements.$OperatorUI.empty().append(newSelect);
                        $(newSelect).dropdown('refresh');
                    } else {
                        switch (firstProperty.Type.toLowerCase()) {
                            case 'string':
                                const StringInput = TextInput({
                                    placeholder: `?????????${firstProperty.Desc}`,
                                    name: 'RuleInput',
                                });
                                $Elements.$OperatorUI.empty().html(StringInput);
                                break;
                            case 'int':
                                const IntInput = NumberInput({
                                    placeholder: `?????????${firstProperty.Desc}`,
                                    name: 'RuleInput',
                                });
                                $Elements.$OperatorUI.empty().html(IntInput);
                                break;
                            default:
                                const IsFullsiteRule = firstList.TableName === StringEnum.All;
                                if (IsFullsiteRule) {
                                    $Elements.$OperatorUI
                                        .empty()
                                        .html('????????????????????????????????????????????????????????????????????????????????????');
                                } else {
                                    const Checkbox = CreateCheckbox({ label: firstProperty.Desc });
                                    $Elements.$OperatorUI.empty().html(Checkbox);
                                }

                                break;
                        }
                    }
                    //??????????????? ????????????
                    $Elements.$RuleOperatorSelect.empty();
                    if (firstProperty.ListOperator.length > 0) {
                        for (let opera of firstProperty.ListOperator) {
                            const option = CreateOption(<SelectListItem>opera);
                            $Elements.$RuleOperatorSelect.append(option);
                        }
                        $Elements.$RuleOperatorDropdown.removeClass('disabled');
                    } else {
                        const option = CreateOption('none');
                        $Elements.$RuleOperatorSelect.append(option);
                        $Elements.$RuleOperatorDropdown.addClass('disabled');
                    }
                } else {
                    $CreateRuleBtn.addClass('disabled');
                    $Elements.$RuleOperatorSelect.empty().append(CreateOption('none'));
                    $Elements.$RuleColumnSelect.empty().append(CreateOption('none'));
                    $Elements.$RuleTableSelect.empty().append(CreateOption('none'));
                    $Elements.$RuleOperatorDropdown.addClass('disabled');
                    $Elements.$RuleColumnDropdown.addClass('disabled');
                    $Elements.$RuleTableDropdown.addClass('disabled');
                }
                $Elements.$RuleTableDropdown.dropdown('refresh');
                $Elements.$RuleColumnDropdown.dropdown('refresh');
            });
        },
    });
};
/**??????:??????????????? */
const TaskChangeTable = ($Elements: RuleDropdownGroup) => {
    /**??????????????????--????????????????????????????????? */
    $Elements.$RuleTableDropdown.dropdown({
        onChange: function(value, text, $selectedItem) {
            const temp_Data = tempdata.TableList.filter(item => item.TableName == value);
            const table_Data = temp_Data !== undefined ? temp_Data[0] : <FilterTableModel>{};
            const firstProperty = table_Data.Properties !== undefined ? table_Data.Properties[0] : <FieldInfo>{};
            //????????????
            $Elements.$RuleColumnSelect.empty();
            for (let column of table_Data.Properties) {
                const option = CreateOption(<SelectListItem>{ Value: column.Column, Text: column.Desc }, {
                    attrName: 'data-type',
                    attrValue: column.Type,
                });
                $Elements.$RuleColumnSelect.append(option);
            }
            //???????????????
            if (firstProperty.ListItem.length > 0) {
                const newSelect = CreateSelect(firstProperty.ListItem, firstProperty.IsMultiple, 'RuleFilter');
                $Elements.$OperatorUI.empty().append(newSelect);
                $(newSelect).dropdown();
            } else {
                switch (firstProperty.Type.toLowerCase()) {
                    case 'string':
                        const StringInput = TextInput({
                            placeholder: `?????????${firstProperty.Desc}`,
                            name: 'RuleInput',
                        });
                        $Elements.$OperatorUI.empty().html(StringInput);
                        break;
                    case 'int':
                        const IntInput = NumberInput({ placeholder: `?????????${firstProperty.Desc}`, name: 'RuleInput' });
                        $Elements.$OperatorUI.empty().html(IntInput);
                        break;
                    default:
                        const IsFullsiteRule = value === StringEnum.All;
                        if (IsFullsiteRule) {
                            $Elements.$OperatorUI
                                .empty()
                                .html('????????????????????????????????????????????????????????????????????????????????????');
                        } else {
                            const Checkbox = CreateCheckbox({ label: firstProperty.Desc });
                            $Elements.$OperatorUI.empty().html(Checkbox);
                        }

                        break;
                }
            }
            //??????????????? ????????????
            $Elements.$RuleOperatorSelect.empty();
            if (firstProperty.ListOperator.length > 0) {
                for (let opera of firstProperty.ListOperator) {
                    const option = CreateOption(<SelectListItem>opera);
                    $Elements.$RuleOperatorSelect.append(option);
                }
                $Elements.$RuleOperatorDropdown.removeClass('disabled');
            } else {
                const option = CreateOption('none');
                $Elements.$RuleOperatorSelect.append(option);
                $Elements.$RuleOperatorDropdown.addClass('disabled');
            }

            $Elements.$RuleOperatorSelect.dropdown('refresh');
            $Elements.$RuleColumnSelect.dropdown('refresh');
            $Elements.$RuleFilterSelect.dropdown('refresh');
        },
    });
};
/**??????:?????????????????? */
const TaskChangeColumn = ($Elements: RuleDropdownGroup) => {
    $Elements.$RuleColumnDropdown.dropdown({
        onChange: function(value, text, $selectedItem) {
            const tableName = $Elements.$RuleTableDropdown.dropdown('get value');
            const propertydata = tempdata.TableList.filter(item => item.TableName == tableName)[0].Properties;
            const columndata = propertydata.filter(property => property.Column == value)[0];
            if (columndata.ListItem.length > 0) {
                const newSelect = CreateSelect(columndata.ListItem, columndata.IsMultiple, 'RuleFilter');
                $Elements.$OperatorUI.empty().append(newSelect);
                $(newSelect).dropdown('refresh');
            } else {
                switch (columndata.Type.toLowerCase()) {
                    case 'string':
                        const StringInput = TextInput({ placeholder: `?????????${columndata.Desc}`, name: 'RuleInput' });
                        $Elements.$OperatorUI.empty().html(StringInput);
                        break;
                    case 'int':
                        const IntInput = NumberInput({ placeholder: `?????????${columndata.Desc}`, name: 'RuleInput' });
                        $Elements.$OperatorUI.empty().html(IntInput);
                        break;
                    default:
                        const Checkbox = CreateCheckbox({
                            label: columndata.Desc,
                        });
                        $Elements.$OperatorUI.empty().html(Checkbox);
                        break;
                }
            }
            //??????????????? ????????????
            $Elements.$RuleOperatorSelect.empty();
            if (columndata.ListOperator.length > 0) {
                for (let opera of columndata.ListOperator) {
                    const option = CreateOption(<SelectListItem>opera);
                    $Elements.$RuleOperatorSelect.append(option);
                }
                $Elements.$RuleOperatorDropdown.removeClass('disabled');
            } else {
                const option = CreateOption('none');
                $Elements.$RuleOperatorSelect.append(option);
                $Elements.$RuleOperatorDropdown.addClass('disabled');
            }
        },
    });
};
/**???????????????????????????????????????????????????????????????????????? */
const getValueByDropdown = (
    $dropdown: JQuery<HTMLElement>
): { selectdValues: Array<string>; selectTexts: Array<string> } => {
    const selectedValue_ = (<string>$dropdown.dropdown('get value')).toString();
    const selectText_: Array<string> = [];
    const selectdValues = IsNullorUndefined(selectedValue_) ? [] : selectedValue_.split(/[;,]+/);

    for (let value of selectdValues) {
        const item = $dropdown.dropdown('get item', value);
        if (!IsNullorUndefined(item)) {
            selectText_.push(item.text());
        }
    }
    return {
        selectdValues: selectdValues,
        selectTexts: selectText_,
    };
};
/*---------------------------??????-------------------------------------*/

/**???????????? */
$RuleDropdown.dropdown('set selected', StringEnum.All);
Search({
    RuleCategory: StringEnum.Empty,
});
/**???????????????????????? */
$(SearchFormId).submit(function(event) {
    event.preventDefault();
    const ruletype = $RuleDropdown.dropdown('get value');
    Search({
        RuleCategory: ruletype == StringEnum.All ? StringEnum.Empty : ruletype, //TODO ???????????????????????????*?????????
    });
});
/**?????????????????? */
$("button[name='reset']").click(function() {
    $RuleDropdown.dropdown('set selected', StringEnum.All);
});

/*---------------------------??????-------------------------------------*/
/**?????????????????????????????? */
var createRules: Array<EditRuleFilterModel> = [];
/**
 * ?????????????????????
 * @param data
 */
const CreateRuleItem = (data: {
    WhereClauseStr: string;
    sort: string;
    IsEnabled: boolean;
    tableStr: string;
    columnStr: string;
    operatorStr: string;
    FilerValue: string;
    IsFullSite: boolean;
}): HTMLDivElement => {
    const enabledStyle: { text: string; color: Color; icon: string } = data.IsEnabled
        ? { text: '??????', color: Color.?????????, icon: 'check' }
        : { text: '?????????', color: Color.???, icon: 'close' };
    const enabledLabel = Label(enabledStyle.text, enabledStyle.color, [
        enabledStyle.color,
        'basic',
        ' transparent',
        'left',
        'floated',
    ]);
    const item = document.createElement('div');
    item.className = 'item';
    item.innerHTML = data.IsFullSite
        ? `<div class="right floated content">  ${enabledLabel}
            <button class="ui red right floated icon button" type="button" name="deleteRule">  <i class="icon delete"></i> </button>
          </div><div class="content">????????????</div>`
        : `<div class="right floated content">
               ${enabledLabel}
                <button class="ui red right floated icon button" type="button" name="deleteRule">  <i class="icon delete"></i> </button>
            </div>
            <div class="content">
              <span class="ui basic label">${data.WhereClauseStr}</span>
              <span class="ui label">???:</span>
              <span class="ui circular black label">${data.sort}</span>
              <span class="ui label">${data.tableStr}</span>
              <span class="ui blue label">${data.columnStr}</span>
              <span class="ui label">${data.operatorStr}</span>
              <span class="ui blue label x-filterlabel" title='${data.FilerValue}'>${data.FilerValue}</span>
            </div>`;
    return item;
};
/**?????????????????????????????????????????? */
const GetSelectedOptonAttr = (
    containSelector: string,
    selectName: string,
    selectValue: any,
    attrName: string
): string => {
    const $select = GetSelect(containSelector, selectName);
    const $option = $select.find('option[value="' + selectValue + '"]');
    return $option.length > 0 ? $option.attr(attrName) : '';
};
/**???????????????????????????????????? */
const $CreateElements = GetDropdownGroups({
    category: RuleEnglishCategory.NotSpecify,
    ruleInputId: RuleInputId,
    formId: CreateFormId,
});
/**????????????????????????????????? */
const IsFullsiteRule = (table: string | '*', column: string | '*'): boolean => {
    return table === StringEnum.All || column === StringEnum.All;
};
/**??????????????????????????? */
TaskChangeCategory($CreateElements);
/**?????????????????????????????? */
TaskChangeTable($CreateElements);
/**??????????????????????????? */
TaskChangeColumn($CreateElements);

/**???????????????????????? */
$(CreateFormId).submit(function(event) {
    event.preventDefault();
    const $form = $(CreateFormId);
    const $table = $CreateElements.$RuleTableDropdown;
    const $column = $CreateElements.$RuleColumnDropdown;
    const sort = $CreateElements.$RuleInput.find("input[name='RuleOrder']");
    const $whereClause = GetDropdown(RuleInputId, 'RuleWhereClause');
    const $filter = GetDropdown(RuleInputId, 'RuleFilter');
    const $operator = $CreateElements.$RuleOperatorDropdown;
    const tableValue = <string>$table.dropdown('get value');
    const columnValue = <string>$column.dropdown('get value');
    const isf = IsFullsiteRule(tableValue, columnValue); //???????????????
    const isFormValid = isf ? true : CheckForm(CreateFormId, valid.CreateRuleItem);
    if (isFormValid) {
        const enabled = <boolean>$(RuleInputId)
            .find(".checkbox[name='RuleEnabled']")
            .checkbox('is checked') ? true : false;
        const newRule = <EditRuleFilterModel>{
            RuleCategory: <string>$CreateElements.$CategoryDropdown.dropdown('get value'),
            TargetTable: isf ? StringEnum.All : tableValue,
            Operator: isf ? StringEnum.All : <string>$operator.dropdown('get value'),
            FilterValue:
                $CreateElements.$OperatorUI.find("input[name='RuleInput']").length > 0
                    ? <string>$CreateElements.$OperatorUI.find("input[name='RuleInput']").val()
                    : $filter.hasClass('mutiple')
                    ? (<Array<string>>$filter.dropdown('get value')).join(';')
                    : <string>$filter.dropdown('get value'),
            FilterField: isf ? StringEnum.All : <string>$column.dropdown('get value'),
            FieldType: GetSelectedOptonAttr(
                RuleInputId,
                'RuleColumn',
                <string>$column.dropdown('get value'),
                'data-type'
            ),
            Priority: Number(<string>sort.val()),
            IsEnabled: enabled,
            Note: <string>$form.find("input[name='Note']").val(),
            WhereClause: $whereClause.dropdown('get value'),
        };
        $CreateRuleTip.hide();
        if (createRules.length < MaxRuleConditionLength) {
            createRules.push(newRule);
            const FilerText = (): string => {
                let showText: string = StringEnum.Empty;
                for (let value of newRule.FilterValue.split(';')) {
                    const item = $filter.dropdown('get item', value);
                    showText = IsNULLorEmpty(showText) ? item.text() : [showText, item.text()].join('???');
                }
                return showText;
            };

            const ruleitem = CreateRuleItem({
                WhereClauseStr: newRule.WhereClause,
                sort: newRule.Priority.toString(),
                IsEnabled: newRule.IsEnabled,
                tableStr: $table.dropdown('get text'),
                columnStr: $column.dropdown('get text'),
                operatorStr: $operator.dropdown('get text'),
                FilerValue:
                    $CreateElements.$OperatorUI.find("input[name='RuleInput']").length > 0
                        ? <string>$CreateElements.$OperatorUI.find("input[name='RuleInput']").val()
                        : IsFullsiteRule(tableValue, columnValue)
                        ? ''
                        : FilerText(),
                IsFullSite: isf,
            });
            $CreateRuleArea.append(ruleitem);
        } else {
            WarningMessage('????????????????????????');
        }
    }
});
/**??????????????????--??????????????? */
$CreateRuleArea.on('click', "button[name='deleteRule']", function() {
    const item = $(this).closest('.item');
    const itemIndex = item.index();
    item.remove();
    createRules = createRules.filter((rule, index) => index !== itemIndex);
    if (createRules.length == 0) {
        $CreateRuleTip.show();
    }
});
/** ??????????????????*/
ModalTask(CreateModalId, false, {
    closable: false,
    onShow: function() {
        TaskInitDropdown($CreateElements);
    },
    onApprove: function() {
        const $form = $(CreateFormId);
        const isFormValid = CheckForm(CreateFormId, valid.CreateRule);
        if (isFormValid) {
            //Notice:??????????????????????????????,???????????????????????????(??????client???????????????????????????????????????????????????,????????????????????????)
            for (let rule of createRules) {
                rule.Note = <string>$form.find("input[name='Note']").val();
                rule.RuleCategory = <string>$CreateElements.$CategoryDropdown.dropdown('get value');
            }
            route
                .CreateMainRule({
                    RuleMaster: {
                        RuleName: <string>$form.find("input[name='RuleName']").val(),
                        RuleCategory: <string>$CreateElements.$CategoryDropdown.dropdown('get value'),
                        IsEnabled: true, //???????????????????????????
                        Note: <string>$form.find("input[name='Note']").val(),
                    },
                    Filters: createRules,
                })
                .then(res => {
                    const record = <CreateRuleModel>res.Records;
                    const data =<Array<RuleSearchListModel>>res.Data;
                    if (res.IsSuccess) {
                        SuccessMessage(res.Message);
                        for(const datum of data){
                            if (datum.RuleFilters.length == 0) {
                                table.AddRow<RuleFrontEndListModel>(
                                    Object.assign(<RuleFrontEndListModel>{}, {
                                        RuleCategory: record.RuleMaster.RuleCategory,
                                        IsNullModel: true,
                                    })
                                ); //TODO ???????????????????????????????????????model
                            } else {
                                const fullsiteRules = datum.RuleFilters.filter(function(rule) {
                                    return rule.RuleColumn === StringEnum.All && rule.RuleTable === StringEnum.All;
                                });
    
                                /**??????????????????????????? */
                                const IsFullsiteVerify: boolean =
                                    fullsiteRules.length > 0 ? (<RuleFrontEndListModel>fullsiteRules[0]).IsEnabled : false;
                                for (let subRule of datum.RuleFilters) {
                                    /**??????????????????????????? */
                                    const IsFullsiteRule =
                                        subRule.RuleColumn === StringEnum.All && subRule.RuleTable === StringEnum.All;
                                    table.AddRow<RuleFrontEndListModel>(<RuleFrontEndListModel>{
                                        RuleCategory: subRule.RuleCategory,
                                        RuleColumn: subRule.RuleColumn,
                                        RuleTable: subRule.RuleTable,
                                        ColumnName: subRule.ColumnName,
                                        Operator: subRule.Operator,
                                        OperatorStr: subRule.OperatorStr,
                                        FilterValue: subRule.FilterValue,
                                        Priority: subRule.Priority,
                                        IsEnabled: subRule.IsEnabled,
                                        WhereClause: subRule.WhereClause,
                                        RuleName: record.RuleMaster.RuleName,
                                        RuleEnabled: subRule.IsEnabled,
                                        TableName:datum.TableName,
                                        IsNullModel: false,
                                        RuleIndex: subRule.FilterKey,
                                        EnabledStyle:
                                            IsFullsiteRule && IsFullsiteVerify
                                                ? RuleEnabledStyle.First
                                                : IsFullsiteRule && !IsFullsiteVerify
                                                ? RuleEnabledStyle.Disable
                                                : !IsFullsiteRule && IsFullsiteVerify
                                                ? RuleEnabledStyle.Noeffect
                                                : RuleEnabledStyle.Enable,
                                    });
                                }
                            }
                        }
                     

                        $form.trigger('reset');
                        createRules = [];
                        $(CreateModalId).modal('hide');
                    } else {
                        ErrorMessage(res.Message);
                    }
                })
                .catch(error => {
                    Logger.viewres(route.api.CreateRule, '`????????????', error, true);
                });
        }
        return false;
    },
    onDeny: function() {},
});

/*---------------------------??????-------------------------------------*/
/**
 * ????????????
 */
$(document).on('keyup', '#wordFilter', function() {
    const word = <string>$(this).val();
    const filter = [
        { field: 'RuleName', type: Filter.Like, value: word },
        { field: 'RuleTable', type: Filter.Like, value: word },
        { field: 'TableName', type: Filter.Like, value: word },
    ];
    table.SetFilter(filter);
});

/*????????????*/
function Search(SearchParams: RuleSearchModel) {
    table = new tabulatorService(initSetting.TableId, {
        height: TabulatorSetting.height,
        layout: TabulatorSetting.layout,
        ajaxURL: route.api.Search,
        ajaxContentType: 'json',
        ajaxConfig: 'POST',
        ajaxParams: SearchParams,
        index: prop('RuleIndex'),
        //  headerVisible: false,
        ajaxResponse: function(url: string, param: RuleSearchModel, res: IResponse) {
            const originData = <Array<RuleSearchListModel>>res.Data;
            let newData: Array<RuleFrontEndListModel> = [];
            for (let mainRule of originData) {
                const main: RuleTableInheritModel = {
                    RuleName: mainRule.RuleName,
                    RuleEnabled: mainRule.RuleEnabled,
                    RuleTable: mainRule.RuleTable,
                    TableName: mainRule.TableName,
                    RuleCategory: mainRule.RuleCategory,
                };
                /**Notice:?????????????????????????????????????????????????????????,???????????????????????????????????????,??????????????????model */
                if (mainRule.RuleFilters.length == 0) {
                    newData.push(
                        Object.assign(
                            <RuleFrontEndListModel>{
                                IsNullModel: true,
                            },
                            main
                        )
                    );
                } else {
                    for (let subRule of mainRule.RuleFilters) {
                        const fullsiteRules = mainRule.RuleFilters.filter(function(rule) {
                            return rule.RuleColumn === StringEnum.All && rule.RuleTable === StringEnum.All;
                        });

                        /**??????????????????????????? */
                        const IsFullsiteRule =
                            subRule.RuleColumn === StringEnum.All && subRule.RuleTable === StringEnum.All;
                        /**??????????????????????????? */
                        const IsFullsiteVerify: boolean =
                            fullsiteRules.length > 0 ? (<RuleFrontEndListModel>fullsiteRules[0]).IsEnabled : false;
                        const newlist: RuleFrontEndListModel = <RuleFrontEndListModel>{
                            RuleCategory: subRule.RuleCategory,
                            RuleColumn: subRule.RuleColumn,
                            RuleTable: subRule.RuleTable,
                            ColumnName: subRule.ColumnName,
                            Operator: subRule.Operator,
                            OperatorStr: subRule.OperatorStr,
                            FilterValue: subRule.FilterValue,
                            Priority: subRule.Priority,
                            IsEnabled: subRule.IsEnabled,
                            WhereClause: subRule.WhereClause,
                            RuleName: main.RuleName,
                            RuleEnabled: main.RuleEnabled,
                            TableName: main.TableName,
                            IsNullModel: false,
                            RuleIndex: subRule.FilterKey,
                            EnabledStyle:
                                IsFullsiteRule && IsFullsiteVerify ? RuleEnabledStyle.First             //????????????-????????????
                                    : IsFullsiteRule && !IsFullsiteVerify ? RuleEnabledStyle.Disable    //????????????-??????
                                        : !IsFullsiteRule && IsFullsiteVerify ? RuleEnabledStyle.Noeffect //????????????-??????
                                            //: RuleEnabledStyle.Enable,
                                            : subRule.IsEnabled ? RuleEnabledStyle.Enable : RuleEnabledStyle.Disable,
                        };
                        newData.push(newlist);
                    }
                }
            }
            return newData;
        },
        groupBy: prop('RuleCategory'),
        groupStartOpen: true,
        groupToggleElement: 'arrow' /*???header?????????group??????????????????*/,
        groupHeader: [
            function(value, count, data) {
                const DATA = <Array<RuleFrontEndListModel>>data;
                const rulename = DATA[0].RuleName;
                const isenable = DATA[0].RuleEnabled ? 'checked="checked"' : '';
                const ischecked = DATA[0].RuleEnabled ? 'checked' : '';
                const nullModelLength = DATA.filter(item => item.IsNullModel === true).length;
                const showcount = count - nullModelLength; /*Notice:???????????????model????????????????????????????????? */
                const switchbtn = `<div class="Switching" style="float:right;"> 
                                   <div class="slideThree" name="categoryActive" data-category="${value}" 
                                    data-originEnabled="${DATA[0].RuleEnabled}">
                                     <input type="checkbox" value="${value}" name="categoryInput" 
                                      ${ischecked ? `checked="checked"` : ''}> <label></label>  </div> </div>`;
                const templete: string = `<span name="ruleName">${rulename}</span>(????????????: ${value})<span class="ui large label"> <i class="cube ${Color.???} icon"></i>??????${showcount}?????????</span>${switchbtn}
                <button class="ui icon basic green mini  right floated button" name="editMain" data-category="${value}"???data-inverted=""  data-tooltip="????????????" data-position="bottom center"><i class="edit icon"></i></button>
                <button class="ui icon basic yellow mini right floated button" name="createSub" data-category="${value}" data-inverted=""  data-tooltip="???????????????" data-position="bottom center"><i class="plus icon"></i></button> `;
                return templete;
            },
        ],
        groupClick: function(e, group: Tabulator.GroupComponent) {
            const target: HTMLButtonElement | HTMLDivElement | HTMLElement = <any>e.target;
            const $groupElement = group.getElement();

            /*Notice:???????????????"data=init",????????????????????????????????????????????????????????????checkbox???checked(??????onChange???????????????????????????)*/
            if (!$groupElement.hasAttribute('data-init')) {
                $($groupElement)
                    .find('.checkbox')
                    .checkbox('set checked');
                $groupElement.setAttribute('data-init', 'true');
            }
            /*Notice:????????????????????????,?????????????????????????????????????????????????????????????????????*/
            const groupData = <RuleFrontEndListModel>group.getRows()[0].getData();
            const targetCategory = <RuleEnglishCategory>groupData.RuleCategory;
            switch (true) {
                /*??????:?????????????????*/
                case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('plus icon') > -1:
                case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'createSub':
                    const SubCreateModalId = '#CreateSubModal';
                    const SubCreateFormId = '#CreateSubForm';
                    const $SubForm = $(SubCreateFormId);
                    ModalTask(SubCreateModalId, true, {
                        closable: false,
                        onShow: function() {
                            $SubForm.find('.dropdown').dropdown();
                            const $SubElements = GetDropdownGroups({
                                category: targetCategory,
                                ruleInputId: '#SubCreateInput',
                                formId: SubCreateFormId,
                            });
                            TaskInitDropdown($SubElements, targetCategory);
                            TaskChangeCategory($SubElements);
                            TaskChangeTable($SubElements);
                            TaskChangeColumn($SubElements);
                        },
                        onApprove: function() {
                            const $SubElements = GetDropdownGroups({
                                category: targetCategory,
                                ruleInputId: '#SubCreateInput',
                                formId: SubCreateFormId,
                            });
                            const $filter = $SubElements.$OperatorUI
                                .find('select[name="RuleFilter"]')
                                .parent('.dropdown');
                            const is$filterMutiple = $filter.hasClass('mutiple');
                            const $ruleInput = $SubElements.$OperatorUI.find('input[name="RuleInput"]');

                            const FilerText = (): { FilterValue: string; FilterText: string } => {
                                const filterResult = getValueByDropdown($filter);
                                return {
                                    FilterValue:
                                        $filter.length == 0
                                            ? <string>$ruleInput.val()
                                            : filterResult.selectdValues.join(';'),
                                    FilterText: filterResult.selectTexts.join('???'),
                                };
                            };
                            const input = <EditRuleFilterModel>{
                                RuleCategory: targetCategory,

                                TargetTable: GetDropdown(SubCreateFormId, 'RuleTable').dropdown('get value'),
                                /**???????????? */
                                FilterField: GetDropdown(SubCreateFormId, 'RuleColumn').dropdown('get value'),
                                Operator: GetDropdown(SubCreateFormId, 'RuleOperator').dropdown('get value'),
                                FilterValue: FilerText().FilterValue,
                                FieldType: GetSelect(SubCreateFormId, 'RuleColumn')
                                    .find(
                                        `option[value='${GetDropdown(SubCreateFormId, 'RuleColumn').dropdown(
                                            'get value'
                                        )}']`
                                    )
                                    .attr('data-type'),
                                Priority: Number($SubForm.find('input[name="RuleOrder"]').val()),
                                IsEnabled: $SubForm.find('.checkbox[name="RuleEnabled"]').checkbox('is checked'),
                                Note: '',
                                WhereClause: GetDropdown(SubCreateFormId, 'RuleWhereClause').dropdown('get value'),
                            };
                            /*Notice:?????????????????????????????????*/

                            const isFormValid =
                                input.TargetTable === StringEnum.All
                                    ? true
                                    : CheckForm(SubCreateFormId, valid.CreateRuleItem);
                            if (isFormValid) {
                                route
                                    .AddRule(input)
                                    .then(res => {
                                        const record = <EditRuleFilterModel>res.Records;
                                        const data = <RuleSearchListModel>res.Data;
                                        const columnName = $SubElements.$RuleColumnDropdown.dropdown(
                                            'get item',
                                            record.FilterField
                                        );
                                        const columnNameStr = IsNullorUndefined(columnName)
                                            ? StringEnum.Empty
                                            : columnName.text();
                                        if (res.IsSuccess) {
                                            SuccessMessage(res.Message);
                                            table.AddRow(<RuleFrontEndListModel>{
                                                IsNullModel: false,
                                                RuleIndex: `${record.RuleCategory}_${record.TargetTable}_${record.FilterField}`,
                                                FilterKey: `${record.RuleCategory}_${record.TargetTable}_${record.FilterField}`,
                                                EnabledStyle:
                                                    record.TargetTable === StringEnum.All && record.FilterField
                                                        ? record.IsEnabled
                                                            ? RuleEnabledStyle.First
                                                            : RuleEnabledStyle.Noeffect
                                                        : record.IsEnabled
                                                        ? RuleEnabledStyle.Enable
                                                        : RuleEnabledStyle.Disable,
                                                RuleTable: data.RuleTable,
                                                RuleColumn: record.FilterField,
                                                ColumnName: columnNameStr,
                                                Operator: record.Operator,
                                                OperatorStr: getEnumKeyByEnumValue(
                                                    ChineseRuleOperator,
                                                    record.Operator.toLowerCase()
                                                ),
                                                FilterValue:
                                                    $filter.length > 0
                                                        ? FilerText().FilterText
                                                        : FilerText().FilterValue,
                                                Priority: record.Priority,
                                                IsEnabled: record.IsEnabled,
                                                WhereClause: record.WhereClause,

                                                RuleCategory: data.RuleCategory,

                                                RuleEnabled: data.RuleEnabled,
                                                RuleName: data.RuleName,
                                                TableName: data.TableName,
                                            });
                                            $(SubCreateModalId).modal('hide');
                                        } else {
                                            ErrorMessage(res.Message);
                                        }
                                    })
                                    .catch(error => {
                                        Logger.viewres(route.api.EditCategory, '??????????????????', error, true);
                                    });
                            }
                            return false;
                        },
                    });
                    break;
                /*??????:??????????????*/
                case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('edit icon') > -1:
                case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'editMain':
                    const CaEditModalId = '#EditCategoryModal';
                    const CaEditFormId = '#EditCategoryForm';
                    ShowModal<{ category: string }>(CaEditModalId, route.api.ShowCategoryEdit, {
                        category: target.closest("button[name='editMain']").getAttribute('data-category'),
                    })
                        .then(IsSuccess => {
                            if (IsSuccess) {
                                const $CaForm = $(CaEditFormId);
                                ModalTask(CaEditModalId, true, {
                                    closable: false,
                                    onApprove: function() {
                                        const ruleCategory = <string>$CaForm.find("input[name='RuleCategory']").val();
                                        const ruleName = <string>$CaForm.find("input[name='RuleName']").val();
                                        const note = <string>$CaForm.find("input[name='Note']").val();
                                        route
                                            .EditCategoryRule({
                                                RuleCategory: ruleCategory,
                                                RuleName: ruleName,
                                                IsEnabled: $CaForm
                                                    .find(".checkbox[name='RuleEnabled']")
                                                    .checkbox('is checked'),
                                                Note: note,
                                            })
                                            .then(res => {
                                                if (res.IsSuccess) {
                                                    SuccessMessage(res.Message);
                                                    $groupElement.querySelector(
                                                        "span[name='ruleName']"
                                                    ).innerHTML = ruleName;
                                                } else {
                                                    ErrorMessage(res.Message);
                                                }
                                            })
                                            .catch(error => {
                                                Logger.viewres(route.api.EditCategory, '???????????????', error, true);
                                            });
                                    },
                                });
                            } else {
                                ErrorMessage('??????????????????????????????');
                            }
                        })
                        .catch(error => {
                            Logger.viewres(route.api.ShowCategoryEdit, '????????????????????????', error, true);
                        });

                    break;
            }
        },
        rowFormatter: function(row: Tabulator.RowComponent) {
            const rowdata = <RuleFrontEndListModel>row.getData();
            const rowElement = row.getElement();
            if (rowdata.IsNullModel) {
                rowElement.innerHTML = `<div style="width:100%;text-align:center;padding:10px;">??????????????????</div>`;
            } else {
                const nullRow = row
                    .getGroup()
                    .getRows()
                    .filter(r => (<RuleFrontEndListModel>r.getData()).IsNullModel === true);
                if (nullRow.length > 0) {
                    nullRow.forEach(r => table.RemoveRow(r));
                }
            }
            return row;
        },
        columns: [
            {
                title: '??????',
                field: prop('EnabledStyle'),
                hozAlign: 'center',
                vertAlign: 'middle',
                sorter: 'string',
                width: 105,
                formatter: function(cell, formatterParams) {
                    const row = cell.getRow();
                    const rowdata = <RuleFrontEndListModel>row.getData();
                    let cellStyle: {
                        Str: string;
                        Color: Color;
                        Icon: string;
                    };
                    switch (rowdata.EnabledStyle) {
                        case RuleEnabledStyle.Noeffect:
                            cellStyle = {
                                Str: '??????',
                                Color: Color.???,
                                Icon: 'lock',
                            };
                            break;
                        case RuleEnabledStyle.Enable:
                            cellStyle = {
                                Str: '??????',
                                Color: Color.???,
                                Icon: 'check',
                            };
                            break;
                        case RuleEnabledStyle.First:
                            cellStyle = {
                                Str: '??????',
                                Color: Color.???,
                                Icon: 'check',
                            };
                            break;
                        case RuleEnabledStyle.Disable:
                            cellStyle = {
                                Str: '??????',
                                Color: Color.???,
                                Icon: 'lock',
                            };
                            break;
                        default:
                            cellStyle = {
                                Str: '?????????',
                                Color: Color.???,
                                Icon: 'close',
                            };
                            break;
                    }
                    return ` <div class="content ui icon basic ${cellStyle.Color} button">
                              <i class="${cellStyle.Icon} ${cellStyle.Color} icon"></i> ${cellStyle.Str}
                            </div>`;
                },
            },
            {
                title: '?????????',
                field: prop('Priority'),
                sorter: 'string',
                vertAlign: 'middle',
                width: 105,
                formatter: function(cell, formatterParams) {
                    const row = cell.getRow();
                    const rowdata = <RuleFrontEndListModel>row.getData();
                    /**??????????????????????????? */
                    const IsFullsiteRule =
                        rowdata.RuleColumn === StringEnum.All && rowdata.RuleTable === StringEnum.All;
                    const priorityStr = IsFullsiteRule ? `??????` : `???  ${rowdata.Priority}`;
                    return priorityStr;
                    // const branchicon = `<div class="tabulator-data-tree-branch"></div>`;
                    // return branchicon;
                },
            },

            {
                title: '??????',
                field: prop('OperatorStr'),
                sorter: 'string',
                vertAlign: 'middle',
                formatter: function(cell, formatterParams) {
                    const row = cell.getRow();
                    const rowdata = <RuleFrontEndListModel>row.getData();
                    const fullsiteRules = row
                        .getGroup()
                        .getRows()
                        .filter(function(rowCompoment) {
                            const compomentData = <RuleFrontEndListModel>rowCompoment.getData();
                            return (
                                compomentData.RuleColumn === StringEnum.All &&
                                compomentData.RuleTable === StringEnum.All
                            );
                        });

                    /**??????????????????????????? */
                    const IsFullsiteRule =
                        rowdata.RuleColumn === StringEnum.All && rowdata.RuleTable === StringEnum.All;
                    /**??????????????????????????? */
                    const IsFullsiteVerify: boolean =
                        fullsiteRules.length > 0
                            ? (<RuleFrontEndListModel>fullsiteRules[0].getData()).IsEnabled
                            : false;

                    const headerStr: string = IsFullsiteRule
                        ? IsFullsiteVerify
                            ? `??????????????????<span style='color:yellow;text-decoration: underline;'>"????????????"</span>??????????????????<span style='color:red;'>????????????</span>`
                            : '?????????????????????'
                        : `???${rowdata.ColumnName}???${rowdata.OperatorStr}???${rowdata.FilterValue}???`;

                    return headerStr;
                },
            },

            {
                title: '??????',
                field: prop('RuleCategory'),
                hozAlign: 'right',
                vertAlign: 'middle',
                width: 185,
                formatter: function(cell, formatterParams) {
                    const row = cell.getRow();
                    const rowdata = <RuleFrontEndListModel>row.getData();
                    const checkedStyle = rowdata.IsEnabled ? `checked="checked"` : '';
                    const switchbtn = `<div class="Switching"> <div class="slideThree">
                                      <input type="checkbox" value="${rowdata.RuleCategory}" name="switchFullsiteRule"   ${checkedStyle}>
                                      <label></label> </div> </div>`;
                    /**??????????????????????????? */
                    const IsFullsiteRule =
                        rowdata.RuleColumn === StringEnum.All && rowdata.RuleTable === StringEnum.All;
                    if (!IsFullsiteRule) {
                        cell.getElement().classList.add('tabulator-operation');
                        const id: number = parseInt(cell.getValue());
                        const editbtn = EditButton(id, message.Controller);
                        const deletebtn = DeleteButton(id, message.Controller);
                        const btngroups: string = editbtn + deletebtn + switchbtn;
                        return btngroups;
                    }
                    return switchbtn;
                },
                cellClick: function(e, cell) {
                    const target: HTMLButtonElement | HTMLDivElement | HTMLElement = <any>e.target;
                    const row = cell.getRow();
                    const rowdata = <RuleFrontEndListModel>row.getData();
                    const fullsiteRules = row
                        .getGroup()
                        .getRows()
                        .filter(function(r) {
                            const rData = r.getData();
                            return rData.RuleColumn === StringEnum.All && rData.RuleTable === StringEnum.All;
                        });

                    /**??????????????????????????? */
                    const IsFullsiteVerify: boolean =
                        fullsiteRules.length > 0
                            ? (<RuleFrontEndListModel>fullsiteRules[0].getData()).IsEnabled
                            : false;
                    /**??????????????????????????? */
                    const IsFullsiteRule =
                        rowdata.RuleColumn === StringEnum.All && rowdata.RuleTable === StringEnum.All;
                    switch (true) {
                        /*??????:??????*/
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('edit icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'edit':
                            ShowModal<GetRuleModel>(EditModalId, route.api.ShowEdit, {
                                category: rowdata.RuleCategory,
                                table: rowdata.RuleTable,
                                field: rowdata.RuleColumn,
                            })
                                .then(IsSuccess => {
                                    if (IsSuccess) {
                                        const $EditForm = $(EditFormId);
                                        ModalTask(EditModalId, true, {
                                            closable: false,
                                            onShow: function() {
                                                $EditForm.find('.dropdown').dropdown();
                                            },
                                            onApprove: function() {
                                                const $FiltetUI = $EditForm.find("div[name='FilterUI']");
                                                const $Operator = GetDropdown(EditFormId, 'Operator');
                                                const $RuleInput = $FiltetUI.find("input[name='RuleInput']");
                                                const $FilterValueAry = GetDropdown(EditFormId, 'FilterValueAry');
                                                const $FieldType = $EditForm.find("input[name='FieldType']");
                                                const $Priority = $EditForm.find("input[name='Priority']");
                                                const $RuleEnabled = $EditForm.find(".checkbox[name='RuleEnabled']");
                                                const $RuleNote = $EditForm.find("input[name='RuleNote']");
                                                const $RuleWhereClause = GetDropdown(EditFormId, 'RuleWhereClause');
                                                const $ColumnName = $EditForm.find('input[name="ColumnName"]');
                                                const filterResult = getValueByDropdown($FilterValueAry);
                                                route
                                                    .EditRule({
                                                        RuleCategory: rowdata.RuleCategory,
                                                        TargetTable: rowdata.RuleTable,
                                                        FilterField: rowdata.RuleColumn,
                                                        Operator: $Operator.dropdown('get value'),
                                                        FilterValue:
                                                            $RuleInput.length > 0
                                                                ? <string>$RuleInput.val()
                                                                : filterResult.selectdValues.join(';'),
                                                        FieldType: <string>$FieldType.val(),
                                                        Priority: Number($Priority.val()) || 1,
                                                        IsEnabled: $RuleEnabled.checkbox('is checked') ? true : false,
                                                        Note: <string>$RuleNote.val(),
                                                        WhereClause: $RuleWhereClause.dropdown('get value'),
                                                    })
                                                    .then(res => {
                                                        SuccessMessage(res.Message);
                                                        const record = <EditRuleFilterModel>res.Records;
                                                        const data = <RuleSearchListModel>res.Data;
                                                        if (res.IsSuccess) {
                                                            const updateData: Pick<
                                                                RuleFrontEndListModel,
                                                                | 'Priority'
                                                                | 'IsEnabled'
                                                                | 'ColumnName'
                                                                | 'Operator'
                                                                | 'OperatorStr'
                                                                | 'FilterValue'
                                                            > = {
                                                                Priority: record.Priority,
                                                                IsEnabled: record.IsEnabled,
                                                                ColumnName: rowdata.ColumnName,
                                                                Operator: record.Operator.toLowerCase(),
                                                                OperatorStr: getEnumKeyByEnumValue(
                                                                    ChineseRuleOperator,
                                                                    record.Operator.toLowerCase()
                                                                ),
                                                                FilterValue:
                                                                    $RuleInput.length > 0
                                                                        ? <string>$RuleInput.val()
                                                                        : filterResult.selectTexts.join('???'),
                                                            };

                                                            row.update(updateData);
                                                        } else {
                                                            ErrorMessage(res.Message);
                                                        }
                                                    })
                                                    .catch(error => {
                                                        Logger.viewres(route.api.Edit, '???????????????', error, true);
                                                    });
                                            },
                                        });
                                    } else {
                                        ErrorMessage('??????????????????????????????');
                                    }
                                })
                                .catch(error => {
                                    Logger.viewres(route.api.ShowEdit, '`??????????????????', error, true);
                                });

                            break;
                        /**??????:?????? */
                        case target instanceof HTMLElement &&
                            (<HTMLElement>target).className.indexOf('delete icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'delete':
                            ShowModal<GetRuleModel>(DeleteModalId, route.api.ShowDelete, {
                                category: rowdata.RuleCategory,
                                table: rowdata.RuleTable,
                                field: rowdata.RuleColumn,
                            })
                                .then(IsSuccess => {
                                    if (IsSuccess) {
                                        const $form = $(DeleteFormId);
                                        ModalTask(DeleteModalId, true, {
                                            closable: false,
                                            onShow: function() {
                                                $form.find('.dropdown').dropdown();
                                            },
                                            onApprove: function() {
                                                route
                                                    .DeleteRule({
                                                        category: rowdata.RuleCategory,
                                                        table: rowdata.RuleTable,
                                                        field: rowdata.RuleColumn,
                                                    })
                                                    .then(res => {
                                                        if (res.IsSuccess) {
                                                            SuccessMessage(res.Message);
                                                            table.RemoveRow(row);
                                                            if (row.getGroup().getRows().length == 0) {
                                                                table.AddRow(<RuleFrontEndListModel>{
                                                                    IsNullModel: true,
                                                                    RuleName: rowdata.RuleName,
                                                                    RuleEnabled: rowdata.RuleEnabled,
                                                                    RuleTable: rowdata.RuleTable,
                                                                    TableName: rowdata.TableName,
                                                                    RuleCategory: rowdata.RuleCategory,
                                                                });
                                                            }
                                                        } else {
                                                            ErrorMessage(res.Message);
                                                        }
                                                    })
                                                    .catch(error => {
                                                        Logger.viewres(route.api.Delete, '`???????????????', error, true);
                                                    });
                                            },
                                        });
                                    } else {
                                        ErrorMessage('??????????????????????????????');
                                    }
                                })
                                .catch(error => {
                                    Logger.viewres(route.api.ShowDelete, '`??????????????????', error, true);
                                });
                            break;
                        /*??????:????????????????????????*/
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('Switching') > -1:
                        case target instanceof HTMLElement &&
                            (<HTMLElement>target).className.indexOf('slideThree') > -1:
                        case target instanceof HTMLInputElement &&
                            (<HTMLInputElement>target).name == 'switchFullsiteRule':
                        case target instanceof HTMLLabelElement:
                            const $checkbox = $(cell.getElement().querySelector("input[name='switchFullsiteRule']"));
                            /*?????????????????????????????????*/
                            const switchstatus: boolean = $checkbox.prop('checked') ? false : true;
                            const switchFullsiteRuleModalId = '#SwitchFullsiteRuleModal';
                            ModalTask(switchFullsiteRuleModalId, true, {
                                closable: false,
                                onShow: function() {
                                    $(switchFullsiteRuleModalId)
                                        .find('.content')
                                        .html(`?????????${switchstatus ? '??????' : '??????'}?????????`);
                                },
                                onApprove: function() {
                                    route
                                        .ActiveSubRule({
                                            RuleCategory: rowdata.RuleCategory,
                                            Table: rowdata.RuleTable,
                                            Column: rowdata.RuleColumn,
                                            IsActive: switchstatus,
                                        })
                                        .then(res => {
                                            if (res.IsSuccess) {
                                                $checkbox.prop('checked', !$checkbox.prop('checked'));
                                                SuccessMessage(res.Message);
                                                const updateData: Pick<
                                                    RuleFrontEndListModel,
                                                    'IsEnabled' | 'EnabledStyle'
                                                > = {
                                                    IsEnabled: switchstatus,
                                                    EnabledStyle: IsFullsiteRule
                                                        ? switchstatus
                                                            ? RuleEnabledStyle.First
                                                            : RuleEnabledStyle.Noeffect
                                                        : IsFullsiteVerify
                                                        ? RuleEnabledStyle.Noeffect
                                                        : switchstatus
                                                        ? RuleEnabledStyle.Enable
                                                        : RuleEnabledStyle.Disable,
                                                };
                                                row.update(updateData); //TODO ?????????????????????,????????????????????????
                                                if (IsFullsiteRule) {
                                                    row.getGroup()
                                                        .getRows()
                                                        .filter(r => r !== row)
                                                        .forEach(r => {
                                                            const rData = <RuleFrontEndListModel>r.getData();
                                                            const rupdateData: Pick<
                                                                RuleFrontEndListModel,
                                                                'IsEnabled' | 'EnabledStyle'
                                                            > = {
                                                                IsEnabled: rData.IsEnabled,
                                                                EnabledStyle: switchstatus
                                                                    ? RuleEnabledStyle.Noeffect
                                                                    : rData.IsEnabled
                                                                    ? RuleEnabledStyle.Enable
                                                                    : RuleEnabledStyle.Disable,
                                                            };
                                                            /*Notice:????????????????????????EnableStyle??????IsEnabled DATA,????????????????????????,IsEnabled????????????????????????????????????????????????*/
                                                            r.update(rupdateData);
                                                        });
                                                }
                                            } else {
                                                ErrorMessage(res.Message);
                                            }
                                        });
                                },
                            });
                            break;
                    }
                },
            },
        ],
    });
}

/**????????????????????? */
$(document).on('click', 'div[name="categoryActive"]', function() {
    const $checkbox = $(this);
    const $chcekinpput = $(this).children('input[type="checkbox"]');
    const isChecked = $checkbox.attr('data-originEnabled') === 'true' ? true : false;
    const goChecked = !isChecked;
    const targetCategory = $checkbox.attr('data-category');
    const $setGroupToggle = (isChecked: boolean) => {
        if (isChecked) {
            $checkbox.checkbox('set checked');
            $chcekinpput.attr('checked', 'checked');
            $checkbox.attr('data-originEnabled', 'true');
        } else {
            $checkbox.checkbox('set unchecked');
            $chcekinpput.removeAttr('checked');
            $checkbox.attr('data-originEnabled', 'false');
        }
    };
    ModalTask('#CategoryConfirm', true, {
        closable: false,
        onShow: function() {
            $('#CategoryConfirm')
                .find('.content')
                .text(`?????????${goChecked ? '??????' : '??????'}?????????????????????????`);
        },
        onApprove: function() {
            route
                .ActiveRule(targetCategory, goChecked)
                .then(res => {
                    if (res.IsSuccess) {
                        SuccessMessage(res.Message);
                        $setGroupToggle(goChecked);
                    } else {
                        /*Notice:????????????????????????????????????*/
                        ErrorMessage(res.Message);
                        $setGroupToggle(isChecked);
                    }
                })
                .catch(error => {
                    Logger.viewres(route.api.ActiveCategory, '????????????', error, true);
                    $setGroupToggle(isChecked);
                });
        },
        onDeny: function() {
            /*Notice:????????????????????????????????????*/
            $setGroupToggle(isChecked);
        },
    });
});
