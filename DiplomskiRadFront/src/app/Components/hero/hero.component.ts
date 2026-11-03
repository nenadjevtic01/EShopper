import { Component, OnDestroy, OnInit } from '@angular/core';
import { HeroSlideService } from '../../Services/hero-slide.service';
import { IHeroSlide } from '../../Interfaces/i-hero-slide';

@Component({
  selector: 'app-hero',
  templateUrl: './hero.component.html',
  styleUrls: ['./hero.component.scss'],
})
export class HeroComponent implements OnInit, OnDestroy {
  slides: IHeroSlide[] = [];
  index: number = 0;
  previous: number = -1;
  loaded: boolean = false;

  private queryCache = new Map<string, { [key: string]: string }>();
  private timer: any = null;
  private hovering: boolean = false;
  private readonly intervalMs = 7000;

  constructor(private heroSlideService: HeroSlideService) {}

  ngOnInit(): void {
    this.heroSlideService.getSlides().subscribe({
      next: (slides) => {
        this.slides = slides;
        this.loaded = true;
        this.startTimer();
      },
      error: () => {
        this.loaded = true;
      },
    });
  }

  ngOnDestroy(): void {
    this.stopTimer();
  }

  go(target: number): void {
    if (this.slides.length < 2 || target === this.index) return;
    this.previous = this.index;
    this.index = (target + this.slides.length) % this.slides.length;
    this.startTimer();
  }

  next(): void {
    this.go(this.index + 1);
  }

  prev(): void {
    this.go(this.index - 1);
  }

  setHover(value: boolean): void {
    this.hovering = value;
  }

  isExternal(link: string): boolean {
    return /^https?:\/\//i.test(link || '');
  }

  linkPath(link: string): string {
    return (link || '').split('?')[0].split('#')[0] || '/';
  }

  linkQuery(link: string): { [key: string]: string } {
    const cached = this.queryCache.get(link);

    if (cached) {
      return cached;
    }

    const query = (link || '').split('#')[0].split('?')[1];
    const params: { [key: string]: string } = {};

    if (query) {
      new URLSearchParams(query).forEach((value, key) => (params[key] = value));
    }

    this.queryCache.set(link, params);
    return params;
  }

  showMedia(i: number): boolean {
    return i === this.index || i === this.previous;
  }

  private startTimer(): void {
    this.stopTimer();

    if (this.slides.length > 1) {
      this.timer = setInterval(() => {
        if (!this.hovering && !document.hidden) {
          this.previous = this.index;
          this.index = (this.index + 1) % this.slides.length;
        }
      }, this.intervalMs);
    }
  }

  private stopTimer(): void {
    if (this.timer) {
      clearInterval(this.timer);
      this.timer = null;
    }
  }
}
