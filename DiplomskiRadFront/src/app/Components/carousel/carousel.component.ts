import {
  AfterViewInit,
  Component,
  ContentChild,
  ElementRef,
  HostListener,
  Input,
  NgZone,
  OnChanges,
  OnDestroy,
  SimpleChanges,
  TemplateRef,
  ViewChild,
} from '@angular/core';

interface ISlide {
  item: any;
  index: number;
}

@Component({
  selector: 'app-carousel',
  templateUrl: './carousel.component.html',
  styleUrls: ['./carousel.component.scss'],
})
export class CarouselComponent implements OnChanges, AfterViewInit, OnDestroy {
  @Input() items: any[] = [];
  @Input() perView: number = 4;
  @Input() gap: number = 20;
  @Input() autoplayMs: number = 0;
  @Input() ariaLabel: string = 'Carousel';

  @ContentChild(TemplateRef) itemTemplate!: TemplateRef<any>;
  @ViewChild('viewport') viewportRef!: ElementRef<HTMLElement>;

  slides: ISlide[] = [];
  visible: number = 1;
  clones: number = 0;
  pos: number = 0;
  itemWidth: number = 0;
  animate: boolean = false;
  ready: boolean = false;
  looping: boolean = false;

  private busy: boolean = false;
  private hovering: boolean = false;
  private timer: any = null;
  private busyTimer: any = null;
  private resizeObserver?: ResizeObserver;
  private touchStartX: number | null = null;

  private readonly transitionMs = 550;

  constructor(private zone: NgZone) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['items'] || changes['perView']) {
      this.rebuild(0);
    }

    if (changes['autoplayMs']) {
      this.restartAutoplay();
    }
  }

  ngAfterViewInit(): void {
    setTimeout(() => this.measure());

    if (typeof ResizeObserver !== 'undefined') {
      this.zone.runOutsideAngular(() => {
        this.resizeObserver = new ResizeObserver(() => {
          this.zone.run(() => this.measure());
        });

        this.resizeObserver.observe(this.viewportRef.nativeElement);
      });
    }

    this.restartAutoplay();
  }

  ngOnDestroy(): void {
    this.stopAutoplay();
    clearTimeout(this.busyTimer);
    this.resizeObserver?.disconnect();
  }

  get offset(): number {
    return -this.pos * (this.itemWidth + this.gap);
  }

  private responsiveVisible(width: number): number {
    const max = Math.max(1, this.perView);
    if (width >= 1100) return max;
    if (width >= 800) return Math.min(max, 3);
    if (width >= 520) return Math.min(max, 2);
    return 1;
  }

  private measure(): void {
    const el = this.viewportRef?.nativeElement;
    if (!el) return;
    const width = el.clientWidth;
    if (!width) return;

    const visible = this.responsiveVisible(window.innerWidth);
    const logical = this.logicalIndex();

    if (visible !== this.visible) {
      this.visible = visible;
      this.rebuild(logical);
    }

    this.itemWidth = (width - this.gap * (this.visible - 1)) / this.visible;
    this.ready = true;
  }

  private logicalIndex(): number {
    const n = this.items?.length || 0;
    if (!this.looping || n === 0) return 0;
    return (((this.pos - this.clones) % n) + n) % n;
  }

  private rebuild(logical: number): void {
    const items = this.items || [];
    const n = items.length;
    this.visible = this.visible || 1;

    if (typeof window !== 'undefined') {
      this.visible = this.responsiveVisible(window.innerWidth);
    }

    this.looping = n > this.visible;
    this.clones = this.looping ? this.visible : 0;

    const toSlide = (item: any, index: number): ISlide => ({ item, index });
    const head = this.looping
      ? items
          .slice(n - this.clones)
          .map((it, i) => toSlide(it, n - this.clones + i))
      : [];
    const body = items.map((it, i) => toSlide(it, i));
    const tail = this.looping
      ? items.slice(0, this.clones).map((it, i) => toSlide(it, i))
      : [];
    this.slides = [...head, ...body, ...tail];

    this.animate = false;
    this.pos = this.clones + (this.looping ? Math.min(logical, n - 1) : 0);
    this.busy = false;
    setTimeout(() => (this.animate = true), 30);
  }

  next(): void {
    this.move(1);
  }

  prev(): void {
    this.move(-1);
  }

  private move(direction: number): void {
    if (!this.looping || this.busy) return;
    this.busy = true;
    this.animate = true;
    this.pos += direction;

    clearTimeout(this.busyTimer);

    this.busyTimer = setTimeout(() => {
      const n = this.items.length;

      if (this.pos >= this.clones + n) {
        this.jump(this.pos - n);
      } else if (this.pos < this.clones) {
        this.jump(this.pos + n);
      }

      this.busy = false;
    }, this.transitionMs + 30);
  }

  private jump(pos: number): void {
    this.animate = false;
    this.pos = pos;
    setTimeout(() => (this.animate = true), 30);
  }

  private restartAutoplay(): void {
    this.stopAutoplay();

    if (this.autoplayMs > 0) {
      this.zone.runOutsideAngular(() => {
        this.timer = setInterval(() => {
          if (!this.hovering && !document.hidden) {
            this.zone.run(() => this.next());
          }
        }, this.autoplayMs);
      });
    }
  }

  private stopAutoplay(): void {
    if (this.timer) {
      clearInterval(this.timer);
      this.timer = null;
    }
  }

  setHover(value: boolean): void {
    this.hovering = value;
  }

  onTouchStart(event: TouchEvent): void {
    this.touchStartX = event.touches[0].clientX;
    this.hovering = true;
  }

  onTouchEnd(event: TouchEvent): void {
    if (this.touchStartX != null) {
      const dx = event.changedTouches[0].clientX - this.touchStartX;

      if (Math.abs(dx) > 40) {
        dx < 0 ? this.next() : this.prev();
      }
    }

    this.touchStartX = null;
    this.hovering = false;
  }

  @HostListener('window:resize')
  onResize(): void {
    this.measure();
  }

  trackSlide(index: number): number {
    return index;
  }
}
