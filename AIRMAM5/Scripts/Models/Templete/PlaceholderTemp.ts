/** 列表佔位符*/
export const ListPlaceholder=():string=>{
    return`<div class="ui inverted segment">
            <div class="ui inverted placeholder">
            <div class="line"></div>
            <div class="line"></div>
            <div class="line"></div>
            </div>
           </div>`;
}
/**帶有圖片的列表佔位符 */
export const ImageListPlaceholder=():string=>{
    return`<div class="ui inverted placeholder">
            <div class="image header">
               <div class="line"></div>
               <div class="line"></div>
            </div>
            <div class="image header">
               <div class="line"></div>
               <div class="line"></div>
            </div>
            <div class="image header">
                <div class="line"></div>
                <div class="line"></div>
             </div>
            <div class="image header">
              <div class="line"></div>
              <div class="line"></div>
            </div>
           </div>`;
}
/**
 * 卡佔位符
 */
export const CardsPlaceholder=():string=>{
    return `<div class="ui three stackable cards">
    <div class="ui card">
      <div class="image">
        <div class="ui inverted placeholder">
          <div class="square image"></div>
        </div>
      </div>
      <div class="inverted content">
        <div class="ui inverted placeholder">
          <div class="header">
          <div class="short line"></div>
            <div class="medium line"></div>
          </div>
        </div>
      </div>
    </div>
  </div>`;
};
/**圖片佔位符 */
export const ImagePlaceholder=():string=>{
    return `<div class="ui active inverted placeholder"><div class="square fluid image"></div></div>`;
};