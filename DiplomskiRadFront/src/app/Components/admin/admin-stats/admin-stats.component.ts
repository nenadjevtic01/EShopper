import { Component, OnInit } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { StatsService } from '../../../Services/stats.service';
import { ToastService } from '../../../Services/toast.service';

interface IShareSegment {
  name: string;
  quantitySold: number;
  percentage: number;
  color: string;
}

interface IMonthlyBar {
  month: string;
  value: number;
  heightPct: number;
}

interface IDonutSegment {
  name: string;
  orderCount: number;
  percentage: number;
  color: string;
}

@Component({
  selector: 'app-admin-stats',
  templateUrl: './admin-stats.component.html',
  styleUrls: ['./admin-stats.component.scss'],
})
export class AdminStatsComponent implements OnInit {
  headers: any;
  loading: boolean = false;
  errored: boolean = false;

  totalOrders: number = 0;
  totalRevenue: number = 0;
  averageOrderValue: number = 0;

  orderBars: IMonthlyBar[] = [];
  revenueBars: IMonthlyBar[] = [];

  categoryShares: IShareSegment[] = [];
  brandShares: IShareSegment[] = [];

  paymentMethodShares: IDonutSegment[] = [];
  paymentMethodGradient: string = '';

  private readonly categoricalPalette: string[] = [
    '#2a78d6',
    '#eb6834',
    '#1baf7a',
    '#eda100',
    '#e87ba4',
    '#008300',
    '#4a3aa7',
    '#e34948',
  ];

  constructor(
    private statsService: StatsService,
    private toastService: ToastService,
  ) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    this.headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    this.load();
  }

  load(): void {
    this.loading = true;

    this.statsService.getStats(this.headers).subscribe({
      next: (data: any) => {
        this.loading = false;
        this.totalOrders = data.totalOrders;
        this.totalRevenue = data.totalRevenue;
        this.averageOrderValue = data.averageOrderValue;
        this.orderBars = this.toBars(
          data.monthlyStats,
          (m: any) => m.orderCount,
        );
        this.revenueBars = this.toBars(
          data.monthlyStats,
          (m: any) => m.revenue,
        );
        this.categoryShares = this.toShares(data.categoryShares);
        this.brandShares = this.toShares(data.brandShares);
        this.paymentMethodShares = this.toDonutSegments(
          data.paymentMethodShares,
        );
        this.paymentMethodGradient = this.toConicGradient(
          this.paymentMethodShares,
        );
      },
      error: () => {
        this.loading = false;
        this.errored = true;
        this.toastService.error('Could not load stats.');
      },
    });
  }

  private toBars(
    monthlyStats: any[],
    valueSelector: (m: any) => number,
  ): IMonthlyBar[] {
    if (!monthlyStats || monthlyStats.length === 0) {
      return [];
    }

    const max = Math.max(...monthlyStats.map(valueSelector), 1);

    return monthlyStats.map((m) => ({
      month: m.month,
      value: valueSelector(m),
      heightPct: Math.max(
        (valueSelector(m) / max) * 100,
        valueSelector(m) > 0 ? 4 : 0,
      ),
    }));
  }

  private toShares(shares: any[]): IShareSegment[] {
    if (!shares || shares.length === 0) {
      return [];
    }

    const cap = this.categoricalPalette.length;
    let items = shares;

    if (shares.length > cap) {
      const kept = shares.slice(0, cap - 1);
      const rest = shares.slice(cap - 1);

      const other = {
        name: 'Other',
        quantitySold: rest.reduce(
          (sum: number, s: any) => sum + s.quantitySold,
          0,
        ),
        percentage:
          Math.round(
            rest.reduce((sum: number, s: any) => sum + s.percentage, 0) * 10,
          ) / 10,
      };

      items = [...kept, other];
    }

    return items.map((s: any, i: number) => ({
      name: s.name,
      quantitySold: s.quantitySold,
      percentage: s.percentage,
      color: this.categoricalPalette[i % this.categoricalPalette.length],
    }));
  }

  private toDonutSegments(shares: any[]): IDonutSegment[] {
    if (!shares || shares.length === 0) {
      return [];
    }

    const cap = this.categoricalPalette.length;
    let items = shares;

    if (shares.length > cap) {
      const kept = shares.slice(0, cap - 1);
      const rest = shares.slice(cap - 1);

      const other = {
        name: 'Other',
        orderCount: rest.reduce((sum: number, s: any) => sum + s.orderCount, 0),
        percentage:
          Math.round(
            rest.reduce((sum: number, s: any) => sum + s.percentage, 0) * 10,
          ) / 10,
      };

      items = [...kept, other];
    }

    return items.map((s: any, i: number) => ({
      name: s.name,
      orderCount: s.orderCount,
      percentage: s.percentage,
      color: this.categoricalPalette[i % this.categoricalPalette.length],
    }));
  }

  private toConicGradient(segments: IDonutSegment[]): string {
    if (!segments || segments.length === 0) {
      return '';
    }

    let cursor = 0;
    const stops: string[] = [];

    for (const s of segments) {
      const start = cursor;
      const end = cursor + s.percentage;
      stops.push(`${s.color} ${start}% ${end}%`);
      cursor = end;
    }

    if (cursor < 100) {
      stops.push(`var(--gridline) ${cursor}% 100%`);
    }

    return `conic-gradient(${stops.join(', ')})`;
  }
}
