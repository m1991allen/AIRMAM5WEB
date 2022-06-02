/**
 * video player抽象類別
 */
export abstract class videoPlayerAbstractClass {
    abstract Load(fileURL: string, posterURL: string, rangeTime?: { start: number; end: number });
    abstract GetCurrentTime(): number;
    abstract GetTotoalTime(): number;
    abstract GetTotoalTimeCode(): string;
    abstract GetCurrentTimeCode(): string;
    abstract SeekTo(progress: number): void;
    abstract Pause(): void;
    abstract Destory(): void;
    abstract SetCurrentTime(currentSeconds: number): void;
}
/**
 * audio player抽象類別
 */
export abstract class audioPlayerAbstractClass {
    abstract Load(fileURL: string, posterURL: string);
    abstract GetCurrentTime(): number;
    abstract GetTotoalTime(): number;
    abstract GetTotoalTimeCode(): string;
    abstract GetCurrentTimeCode(): string;
    abstract SeekTo(progress: number): void;
    abstract Pause(): void;
    abstract Destory(): void;
    abstract SetCurrentTime(currentSeconds: number): void;
}
