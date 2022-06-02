/**
 * 產生隨意顏色
 * @return 16進位色碼，例如：#ff0012
 */
export function getRandomColor(): string {
    var letters = '0123456789ABCDEF';
    var color = '#';
    for (var i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}
/**
 * 返回用線性漸變填充矩形的畫布
 */
export function LinearGradientCanvas(): CanvasGradient {
    /*
     CanvasGradient ctx .createLinearGradient（x0，y0，x1，y1）;
     x0=>起點的x軸坐標,y0=>起點的y軸坐標,x1=>終點的x軸坐標,y1=>終點的y軸坐標
    */
    const linGrad = document
        .createElement('canvas')
        .getContext('2d')
        .createLinearGradient(0, 0, 1000, 128);
    linGrad.addColorStop(0, getRandomColor());
    linGrad.addColorStop(1, getRandomColor());
    return linGrad;
}

/**
 * 十六進制顏色代碼轉RGBA
 * @param hex 十六進制顏色代碼
 * @param alpha 透明度
 */
export function HexToRGBA(hex, alpha) {
    hex = hex.replace('#', '');
    var r = parseInt(hex.length == 3 ? hex.slice(0, 1).repeat(2) : hex.slice(0, 2), 16);
    var g = parseInt(hex.length == 3 ? hex.slice(1, 2).repeat(2) : hex.slice(2, 4), 16);
    var b = parseInt(hex.length == 3 ? hex.slice(2, 3).repeat(2) : hex.slice(4, 6), 16);
    if (alpha) {
        return 'rgba(' + r + ', ' + g + ', ' + b + ', ' + alpha + ')';
    } else {
        return 'rgb(' + r + ', ' + g + ', ' + b + ')';
    }
}

/**取得線性顏色 */
export function getGradient(ctx, chartArea,colors:Array<string>) {
    const chartWidth = chartArea.right - chartArea.left;
    const chartHeight = chartArea.bottom - chartArea.top;
    let gradient=null;
    let width = chartWidth;
    let height = chartHeight;
    if (gradient === null || width !== chartWidth || height !== chartHeight) {
      gradient = ctx.createLinearGradient(0, chartArea.bottom, 0, chartArea.top);
      if(colors.length>0){
          colors.forEach((color:string,index:number)=>{
              const step=(1/colors.length)* index;
            gradient.addColorStop(step, color);
          });
      }
    }
  
    return gradient;
}
