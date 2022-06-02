import { format } from "path";

(function($){
    var originalData=null;
    var editData=null;
    $.fn.extend({
        trackEdit: function() {
          const form=$(this);
          form.data("changed", true);
          originalData=form.serialize();
          $(":input,textarea,select",form).change(function() {
             editData=form.serialize();
             form.isEdited();
          });
          return form;
        },
        isEdited: function() { 
            if(originalData !== editData){
                $(this).trigger('edit');
                this.data("changed",true);
            }else{
                this.data("changed",false);
            }
           return $(this);
          
        },
        isCancel:function(){
           originalData = null,editData = null;
          $(this).data("changed", false);
          $(this).trigger('cancel');
          return $(this);
        },
        onEdit:function(callback){
             $(this).on('edit',function(event){
                  if(callback !==null && typeof callback !=='undefined' && callback !==undefined){
                      callback();
                  }
             });
             return $(this);
        },
        onCancel:function(callback){
            $(this).on('cancel',function(event){
                if(callback !==null && typeof callback !=='undefined' && callback !==undefined){
                    callback();
                }
           });
           return $(this);
        }
    });
    
    
})(jQuery);