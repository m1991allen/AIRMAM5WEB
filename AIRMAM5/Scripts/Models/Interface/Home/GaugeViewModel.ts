/**
 * 
 */
// export interface GaugeViewModel{
//    id:string;
//    data:Array<number>;
//    // needleValue:number;
//    color:string;
//    title:string;
//}
/**
 * Dashboard�D���ե�,�J�w�@�~ �Ϫ��� */
export interface GaugeWorkModel {
    GaugeId: string;
    GaugeData: Array<number>;
    GaugeColor: string;
    GaugeTitle: string;
}