/**
 * 讓input[type="password"]切換可見
 * @param inputId 
 */
export function PasswordVisible(inputId:string){
    inputId=inputId.replace("#","");
    const input= <HTMLInputElement>document.getElementById(inputId);
    if (input.type === "password") {
      input.type = "text";
    } else {
      input.type = "password";
    }
}