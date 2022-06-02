import { IwavesurferService } from '../Interface/Service/IwavesurferService';


/**
 * (需要先引用wavesurfer.js=> https://wavesurfer-js.org/)
 * 音頻波形服務
 */
export class wavesurferService implements IwavesurferService {
    protected wavesurfer: WaveSurfer;
    constructor(settings: WavesurferOptions) {
        this.wavesurfer = WaveSurfer.create(settings);
    }
    load(url: string, peaks?: any[], preload?: ['none' | 'metadata' | 'auto']): void {
        this.wavesurfer.load(url, peaks, preload);
    }
    loadBlob(url: string): void {
        this.wavesurfer.loadBlob(url);
    }
    play(start?: number, end?: number): void {
        this.wavesurfer.play();
    }
    pause(): void {
        this.wavesurfer.pause();
    }
    playPause(): void {
        this.wavesurfer.playPause();
    }
    stop(): void {
        this.wavesurfer.stop();
    }
    toggleMute(): void {
        this.wavesurfer.toggleMute();
    }
    toggleScroll(): void {
        this.wavesurfer.toggleScroll();
    }
    zoom(pxPerSec: number): void {
        this.wavesurfer.zoom(pxPerSec);
    }
    isPlaying(): boolean {
        return this.wavesurfer.isPlaying();
    }
    skip(offset: number): void {
        this.wavesurfer.skip(offset);
    }
    skipBackward(): void {
        this.wavesurfer.skipBackward();
    }
    skipForward(): void {
        this.wavesurfer.skipForward();
    }
    getReady() {
        return this.wavesurfer.getReady();
    }
    getPlaybackRate(): number {
        return this.wavesurfer.getPlaybackRate();
    }
    getVolume(): number {
        return this.wavesurfer.getVolume();
    }
    getMute() {
        return this.wavesurfer.getMute();
    }
    getCurrentTime(): number {
        return this.wavesurfer.getCurrentTime();
    }
    getDuration(): number {
        return this.wavesurfer.getDuration();
    }
    setVolume(newVolume: number): void {
        this.wavesurfer.setVolume(newVolume);
    }
    setMute(mute: boolean): void {
        this.wavesurfer.setMute(mute);
    }
    setHeight(height: any): void {
        this.wavesurfer.setHeight(height);
    }
    setPlaybackRate(rate: number): void {
        this.wavesurfer.setPlaybackRate(rate);
    }
    getProgressColor(): string {
        return this.wavesurfer.getProgressColor();
    }
    getBackgroundColor(): string {
        return this.wavesurfer.getBackgroundColor();
    }
    getCursorColor(): string {
        return this.wavesurfer.getCursorColor();
    }
    getWaveColor(): string {
        return this.wavesurfer.getWaveColor();
    }
    setBackgroundColor(color: any): void {
        this.wavesurfer.setBackgroundColor(color);
    }
    setCursorColor(color: any): void {
        this.wavesurfer.setCursorColor(color);
    }
    setProgressColor(color: any): void {
        this.wavesurfer.setProgressColor(color);
    }
    setWaveColor(color: any): void {
        this.wavesurfer.setWaveColor(color);
    }
    cancelAjax(): void {
        this.wavesurfer.cancelAjax();
    }
    destroy(): void {
        this.wavesurfer.destroy();
    }
    empty(): void {
        this.wavesurfer.empty();
    }
    on(eventName: WavesurferEvent, callback: (x?: any) => void): Disposer {
        return this.wavesurfer.on(eventName, callback);
    }
    un(eventName: WavesurferEvent, callback: (x?: any) => void): void {
        this.wavesurfer.un(eventName, callback);
    }
    unAll(): void {
        this.wavesurfer.unAll();
    }
    seekAndCenter(progress: number): void {
        this.wavesurfer.seekAndCenter(progress);
    }
    seekTo(progress: number): void {
        this.wavesurfer.seekTo(progress);
    }
    setSinkId(deviceId: any): void {
        this.wavesurfer.setSinkId(deviceId);
    }
    setFilter(filters: any[]): void {
        this.wavesurfer.setFilter(filters);
    }
    getActivePlugins() {
        return this.wavesurfer.getActivePlugins();
    }
    getFilters(): any[] {
        return this.wavesurfer.getFilters();
    }
    toggleInteraction(): void {
        this.wavesurfer.toggleInteraction();
    }
    exportPCM(length?: number, accuracy?: number, noWindow?: boolean, start?: number): JSON {
        return this.wavesurfer.exportPCM(length, accuracy, noWindow, start);
    }
    exportImage(format: string, quality: number, type: string): string | URL | Blob {
        return this.wavesurfer.exportImage(format, quality, type);
    }
}
