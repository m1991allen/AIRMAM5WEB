import { PasswordVisible } from "../../Models/Function/Password";

/**讓密碼是否可見 */
$("i.eye").click(function(){
    const input=$(this).siblings("input");
    const id=input.attr("id");
    if($(this).hasClass("slash")){
       $(this).removeClass("slash");
    }else{
      $(this).addClass("slash");
    }
    PasswordVisible(id);
});
