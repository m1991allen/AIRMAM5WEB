import { Config } from "inspire-tree";


declare module "inspire-tree"{
    export interface Config{
        dom?:{
            deferredRendering?:boolean;   
        };
        visible: boolean;
    }
}

export {Config};